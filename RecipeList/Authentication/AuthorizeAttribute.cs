using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RecipeList.Authentication
{
    public class AuthorizeAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionUId = context.HttpContext.Session.GetInt32("_Userid");
            if (sessionUId == null)
            {
                context.Result = new RedirectResult("/account/login");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
