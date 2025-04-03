using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO;
using Shared.Model.Request.AdminUser;
using WebAdmin.Controllers.Base;

namespace WebAdmin.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class DashboardController : AdminBaseController
    {
        private readonly IEscortServices _escortService;
        private readonly AdminPermissionService _adminPermissionService;
        private readonly PermissionsCache _permissions;
        private int _userRole;
        public DashboardController(IEscortServices escortService, AdminPermissionService adminPermissionService)
        {
            _escortService = escortService;
            _adminPermissionService = adminPermissionService;
            _permissions = _adminPermissionService.GetUserPermissions(UserId);
            _userRole = LoginMemberSession.UserDetailSession?.UserTypeId ?? (int)UserTypes.SuperAdmin;
        }
        public async Task<IActionResult> Index()
        {
            bool canViewRevenueReport = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            bool canViewEscorts = _adminPermissionService.CanView(Constants.Escorts, _permissions);
            bool canViewClients = _adminPermissionService.CanView(Constants.Clients, _permissions);
            bool canViewEstablishment = _adminPermissionService.CanView(Constants.Establishments, _permissions);
            var dashboardStatistics = await _escortService.GetAdminDashboardStatistics(SiteKeys.AdminPercentage);
            return View(new Tuple<AdminDashboardStatisticsDto,bool,bool,bool,bool>(dashboardStatistics, (canViewRevenueReport || _userRole == (int)UserTypes.SuperAdmin),
                (canViewEscorts || _userRole == (int)UserTypes.SuperAdmin),
                (canViewClients || _userRole == (int)UserTypes.SuperAdmin),
                (canViewEstablishment || _userRole == (int)UserTypes.SuperAdmin)

                ));
        }

        public async Task<JsonResult> GetGiftsChart()
        {
            var giftChart = await _escortService.GetLastTwelveMonthGiftTokens();
            return Json(giftChart);
        }

        public async Task<JsonResult> GetSubscriptionChart()
        {
            var subscriptionChart = await _escortService.GetLastTwelveMonthSubscriptionReport();
            return Json(subscriptionChart);
        }

        public async Task<JsonResult> GetRevenueChart()
        {
            var revenueChart = await _escortService.GetLastTwelveMonthRevenueReport(SiteKeys.AdminPercentage);
            return Json(revenueChart);
        }

        public async Task<PartialViewResult> GetCountryWiseEscorts()
        {
            var countryWiseEscorts = await _escortService.GetCountryWiseEscortCount();
            return PartialView("_CountryWiseEscorts", countryWiseEscorts);
        }
    }
}
