using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO;
using Shared.Model.Escort;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;
using Web.Controllers.Base;

namespace Web.Controllers
{
    [Route("escort")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class EscortController : UserBaseController
    {
        private readonly IEscortServices _escortService;

        public EscortController(IEscortServices escortService)
        {
            _escortService = escortService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardStatistics = await _escortService.GetEscortDashboardStatistics(UserId, SiteKeys.AdminPercentage);
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            return View(dashboardStatistics);
        }

        [Route("GetEscortLiveCamGiftSummery")]
        [HttpPost]
        public async Task<PartialViewResult> GetEscortLiveCamGiftSummery(EscortLiveCamGiftSummeryRequest model)
        {
            model.EscortUserId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            var items = await _escortService.EscortLiveCamGiftSummery(model, SiteKeys.AdminPercentage);
            var escortlist = items.Data;
            var result = new DataTableResult<EscortLiveCamGiftSummeryDto>
            {
                Draw = 1,
                Data = escortlist,
                RecordsFiltered = escortlist?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = escortlist?.Count() ?? 0
            };
            return PartialView("_GiftSummary", escortlist);
        }
    }
}
