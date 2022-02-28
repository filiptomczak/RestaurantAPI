using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware
{
    public class TimerMiddleware : IMiddleware
    {
        private readonly ILogger<TimerMiddleware> _logger;
        private Stopwatch _s;
        public TimerMiddleware(ILogger<TimerMiddleware> logger)
        {
            _logger = logger;
            _s = new Stopwatch();
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            _s.Start();
            await next.Invoke(context);
            _s.Stop();
            if (_s.ElapsedMilliseconds >= 4000)
            {
                var msg = $"Request [{context.Request.Method}] at {context.Request.Path} " +
                     $"took { _s.ElapsedMilliseconds }ms";
                _logger.LogInformation(msg);
            }
           
        }
    }
}
