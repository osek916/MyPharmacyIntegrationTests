using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyPharmacyIntegrationTests.Filters
{
    public class FakeManagerFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Role, "Manager"),
                    new Claim(ClaimTypes.NameIdentifier, "2")
                }));
            context.HttpContext.User = claimsPrincipal;
            await next();
        }
    }
}
