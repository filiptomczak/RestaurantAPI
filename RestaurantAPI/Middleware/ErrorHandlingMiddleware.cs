using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        //dzieki temu middleware nie trzeba try catch w kazdym miejscu,
        //gdzie moze wystapic wyjatek, kazde zapytanie bedzie przechodzilo przez ten
        //middleware. Konieczna jest implementacja interfejsu IMiddleware

        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;//wstrzyknicecie zaleznosci
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            
            try
            {
                await next.Invoke(context);
            }
            catch(BadLoginException ble)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(ble.Message);
            }
            catch(NotFoundException nfe)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(nfe.Message);
            }
            catch(ConfrimPasswordException cpe)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(cpe.Message);
            }
            catch (EmailTakenException ete)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(ete.Message);
            }
            catch(ForbidException fe)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(fe.Message);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = 500;//internal server error
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
