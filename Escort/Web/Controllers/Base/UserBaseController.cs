using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;

namespace Web.Controllers.Base
{
    public class UserBaseController : Controller
    {
        public UserBaseController()
        {
        }
        public int UserId { get; init; } = LoginMemberSession.UserDetailSession?.UserId ?? 0;


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                                 .Any(em => em.GetType() == typeof(AllowAnonymousAttribute)); //< -- Here it is

            IAccountService? commonServic = context.HttpContext.RequestServices.GetService(typeof(IAccountService)) as IAccountService;
            LoginSessionModel? userObj = HttpContext.Session.GetComplexData<LoginSessionModel>("LoginMemberSession");

            if (userObj != null)
            {
                var controllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
                if ((userObj.UserTypeId == (int)UserTypes.IndependentEscort || userObj.UserTypeId == (int)UserTypes.Establishment) && (userObj.SubscriptionPlanDurationType ?? 0) == 0 && controllerName.ToLower() != "subscription")
                {
                    context.Result = RedirectToAction("Index", "Subscription", new { area = "" });
                    return;
                }
            }

            if (hasAllowAnonymous)
            {

                if (userObj != null && !string.IsNullOrEmpty(userObj.EmailId))
                {
                    var isUserActive = commonServic?.FindByEmailAndUpdateAsync(email: userObj.EmailId.ToString(), accessToken: userObj.AccessToken ?? string.Empty);
                    var controller = context.Controller as ControllerBase;
                    if (isUserActive == null || isUserActive.Result == null)
                    {
                        context.Result = controller?.RedirectToAction("Logout", "Account", new { area = "" });
                        return;
                    }
                    return;
                }



                return;
            }


            var requestType = HttpContext.Request.Headers["X-Requested-With"];

            if (userObj != null)
            {
                var controllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;

                bool isHaveAccess = CheckActionAccess(controllerName, userObj.UserTypeId);

                if (!isHaveAccess)
                {
                    if (!string.IsNullOrEmpty(requestType) && requestType == "XMLHttpRequest")
                    {
                        context.Result = new UnauthorizedResult();
                    }
                    else
                    {
                        TempData["ReturnUrl"] = context.HttpContext.Request.Path.ToString() + context.HttpContext.Request.QueryString;

                        context.Result = RedirectToAction("Logout", "Account", new { area = "" });
                    }
                }



                if (commonServic != null && !string.IsNullOrEmpty(userObj.EmailId))
                {
                    var isUserActive = commonServic.FindByEmailAndUpdateAsync(email: userObj.EmailId.ToString(), accessToken: userObj.AccessToken ?? string.Empty);
                    var controller = context.Controller as ControllerBase;

                    if (isUserActive.Result == null)
                    {
                        context.Result = controller?.RedirectToAction("Logout", "Account", new { area = "" });
                        return;
                    }

                    if (!(isUserActive.Result.IsActive ?? false))
                    {
                        if (controller != null)
                        {
                            context.Result = controller.RedirectToAction("Logout", "Account", new { area = "" });
                        }
                        return;
                    }
                }

            }
            else
            {

                if (!string.IsNullOrEmpty(requestType) && requestType == "XMLHttpRequest")
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    TempData["ReturnUrl"] = context.HttpContext.Request.Path.ToString() + context.HttpContext.Request.QueryString;

                    context.Result = RedirectToAction("Logout", "Account", new { area = "" });
                }
            }
            base.OnActionExecuting(context);
        }

        private bool CheckActionAccess(string controllerName, int userType)
        {
            string[] establishmentArray = new string[] { "establishment", "profile", "testimonial", "chatauth", "subscription" };
            string[] escortArray = new string[] { "profile", "testimonial", "streaming", "chatauth", "subscription", "escort" };
            string[] clientArray = new string[] { "user", "testimonial", "chatauth", "credit" };
            if (userType == (int)UserTypes.Establishment)
            {
                return establishmentArray.Contains(controllerName.ToLower());
            }
            else if (userType == (int)UserTypes.IndependentEscort || userType == (int)UserTypes.EstablishmentEscort)
            {
                return escortArray.Contains(controllerName.ToLower());
            }
            else if (userType == (int)UserTypes.IndependentEscort)
            {
                return clientArray.Contains(controllerName.ToLower());
            }
            return true;
        }
    }
}