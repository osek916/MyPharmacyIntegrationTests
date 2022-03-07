using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyPharmacyIntegrationTests
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IEnumerable<ClaimsIdentity> identities = new ClaimsIdentity[]
                        {
                            new ClaimsIdentity(
                                new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, "1"),
                                    new Claim(ClaimTypes.Role, "Admin"),
                                }),

                            new ClaimsIdentity(
                                new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, "2"),
                                    new Claim(ClaimTypes.Role, "Manager"),
                                })
                        };

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentities(identities);

            context.HttpContext.User = claimsPrincipal;
            await next();

        }
    }
}

//var claimsPrincipal = new ClaimsPrincipal();
//claimsPrincipal.AddIdentity(new ClaimsIdentity(
//    new[]
//    {
//                    new Claim(ClaimTypes.NameIdentifier, "1"),
//                    new Claim(ClaimTypes.Role, "Admin"),
//    }));

//context.HttpContext.User = claimsPrincipal;
//await next();


//IEnumerable<ClaimsIdentity> identities = new ClaimsIdentity[]
//            {
//                //new ClaimsIdentity(
//                //    new[]
//                //    {
//                //        new Claim(ClaimTypes.NameIdentifier, "1"),
//                //        new Claim(ClaimTypes.Role, "Admin"),
//                //    }),

//                new ClaimsIdentity(
//                    new[]
//                    {
//                        new Claim(ClaimTypes.NameIdentifier, "2"),
//                        new Claim(ClaimTypes.Role, "Manager"),
//                    })
//            };


//var claimsPrincipal = new ClaimsPrincipal();
//claimsPrincipal.AddIdentities(identities);



//context.HttpContext.User = claimsPrincipal;
//await next();