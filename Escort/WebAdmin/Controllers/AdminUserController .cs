using Business.IServices;
using Business.Services;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.Common;
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
    public class AdminUserController : AdminBaseController
    {
        private readonly IManageService _manageService;
        private readonly AdminPermissionService _adminPermissionService;
        public AdminUserController(IManageService manageService, AdminPermissionService adminPermissionService)
        {
            _manageService = manageService;
            _adminPermissionService = adminPermissionService;
        }
        public IActionResult Index()
        {
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Index(UsersRequestModel model)
        {
            var items = await _manageService.AdminUserList(model);
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
        public async Task<IActionResult> AddUsers(int? id)
        {
            var model = new AdminUserRequestModel();

            if (id > 0)
            {
                var userDetails = (await _manageService.GetUserDetails(id.Value)).Data;
                if (userDetails?.UserId <= 0)
                {
                    return RedirectToAction("Index");
                }

                model.Id = userDetails.UserId;
                model.FirstName = userDetails.FirstName;
                model.LastName = userDetails.LastName;
                model.Email = userDetails.Email;
                model.UserType = userDetails.UserType;
                model.DisplayName = userDetails.DisplayName;
                model.IsActive = userDetails.IsActive ?? false;
            }
            var userPermissions = await _manageService.GetUserPermission(id ?? 0);
            model.ModulePermissions = userPermissions.Data ?? new List<ModulePermissionModel>();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateUser(AdminUserRequestModel requestModel)
        {
            var res = await _manageService.AddUpdateAdminUser(requestModel, UserId);

            await Task.Factory.StartNew(async () =>
            {
                if (res.Data && requestModel.Id > 0)
                { // set permission after updated
                    var permisison = await _manageService.GetUserPermission(requestModel.Id);
                    _adminPermissionService.SetUserPermissions(permisison.Data, requestModel.Id);
                }
            });

            return Json(res);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string Email, int Id)
        {
            var responses = await _manageService.IsUserEmailInUse(Email, Id);
            if (responses.Data)
            {
                return Json(responses.Message);
            }
            else
                return Json(true);
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

        public async Task<IActionResult> LoginHistory(int? id)
        {
            var adminUsers = await _manageService.AdminUserList(new UsersRequestModel {  Start = 0, Length = int.MaxValue });
            ViewBag.AdminUserSelectList = adminUsers.Data?.Select(x => new SelectListItem() { Text = $"{x.Name} ({x.Role})", Value = x.Id.ToString() }).ToList();
            return View(new LoginHistoryRequestModel { UserId = id ?? 0});
        }

        [HttpPost]
        public async Task<IActionResult> LoginHistory(LoginHistoryRequestModel model)
        {
            var items = await _manageService.UserLoginHistory(model);
            var loginHistory = items.Data;
            var result = new DataTableResult<LoginHistoryDto>
            {
                Draw = model.Draw,
                Data = loginHistory,
                RecordsFiltered = loginHistory?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = loginHistory?.Count() ?? 0

            };
            return Json(result);
        }

        public async Task<IActionResult> ActivityLogHistory(int? id)
        {
            var adminUsers = await _manageService.AdminUserList(new UsersRequestModel { Start = 0, Length = int.MaxValue });
            ViewBag.AdminUserSelectList = adminUsers.Data?.Select(x => new SelectListItem() { Text = $"{x.Name} ({x.Role})", Value = x.Id.ToString() }).ToList();
            return View(new ActivityLogHistoryRequestModel { AdminUserId = id ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> ActivityLogHistory(ActivityLogHistoryRequestModel model)
        {
            var items = await _manageService.GetAdminUserActivities(model);
            var activityLogHistory = items.Data;
            var result = new DataTableResult<ActivityLogDto>
            {
                Draw = model.Draw,
                Data = activityLogHistory,
                RecordsFiltered = activityLogHistory?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = activityLogHistory?.Count() ?? 0

            };
            return Json(result);
        }
    }
}
