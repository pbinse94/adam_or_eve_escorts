using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Escort;
using Shared.Common;
using Shared.Model.Request.Admin;
using Business.Services;
using Shared.Model.Base;
using Shared.Resources;
using WebAdmin.Controllers.Base;
using Shared.Common.Enums;
using System.Security;
using Shared.Model.Request.AdminUser;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Shared.Model.DTO;

namespace WebAdmin.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class EscortController : AdminBaseController
    {
        private readonly IEscortServices _escortServices;
        private readonly IAccountService _accountService;
        private readonly AdminPermissionService _adminPermissionService;
        private readonly PermissionsCache _permissions;
        private readonly IProfileService _profileService;
        private int _userRole;
        public EscortController(AdminPermissionService adminPermissionService, IEscortServices escortServices, IAccountService accountService, IProfileService profileService)
        {
            _escortServices = escortServices;
            _adminPermissionService = adminPermissionService;
            _permissions = _adminPermissionService.GetUserPermissions(UserId);
            _userRole = LoginMemberSession.UserDetailSession?.UserTypeId ?? (int)UserTypes.SuperAdmin;
            _accountService = accountService;
            _profileService = profileService;
        }
        public IActionResult Index()
        {
            // CHeck user permission for view
            bool canView = _adminPermissionService.CanView(Constants.Escorts, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }
            bool canUpdate = _adminPermissionService.CanEdit(Constants.Escorts, _permissions);
            ViewBag.CanUpdate = (_userRole == (int)UserTypes.SuperAdmin || canUpdate);


            EscortModel escortlst = new EscortModel();
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;

            var countryCodes = _accountService.GetCountryCodes();
            ViewBag.SelectListItems = countryCodes.Result.Select(x => new SelectListItem() { Text = x.CountryName, Value = x.CountryName }).ToList();

            return View(escortlst);
        }

        [HttpPost]
        public async Task<IActionResult> EscortList(UsersRequestModel model)
        {
            var items = await _escortServices.EscortList(model);
            var escortlist = items.Data;
            var result = new DataTableResult<EscortModel>
            {
                Draw = model.Draw,
                Data = escortlist,
                RecordsFiltered = escortlist?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = escortlist?.Count() ?? 0

            };
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEscortStatus(int userId, bool activeStatus, bool deleteStatus, bool isActiveStatusChange)
        {
            var changeEscortStatus = await _escortServices.ChangeEscortStatus(userId, activeStatus, deleteStatus);
            if (changeEscortStatus > 0)
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.EscortActivated });
                    }
                    else
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.EscortDeactivate });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.EscortDeleteSuccess });
                    }
                    else
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.RecoverEscort });
                    }
                }
            }
            else
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortActivateFailed });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortDeactivateFailed });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortDeleteFailed });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortAccountRecoverFailed });
                    }
                }
            }

        }

        //[Route("ApproveAccount")]
        [HttpPost]
        public async Task<IActionResult> ApproveAccount(int userId, bool isApprove)
        {
            var updateUserProfile = await _accountService.ApproveEscort(userId, isApprove, UserId);
            if (updateUserProfile.Data)
            {
                return StatusCode(StatusCodes.Status200OK, updateUserProfile);
            }

            return StatusCode(StatusCodes.Status404NotFound, new ApiResponse<bool>(false, message: ResourceString.SomethingWrong, apiName: "ApproveAccount"));
        }

        
        [HttpGet]
        public async Task<ActionResult> Profile(int id)
        {

            var response = await _profileService.GetEscortFullProfileDetails(id);

            //var escortDetail = new EscortDetailDto();
            //var escortRule = new EscortRules(null, null);


            //if (response.Data != null)
            //{
            //    escortDetail = response.Data.Item1;
            //    escortDetail.EscortGallery = escortDetail.EscortGallery.OrderBy(item => item.MediaType != 1)
            //                 .ToList();
            //    escortRule = response.Data.Item2;
            //}

            return View(response.Data);
        }
    }
}
