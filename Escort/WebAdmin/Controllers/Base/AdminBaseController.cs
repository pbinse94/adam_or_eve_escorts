using Business.IServices;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.Request.Account;
using Shared.Resources;
using static Amazon.RuntimeDependencies.SecurityTokenServiceClientContext;

namespace WebAdmin.Controllers.Base
{
    public class AdminBaseController : Controller
    {
        public AdminBaseController()
        {
        }
        public int UserId { get; init; } = LoginMemberSession.UserDetailSession?.UserId ?? 0;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoginSessionModel? userObj = HttpContext.Session.GetComplexData<LoginSessionModel>("LoginMemberSession");
            var requestType = HttpContext.Request.Headers["X-Requested-With"];

            if (userObj != null)
            {
                // check user is valid with role
                if (!(new short[] { (short)UserTypes.SuperAdmin, (short)UserTypes.Admin, (short)UserTypes.Editor, (short)UserTypes.Viewer, (short)UserTypes.Management, (short)UserTypes.Accounting }
                                                .Contains((short)userObj.UserTypeId)))
                {
                    filterContext.Result = RedirectToAction("Logout", "Account", new { area = "" });
                }

                // check is user hit invalid request
                string controllerName = filterContext.RouteData.Values["Controller"]?.ToString() ?? String.Empty;
                if (!String.IsNullOrEmpty(controllerName) && controllerName == "AdminUser" && userObj.UserTypeId != (short)UserTypes.SuperAdmin)
                {
                    filterContext.Result = RedirectToAction("Logout", "Account", new { area = "" });
                }

                // check is user active or not
                if (userObj.UserTypeId != (short)UserTypes.SuperAdmin)
                {
                    var svc = filterContext.HttpContext.RequestServices;
                    var userAccountService = svc.GetService<IAccountService>();
                    var userDetails = userAccountService?.GetAdminDetailById(UserId, userObj.AccessToken ?? string.Empty).Result;

                    if (userDetails == null)
                    {
                        filterContext.Result = RedirectToAction("Logout", "Account", new { area = "" });
                        return;
                    }

                    if (!userDetails?.IsActive ?? false)
                    {
                        if (!string.IsNullOrEmpty(requestType) && requestType == "XMLHttpRequest")
                        {
                            HttpContext.SignOutAsync();
                            HttpContext.Session.Clear();
                            filterContext.Result = new UnauthorizedResult();
                            return;
                        }
                        else
                        {
                            TempData["ReturnUrl"] = filterContext.HttpContext.Request.Path.ToString();
                            TempData["AccountDecative"] = ResourceString.UserIsNotActive;
                            filterContext.Result = RedirectToAction("Logout", "Account", new { area = "" });
                            return;
                        }
                    }
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(requestType) && requestType == "XMLHttpRequest")
                {
                    filterContext.Result = new UnauthorizedResult();
                }
                else
                {
                    filterContext.Result = RedirectToAction("Logout", "Account", new { area = "" });
                }
            }
        }
    }
}
