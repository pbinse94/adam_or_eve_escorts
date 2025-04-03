using Business.Communication;
using Business.IServices;
using Irony.Parsing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json; 
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Resources;
using System.Net;
using UAParser;

namespace Web.Controllers
{
    [AllowAnonymous, ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IUserVerificationCodeService _userVerificationCodeService;
        public AccountController(IHttpContextAccessor httpContextAccessor,
            IAccountService accountService,
            ISubscriptionPlanService subscriptionPlanService,
            INotificationService notificationService,
            IUserVerificationCodeService userVerificationCodeService)
        {
            _accountService = accountService;
            _subscriptionPlanService = subscriptionPlanService;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _userVerificationCodeService = userVerificationCodeService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginViewModel)
        {
            var userAgent = HttpContext?.Request?.Headers["User-Agent"].FirstOrDefault() ?? "Unknown Device";
            var ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var uaParser = UAParser.Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);
            string browserName = clientInfo.UA.Family; // e.g., Chrome, Firefox, Safari
            string browserVersion = clientInfo.UA.Major; // e.g., 130 for Chrome 130.x
            string osName = clientInfo.OS.Family; // e.g., Windows, Mac OS, Linux
            string deviceName = clientInfo.Device.Family; // e.g., Desktop, iPhone, etc.
            var deviceDetails = string.Empty;
            if (deviceName.ToLower() != "other")
            {
                deviceDetails = $"{ipAddress}, {deviceName} - {osName} server, {browserName} version {browserVersion}";
            }
            else
            {
                deviceDetails = $"{ipAddress}, {osName} server, {browserName} version {browserVersion}";
            }
            string deviceLocation = CommonFunctions.GetLocationFromIp(ipAddress);
            ViewBag.ShowEmailVerificationPopUp = false;
            var loginResponse = await _accountService.Login(new ApiLoginRequest()
            {
                Email = loginViewModel.Email,
                Password = loginViewModel.Password,
                DeviceToken = HttpContext?.Session.Id ?? "Unknown session",
                DeviceInfo = deviceDetails.ToString(),
                DeviceLocation = deviceLocation,
            });

            if (loginResponse.Data is null)
            {
                bool showEmailVerificationPopUp = false;
                if (loginResponse.Message?.Equals(ResourceString.EmailNotVerified) ?? false)
                {
                    showEmailVerificationPopUp = true;
                }
                ViewBag.message = loginResponse.Message;
                return BadRequest(new ApiResponse<object> { Data = new { ShowEmailVerificationPopUp = showEmailVerificationPopUp }, Message = loginResponse.Message });
            }

            //Remember me
            if (loginViewModel.RememberMe)
            {
                CookieData cookieData = new CookieData { Email = loginViewModel.Email, Password = loginViewModel.Password };
                string cookieValue = JsonConvert.SerializeObject(cookieData);

                Response.Cookies.Append("RememberMeCookie", cookieValue, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMinutes(SiteKeys.RememberMeCookieTimeMinutes)
                });
            }

            //Store value in session
            LoginSessionModel sessionobj = new LoginSessionModel()
            {
                UserId = loginResponse.Data.UserId,
                FirstName = loginResponse.Data.FirstName,
                LastName = loginResponse.Data.LastName,
                DisplayName = loginResponse.Data.DisplayName,
                UserTypeId = loginResponse.Data.UserType,
                EmailId = loginResponse.Data.Email,
                ProfileImage = loginResponse.Data.ProfileImage,
                AccessToken = loginResponse.Data.AccessToken,
                SubscriptionPlanDurationType = (SubscriptionPlanDurationType)loginResponse.Data.PlanDuration,
                SubscriptionPlanType = (SubscriptionPlanType)loginResponse.Data.PlanType,
                SubscriptionPlanId = loginResponse.Data.SubscriptionPlanId,

                SubscriptionPlanExpireDateTime = loginResponse.Data.SubscriptionPlanExpireDateTime,
                SubscriptionPlanExpireOn = loginResponse.Data.SubscriptionPlanExpireDateTime.ToLocal(_httpContextAccessor),
                SubscriptionPaypalId = loginResponse.Data.SubscriptionPaypalId,
                SubscriptionPlanPaypalId = loginResponse.Data.SubscriptionPlanPaypalId
            };

            LoginMemberSession.UserDetailSession = sessionobj;

            var returnurl = TempData["ReturnUrl"]?.ToString() ?? null;
            TempData["ReturnUrl"] = null;
            return Ok(new ApiResponse<object>
            {
                Data = sessionobj,
                Message = loginResponse.Message,
                ApiName = returnurl
            });

        }

        public async Task<IActionResult> Logout()
        {
            if (LoginMemberSession.UserDetailSession != null)
            {
                await _accountService.LogoutUser(LoginMemberSession.UserDetailSession.UserId, LoginMemberSession.UserDetailSession.AccessToken ?? string.Empty);
            }

            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }



        public async Task<IActionResult> LogoutFromAllDevices(string accesstoken)
        {
            var logoutUser = await _accountService.LogoutAllUser(accesstoken);
            ViewBag.IsURLValid = logoutUser?.Data;
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return View();
        }

        [HttpGet]
        [Route("SignUp")]
        public async Task<IActionResult> Registration()
        {
            RegistrationRequest registrationRequest = new();
            var countryCodes = await _accountService.GetCountryCodes();
            ViewBag.SelectListItems = countryCodes.Select(x => new SelectListItem() { Text = x.CountryName, Value = x.CountryName }).ToList();
            return View(registrationRequest);
        }

        [HttpPost]
        public async Task<ActionResult> Registration(RegistrationRequest request)
        {
            if (!request.TermsAndCondtion)
            {
                return StatusCode((int)HttpStatusCode.UnprocessableContent, new ApiResponse<int> { Message = ResourceString.AcceptTermAndConditons });
            }
            var userData = await _userVerificationCodeService.GetByEmail(request.Email);
            if (userData is null || userData.Email != request.Email || !userData.IsVerify)
            {
                return BadRequest(new ApiResponse<ProfileDto> { Message = ResourceString.EmailNotVerified });
            }
            request.IsEmailVerified = true;

            //var subscriptionPlan = await _subscriptionPlanService.GetSubscriptionPlanById(request.Plan);
            //if (subscriptionPlan == null)
            //{
            //    return BadRequest(new ApiResponse<int> { Message = ResourceString.SomethingWrong });
            //}

            var userAgent = HttpContext?.Request?.Headers["User-Agent"].FirstOrDefault() ?? "Unknown Device";
            var ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var uaParser = UAParser.Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);
            string browserName = clientInfo.UA.Family; // e.g., Chrome, Firefox, Safari
            string browserVersion = clientInfo.UA.Major; // e.g., 130 for Chrome 130.x
            string osName = clientInfo.OS.Family; // e.g., Windows, Mac OS, Linux
            string deviceName = clientInfo.Device.Family; // e.g., Desktop, iPhone, etc.
            var deviceDetails = string.Empty;
            if (deviceName.ToLower() != "other")
            {
                deviceDetails = $"{ipAddress}, {deviceName} - {osName} server, {browserName} version {browserVersion}";
            }
            else
            {
                deviceDetails = $"{ipAddress}, {osName} server, {browserName} version {browserVersion}";
            }
            string deviceLocation = CommonFunctions.GetLocationFromIp(ipAddress);

            request.DeviceToken = HttpContext?.Session.Id ?? "Unknown session";
            request.DeviceInfo = deviceDetails.ToString();
            request.DeviceLocation = deviceLocation;
           
            var registrationResponse = await _accountService.SignUp(request);
            if (registrationResponse.Data is null || registrationResponse.Data.UserId == 0)
            {
                return BadRequest(registrationResponse);
            }
            //Login  user if registration successfully 
            LoginSessionModel sessionobj = new LoginSessionModel()
            {
                UserId = registrationResponse.Data.UserId,
                FirstName = registrationResponse.Data.FirstName,
                LastName = registrationResponse.Data.LastName,
                DisplayName = registrationResponse.Data.DisplayName,
                UserTypeId = registrationResponse.Data.UserType,
                EmailId = registrationResponse.Data.Email,
                ProfileImage = registrationResponse.Data.ProfileImage,
                AccessToken = registrationResponse.Data.AccessToken,
                SubscriptionPlanDurationType = (SubscriptionPlanDurationType)registrationResponse.Data.PlanDuration,
                SubscriptionPlanType = (SubscriptionPlanType)registrationResponse.Data.PlanType,
                SubscriptionPlanId = registrationResponse.Data.SubscriptionPlanId,

                SubscriptionPlanExpireDateTime = registrationResponse.Data.SubscriptionPlanExpireDateTime,
                SubscriptionPlanExpireOn = registrationResponse.Data.SubscriptionPlanExpireDateTime.ToLocal(_httpContextAccessor),
                SubscriptionPaypalId = registrationResponse.Data.SubscriptionPaypalId,
                SubscriptionPlanPaypalId = registrationResponse.Data.SubscriptionPlanPaypalId
            };
            LoginMemberSession.UserDetailSession = sessionobj;
            return Ok(registrationResponse);

            //if (subscriptionPlan.PlanType == (short)SubscriptionPlanType.Free)
            //{
            //    TempData["FreePlan"] = true;
            //    TempData["FreePlanEmail"] = request.Email;
            //    return Ok(new ApiResponse<object> { Data = new { Url = Url.Action("Success", "Account") } });
            //}
            //else
            //{
            //   var stripeSession = await _subscriptionPlanService.GetStripePaymentLink(subscriptionPlan.PriceId, request.Email);

            //    if (stripeSession == null)
            //    {
            //        return BadRequest(new ApiResponse<int> { Message = ResourceString.SomethingWrong });
            //    }

            //    return Ok(stripeSession);
            //}

        }




        public async Task<IActionResult> EmailVerification(string token, int type = 0)
        {
            EmailVerifyRequest request = new();
            request.Token = token;
            var isEmailVerified = await _accountService.VerifyEmail(request);
            ViewBag.type = type;
            if (isEmailVerified.Data)
            {
                ViewBag.EmailVerified = true;

            }
            else
            {
                ViewBag.EmailVerified = false;
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgetPassword(ForgotPasswordRequestModel model)
        {
            var getUserByEmail = await _accountService.FindByEmailAsync(model.Email);

            if (getUserByEmail != null)
            {
                var updateResetToken = await _accountService.ResetPasswordTokenAsync(Convert.ToInt64(getUserByEmail.UserId));

                if (updateResetToken.Data != null)
                {
                    return Ok(new ApiResponse<string> { Message = updateResetToken.Message });
                }
                else
                {
                    return BadRequest(new ApiResponse<string> { Message = updateResetToken.Message });
                }
            }
            else
            {
                return BadRequest(new ApiResponse<string> { Message = ResourceString.UserNotExist });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            ResetPasswordModel model = new();
            model.Token = token;
            if (!string.IsNullOrEmpty(token))
            {
                var isResetTokenExists = await _accountService.CheckResetPasswordTokenExist(token);
                model.ValidToken = isResetTokenExists;
            }
            else
            {
                model.ValidToken = false;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            var passwordSetup = await _accountService.ResetPassword(model);

            if (passwordSetup == ResponseTypes.Success)
            {
                return Ok(new ApiResponse<bool> { Message = ResourceString.PasswordUpdated });
            }
            else if (passwordSetup == ResponseTypes.OldNewPasswordMatched)
            {
                return BadRequest(new ApiResponse<bool> { Message = ResourceString.OldNewPasswordNotSame });
            }
            else
                return NotFound(new ApiResponse<bool> { Message = ResourceString.InvalidOrResetTokenExpired });
        }

        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> VerifyEmail(VerifyEmailRequestModel model)
        {
            var getUserByEmail = await _accountService.FindByEmailAsync(model.Email);
            if (getUserByEmail != null)
            {
                if (getUserByEmail.IsEmailVerified)
                {
                    return BadRequest(new ApiResponse<string> { Message = ResourceString.EmailAlreadyVerified });
                }
                else if (getUserByEmail.IsDeleted ?? false)
                {
                    return BadRequest(new ApiResponse<string> { Message = ResourceString.UserAccountDeleted });
                }
                else if (!(getUserByEmail.IsActive ?? false))
                {
                    return BadRequest(new ApiResponse<string> { Message = ResourceString.DeactivateUser });
                }
                else
                {
                    var updateEmailVerificationToken = await _accountService.UpdateEmailVerificationToken(getUserByEmail.UserId, getUserByEmail.Email ?? string.Empty, getUserByEmail.DisplayName ?? string.Empty);
                    if (updateEmailVerificationToken != null)
                    {
                        if (updateEmailVerificationToken.Data)
                        {
                            return Ok(new ApiResponse<string> { Message = updateEmailVerificationToken.Message });
                        }
                        else
                        {
                            return BadRequest(new ApiResponse<string> { Message = updateEmailVerificationToken.Message });
                        }
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<string> { Message = ResourceString.VerificationLinkSentFailed });
                    }
                }
            }
            else
            {
                return NotFound(new ApiResponse<string> { Message = ResourceString.UserNotExist });
            }
        }




        [HttpPost]
        public async Task<ActionResult> SendVerifyEmail(VerifyEmailRequestModel model)
        {
            var getUserByEmail = await _accountService.FindByEmailAsync(model.Email);
            if (getUserByEmail != null)
            {
                return BadRequest(new ApiResponse<bool> { Data = false, Message = ResourceString.EmailExists });
            }
            else
            {
                Random random = new Random();
                string verifyCode = random.Next(100000, 999999).ToString();
                await _notificationService.EmailVerificationCode(model.Email, "", verifyCode, ResourceString.VerifyEmailSubject, 0);
                await _userVerificationCodeService.AddUpdate(new UserVerificationCode { Email = model.Email, IsVerify = false, Code = verifyCode });
                return Ok(new ApiResponse<bool> { Data = true, Message = ResourceString.SentVerifyCode });
            }
        }
        [HttpPost]
        public async Task<ActionResult> VerifyEmailCode(VerifyEmailCodeRequestModel model)
        {

            var data = await _userVerificationCodeService.GetByEmail(model.Email);
            if (data != null)
            {
                if (!string.Equals(data.Code, model.Code))
                {
                    return BadRequest(new ApiResponse<int> { Message = ResourceString.InvalidVerificationCode });
                }
                data.IsVerify = true;
                await _userVerificationCodeService.AddUpdate(data);
                return Ok(new ApiResponse<int> { Message = ResourceString.EmailCodeVerified });
            }
            else
            {
                return BadRequest(new ApiResponse<int> { Message = ResourceString.InvalidVerificationCode });
            }
        }
    }
}
