using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Escort;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;
using Shared.Resources;
using WebAdmin.Controllers.Base;

namespace WebAdmin.Controllers
{
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserController : AdminBaseController
    {
        private readonly IManageService _manageService;
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IFileStorageService _fileStorageService;
       
        private readonly AdminPermissionService _adminPermissionService;
        private readonly PermissionsCache _permissions;
        private int _userRole;
        public UserController(AdminPermissionService adminPermissionService, IManageService manageService, IAccountService accountService, IUserService userService, IFileStorageService fileStorageService)
        {
            _manageService = manageService;
            _adminPermissionService = adminPermissionService;
            _permissions = _adminPermissionService.GetUserPermissions(UserId);
            _userRole = LoginMemberSession.UserDetailSession?.UserTypeId ?? (int)UserTypes.SuperAdmin;
            _accountService = accountService;
            _userService = userService;
            _fileStorageService = fileStorageService;
        }
        public IActionResult Index()
        {
            // CHeck user permission for view
            bool canView = _adminPermissionService.CanView(Constants.Clients, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }
            bool canUpdate = _adminPermissionService.CanEdit(Constants.Clients, _permissions);
            ViewBag.CanUpdate = (_userRole == (int)UserTypes.SuperAdmin || canUpdate);


            UsersDto userlst = new UsersDto();
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            var countryCodes = _accountService.GetCountryCodes();
            ViewBag.SelectListItems = countryCodes.Result.Select(x => new SelectListItem() { Text = x.CountryName, Value = x.CountryName }).ToList();
            return View(userlst);
        }

        [HttpPost]
        public async Task<JsonResult> Index(UsersRequestModel model)
       {
            var items = await _manageService.UserList(model);
            var users = items.Data;
            var result = new DataTableResult<UsersDto>
            {
                Draw = model.Draw,
                Data = users,
                RecordsFiltered = users?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = users?.Count() ?? 0
            };
            return Json(result);
        }





        [HttpGet]
        public async Task<ActionResult> ProfileDetail(int? id)
        { 
            if (id > 0)
            {
                ViewBag.ClientId = id;
                var userDetails = await _manageService.GetUserDetails(id.Value);
                if (userDetails.Data?.UserId > 0 && userDetails.Data?.UserType == (int)UserTypes.Client)
                    return View(userDetails.Data);
            }
            return RedirectToAction("Index", "User");
        }

         

        [HttpPost]
        public async Task<IActionResult> ClientBalanceList(UserTokenTransactionRequestModel model)
        {

            var items = await _userService.GetClientBalanceReportAdmin(model);
            var clientBalanceList = items.Data;
            var result = new DataTableResult<UserTokenTransactionClientDto>
            {
                Draw = model.Draw,
                Data = clientBalanceList,
                RecordsFiltered = clientBalanceList?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = clientBalanceList?.Count() ?? 0

            };
            return Json(result);
        }




        [HttpGet]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id > 0)
            {
                var userDetails = await _manageService.GetUserDetails(id.Value);
                if (userDetails.Data?.UserId > 0 && userDetails.Data?.UserType == (int)UserTypes.Client)
                    return View(userDetails.Data);
            }
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Detail(UserDetailsDto requestmodel)
        {
            var updateUser = await _manageService.UpdateUserDetail(requestmodel);
            return Json(updateUser);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ChangePasswordModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var changeAdminPassword = await _manageService.ChangePassword(model, UserId);

            return changeAdminPassword switch
            {
                ResponseTypes.Success => Ok(new ApiResponse<bool> { Message = ResourceString.PasswordUpdated }),
                ResponseTypes.OldPasswordWrong => BadRequest(new ApiResponse<bool> { Message = ResourceString.WrongOldPassword }),
                _ => BadRequest(new ApiResponse<bool> { Message = ResourceString.FailedToSetNewPassword }),
            };
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(int userId, bool activeStatus, bool deleteStatus, bool isActiveStatusChange)
        {
            var changeUserStatus = await _manageService.ChangeUserStatus(userId, activeStatus, deleteStatus);
            if (changeUserStatus > 0)
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.UserActivated });
                    }
                    else
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.UserDeactivate });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.UserDeleteSuccess });
                    }
                    else
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.RecoverUser });
                    }
                }
            }
            else
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.UserActivateFailed });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.UserDeactivateFailed });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.UserDeleteFailed });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.UserAccountRecoverFailed });
                    }
                }
            }
        }
    }
}
