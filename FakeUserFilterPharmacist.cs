using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyPharmacyIntegrationTests
{
    public class FakeUserFilterPharmacist : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Role, "Pharmacist"),    
                    new Claim("PharmacyId", "1")                 
                });
           

            var claimPrincipal = new ClaimsPrincipal();
            claimPrincipal.AddIdentity(identity);

            context.HttpContext.User = claimPrincipal;
            await next();
        }
    }
}
