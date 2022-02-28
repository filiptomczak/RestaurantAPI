using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Entitites;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Controller
{
    [Route("api/account")]
    //[ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public ActionResult Register([FromBody]RegisterUserDto dto)
        {
            _userService.Register(dto);
            return Ok();
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody]LoginUserDto dto)
        {
            string token = _userService.GenerateToken(dto);
            return Ok(token);

        }
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll(UserQuery query)
        {
            var users = _userService.GetAll(query);
            return users!=null?Ok(users):NoContent();
        }

    }
}
