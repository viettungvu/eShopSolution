using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eShopSolution.AdminApp.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var session = context.HttpContext.Session.GetString("Token");
            if (session == null)
                context.Result = new RedirectToActionResult("Index", "Login", null);
            base.OnActionExecuted(context);
        }

        protected void SetAlert(string type, string message)
        {
            TempData["Message"] = message;
            switch (type)
            {
                case "success":
                    TempData["Type"] = "success";
                    break;

                case "warning":
                    TempData["Type"] = "warning";
                    break;

                case "info":
                    TempData["Type"] = "info";
                    break;

                case "danger":
                    TempData["Type"] = "danger";
                    break;

                case "dark":
                    TempData["Type"] = "dark";
                    break;

                default:
                    TempData["Type"] = "primary";
                    break;
            }
        }
    }
}