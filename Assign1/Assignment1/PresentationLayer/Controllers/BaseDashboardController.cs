using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Enum;

namespace PresentationLayer.Controllers
{
    public class BaseDashboardController : Controller
    {
        protected UserRole? CurrentUserRole { get; private set; }
        protected string CurrentUserName { get; private set; }
        protected string CurrentUserEmail { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            
            var isAnonymous = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();
            if (isAnonymous)
            {
                return;
            }

            
            var roleString = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserFullName");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            
            if (string.IsNullOrEmpty(roleString) || string.IsNullOrEmpty(userName))
            {
                TempData["Error"] = "Vui lòng đăng nhập để truy cập Dashboard.";
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            
            if (System.Enum.TryParse<UserRole>(roleString, out var role))
            {
                CurrentUserRole = role;
            }

            CurrentUserName = userName;
            CurrentUserEmail = userEmail ?? "";

            
            ViewBag.UserRole = CurrentUserRole;
            ViewBag.UserRoleName = CurrentUserRole.ToString();
            ViewBag.UserName = CurrentUserName;
            ViewBag.UserEmail = CurrentUserEmail;
        }

        protected bool IsDealer()
        {
            return CurrentUserRole == UserRole.DealerStaff || CurrentUserRole == UserRole.DealerManager;
        }

        protected bool IsAdmin()
        {
            return CurrentUserRole == UserRole.Admin || CurrentUserRole == UserRole.EVMStaff;
        }
    }
}

