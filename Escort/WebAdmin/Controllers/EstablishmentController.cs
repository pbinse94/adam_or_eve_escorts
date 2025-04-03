using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Escort;
using Shared.Model.Establishment;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;
using Shared.Resources;
using WebAdmin.Controllers.Base;

namespace WebAdmin.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class EstablishmentController : AdminBaseController
    {
        private readonly IEstablishmentService _establishmentServices;
        private readonly IAccountService _accountService;
        private readonly AdminPermissionService _adminPermissionService;
        private readonly PermissionsCache _permissions;
        private readonly IProfileService _profileService;
        private readonly IEscortServices _escortServices;
        private int _userRole;
        public EstablishmentController(AdminPermissionService adminPermissionService, IEstablishmentService establishmentServices, IAccountService accountService, IProfileService profileService, IEscortServices escortServices)
        {
            _establishmentServices = establishmentServices;
            _adminPermissionService = adminPermissionService;
            _permissions = _adminPermissionService.GetUserPermissions(UserId);
            _userRole = LoginMemberSession.UserDetailSession?.UserTypeId ?? (int)UserTypes.SuperAdmin;
            _accountService = accountService;
            _profileService = profileService;
            _escortServices = escortServices;
        }
        public IActionResult Index()
        {
            // CHeck user permission for view
            bool canView = _adminPermissionService.CanView(Constants.Establishments, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }
            bool canUpdate = _adminPermissionService.CanEdit(Constants.Establishments, _permissions);
            ViewBag.CanUpdate = (_userRole == (int)UserTypes.SuperAdmin || canUpdate);

            EstablishmentModel establishmentlst = new EstablishmentModel();
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;

            var countryCodes = _accountService.GetCountryCodes();
            ViewBag.SelectListItems = countryCodes.Result.Select(x => new SelectListItem() { Text = x.CountryName, Value = x.CountryName }).ToList();

            return View(establishmentlst);
        }

        [HttpPost]
        public async Task<IActionResult> EstablishmentList(UsersRequestModel model)
        {
            var items = await _establishmentServices.EstablishmentList(model);
            var establishmentlist = items.Data;
            var result = new DataTableResult<EstablishmentModel>
            {
                Draw = model.Draw,
                Data = establishmentlist,
                RecordsFiltered = establishmentlist?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = establishmentlist?.Count() ?? 0

            };
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEstablishmentStatus(int userId, bool activeStatus)
        {
            var changeEstablishmentStatus = await _establishmentServices.ChangeEstablishmentStatus(userId, activeStatus);
            if (changeEstablishmentStatus > 0)
            {
                if (activeStatus)
                {
                    return Ok(new ApiResponse<bool> { Message = ResourceString.EstablishmentActivated });
                }
                else
                {
                    return Ok(new ApiResponse<bool> { Message = ResourceString.EstablishmentDeActivated });
                }
            }
            else
            {
                if (activeStatus)
                {
                    return BadRequest(new ApiResponse<bool> { Message = ResourceString.EstablishmentActivationFailed });
                }
                else
                {
                    return BadRequest(new ApiResponse<bool> { Message = ResourceString.EstablishmentDeActivationFailed });
                }
            }

        }


      
        [HttpGet]
        public async Task<ActionResult> ProfileDetail(int id)
        {


            // CHeck user permission for view
            bool canView = _adminPermissionService.CanView(Constants.Establishments, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }
            bool canUpdate = _adminPermissionService.CanEdit(Constants.Establishments, _permissions);
            ViewBag.CanUpdate = (_userRole == (int)UserTypes.SuperAdmin || canUpdate);
            ViewBag.EstablishmentId = id;
            var response = await _establishmentServices.GetEstablishmentById(id);
            var escortDetail = new EscortDetailDto();

            if (response.Data != null)
            {
                escortDetail = response.Data;
            }
            return View(escortDetail);
        }


        [HttpPost]
        public async Task<IActionResult> EscortList(UsersRequestModel model)
        {
            model.EstablishmentId = model.EstablishmentId;
            model.UserType = (int)UserTypes.EstablishmentEscort;
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

    }
}
