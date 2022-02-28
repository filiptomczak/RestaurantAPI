using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entitites;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        
        public void ConfigureServices(IServiceCollection services)
        {
            //konfiguracja tokenu JWT
            var authenticationSettings = new AuthenticationSettings();
            Configuration.GetSection("Authentication").Bind(authenticationSettings);
            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality"));
                options.AddPolicy("AtLeast20", builder => builder.AddRequirements(new MinAgeRequirement(20)));
                options.AddPolicy("Min2RestaurantsAdded", builder => builder.AddRequirements(new MinNumOfRestaurantAddedRequirement(2)));
            });

            services.AddScoped<IAuthorizationHandler, MinNumOfRestaurantAddedHandler>();
            services.AddScoped<IAuthorizationHandler, MinAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequitementHandler>();

            services.AddControllers();

            services.AddDbContext<RestaurantDbContext>(
                options=>options.UseSqlServer(Configuration.GetConnectionString("RestaurantDbConnection")));
            
            services.AddScoped<RestaurantSeeder>();
            services.AddAutoMapper(this.GetType().Assembly);

            services.AddScoped<IRestaurantService,RestaurantService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IUserService, UserService > ();

            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<TimerMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RestaurantQuery>,RestaurantQueryValidator> ();
            services.AddSwaggerGen();

            services.AddScoped<IUserContextService, UserContextService>();
            //dodatkowo, z racji ze korzytsamy z akcesora dostepu http, pozwala to wstrzyknac do
            //klasy UserContextService referencje do IHttpContextAccesor
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder =>
                
                    builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(Configuration["AllowedOrigins"])
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
        {
            app.UseResponseCaching();//cachowanie odpowiedzi
            app.UseStaticFiles();//umozliwia prace z plikami statycznymi
            app.UseCors("FrontEndClient");
            seeder.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //musi byc wywolany wczesniej zeby wylapywal blad przed requestem
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<TimerMiddleware>();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
            });


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
