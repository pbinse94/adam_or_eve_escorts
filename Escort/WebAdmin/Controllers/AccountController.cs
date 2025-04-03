using Amazon.Runtime.Internal.Util;
using Azure.Core;
using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.Request.Account;
using Shared.Model.Request.AdminUser;
using Shared.Resources;

namespace WebAdmin.Controllers
{
    [AllowAnonymous, ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IManageService _manageService;
        private readonly AdminPermissionService _adminPermissionService;

        public AccountController(AdminPermissionService adminPermissionService, IMemoryCache cache, IManageService manageService, IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
            _manageService = manageService;
            _adminPermissionService = adminPermissionService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (LoginMemberSession.UserDetailSession != null &&
                new short[] { (short)UserTypes.SuperAdmin, (short)UserTypes.Admin, (short)UserTypes.Editor, (short)UserTypes.Viewer, (short)UserTypes.Accounting, (short)UserTypes.Management }
                                                .Contains((short)LoginMemberSession.UserDetailSession.UserTypeId))
            {
                return RedirectToAction("Index", "Dashboard");
            }


            ViewBag.ShowEmailVerificationPopUp = false;
            var returnUrl = TempData["ReturnUrl"];
            LoginRequest model = new()
            {
                ReturnUrl = returnUrl == null ? string.Empty : (string)returnUrl,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginViewModel)
        {
            ViewBag.ShowEmailVerificationPopUp = false;
            var retrurnUrl = loginViewModel.ReturnUrl;

            var context = _httpContextAccessor.HttpContext;
            SiteKeys.UtcOffset = Convert.ToInt32(context?.Request.Cookies["timezoneoffset"]);

            if (Request.Cookies["timezoneoffset"] != null)
            {
                _httpContextAccessor.HttpContext?.Session.SetInt32("UtcOffsetInSecond", Convert.ToInt32(Request.Cookies["timezoneoffset"]) * 60);
            }


            var userAgent = HttpContext?.Request?.Headers["User-Agent"].FirstOrDefault() ?? "Unknown Device";
            var ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var uaParser = UAParser.Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);
            string browserName = clientInfo.UA.Family; // e.g., Chrome, Firefox, Safari
            string browserVersion = clientInfo.UA.Major; // e.g., 130 for Chrome 130.x
            string osName = clientInfo.OS.Family; // e.g., Windows, Mac OS, Linux
            string deviceName = clientInfo.Device.Family; // e.g., Desktop, iPhone, etc.
            var deviceDetails = string.Empty;
            string deviceLocation = CommonFunctions.GetLocationFromIp(ipAddress);
            if (deviceName.ToLower() != "other")
            {
                deviceDetails = $"IpAddress - {ipAddress}, {deviceName} - {osName} server, {browserName} version {browserVersion}";
            }
            else
            {
                deviceDetails = $"IpAddress - {ipAddress}, {osName} server, {browserName} version {browserVersion}";
            }

            var loginResponse = await _accountService.AdminLogin(new ApiLoginRequest() { Email = loginViewModel.Email, Password = loginViewModel.Password, UserType = UserTypes.SuperAdmin, DeviceToken = HttpContext?.Session.Id ?? "Unknown session", DeviceInfo = deviceDetails.ToString(), DeviceLocation = deviceLocation  });

            if (loginResponse.Data is null || !(new short[] { (short)UserTypes.SuperAdmin, (short)UserTypes.Admin, (short)UserTypes.Editor, (short)UserTypes.Viewer, (short)UserTypes.Management, (short)UserTypes.Accounting }
                                                .Contains((short)loginResponse.Data.UserType)))
            {
                if (loginResponse.Message?.Equals(ResourceString.EmailNotVerified) ?? false)
                {
                    ViewBag.ShowEmailVerificationPopUp = true;
                }
                ViewBag.message = loginResponse.Message;
                return View(loginViewModel);
            }

            //Store value in session
            LoginSessionModel sessionobj = new();
            sessionobj.UserId = loginResponse.Data.UserId;
            sessionobj.FirstName = loginResponse.Data.FirstName;
            sessionobj.LastName = loginResponse.Data.LastName;
            sessionobj.UserTypeId = loginResponse.Data.UserType;
            sessionobj.EmailId = loginResponse.Data.Email;
            sessionobj.DisplayName = loginResponse.Data.DisplayName;
            sessionobj.AccessToken = loginResponse.Data.AccessToken;
            LoginMemberSession.UserDetailSession = sessionobj;


            // get user permisison 

            if (new short[] { (short)UserTypes.Admin, (short)UserTypes.Editor, (short)UserTypes.Viewer, (short)UserTypes.Management, (short)UserTypes.Accounting }
                                                .Contains((short)loginResponse.Data.UserType))
            {
                var permisison = await _manageService.GetUserPermission(loginResponse.Data.UserId);
                _adminPermissionService.SetUserPermissions(permisison.Data, loginResponse.Data.UserId);
            }

            if (retrurnUrl != null)
            {
                return Redirect(retrurnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }


        public async Task<IActionResult> Logout()
        {
            if (LoginMemberSession.UserDetailSession != null)
            {
                await _accountService.LogoutUser(LoginMemberSession.UserDetailSession.UserId, LoginMemberSession.UserDetailSession.AccessToken ?? string.Empty);
            }

            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
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
                if (new short[] { (short)UserTypes.SuperAdmin, (short)UserTypes.Admin, (short)UserTypes.Editor, (short)UserTypes.Viewer, (short)UserTypes.Management, (short)UserTypes.Accounting }
                                                .Contains((short)getUserByEmail.UserType))
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
                    return BadRequest(new ApiResponse<string> { Message = ResourceString.InvalidRequest });
                }
            }
            else
            {
                return NotFound(new ApiResponse<string> { Message = ResourceString.UserNotExist });
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


        public async Task<IActionResult> EmailVerification(string token)
        {
            EmailVerifyRequest request = new();
            request.Token = token;
            var isEmailVerified = await _accountService.VerifyEmail(request);
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
    }
}