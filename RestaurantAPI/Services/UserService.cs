using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entitites;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Services
{
    public interface IUserService
    {
        void Register(RegisterUserDto dto);
        PagedResult<User> GetAll(UserQuery query);
        public string GenerateToken(LoginUserDto dto);
    }
    public class UserService : IUserService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;


        public UserService(RestaurantDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        public void Register(RegisterUserDto dto)
        {
            PasswordValidation(dto.Password, dto.ConfirmPassword);
            UniqueEmailValidation(dto.Email);

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        public PagedResult<User> GetAll(UserQuery query)
        {
            var baseQuery =_dbContext.Users.ToList();
            var totalUsersCount = baseQuery.Count;
            
            var users = baseQuery
                .Skip(query.PageSize*(query.PageNumber-1))
                .Take(query.PageSize)
                .ToList();
            if (users is null)
                return null;

            

            var result = new PagedResult<User>(users, query.PageSize, query.PageNumber, totalUsersCount);
            
            return result;
        }

        private bool PasswordValidation(string pass, string confirmPass)
        {
            if (pass == confirmPass) return true;
            throw new ConfrimPasswordException("Password does not match with confirm password");
        }
        private bool UniqueEmailValidation(string email)
        {

            var unique = _dbContext.Users.SingleOrDefault(u => u.Email == email);
            if (unique is null) return true;
            throw new EmailTakenException($"email: {email} already exists in database");
        }

        public string GenerateToken(LoginUserDto dto)
        {
            var user = _dbContext.Users.
                Include(user=>user.Role).
                SingleOrDefault(u => u.Email == dto.Email);
            if (user is null)
                throw new BadLoginException("Invalid email or password");
            var res = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (res == PasswordVerificationResult.Failed)
                throw new BadLoginException("Invalid email or password");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Email}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth",user.BirthDate.Value.ToString("yyyy-MM-dd")),
                //new Claim("Nationality",user.Nationality)
            };
            if (!string.IsNullOrEmpty(user.Nationality))
            {
                claims.Add(
                    new Claim("Nationality", user.Nationality)
                    );
            }


            /*if (user.NumOfCreatedRestaurants != null)
            {
                claims.Add(new Claim("AddedRestaurants", user.NumOfCreatedRestaurants.ToString()));
            }*/

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}