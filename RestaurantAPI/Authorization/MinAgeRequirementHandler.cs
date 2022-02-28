using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RestaurantAPI.Authorization
{
    public class MinAgeRequirementHandler : AuthorizationHandler<MinAgeRequirement>
    {
        private readonly ILogger<MinAgeRequirementHandler> _logger;
        public MinAgeRequirementHandler(ILogger<MinAgeRequirementHandler>logger)
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinAgeRequirement requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(u => u.Type == "DateOfBirth").Value);
            var userEmail = context.User.FindFirst(u => u.Type == ClaimTypes.Name).Value;
            _logger.LogInformation($"User:{userEmail} with date of birth :{dateOfBirth}");
            if (dateOfBirth.AddYears(requirement.Age) < DateTime.Today)
            {
                _logger.LogInformation("Authorization succedded");
                context.Succeed(requirement);
            }
            _logger.LogInformation("Authorization failed");
            return Task.CompletedTask;
        }
    }
}
