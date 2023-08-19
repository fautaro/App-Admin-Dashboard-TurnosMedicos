using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace TuNamespace.Attributes
{
    public class AuthorizeOrRedirectAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                await next();
            }
            else
            {
                context.Result = new RedirectResult("~/login");
            }
        }
    }
}
