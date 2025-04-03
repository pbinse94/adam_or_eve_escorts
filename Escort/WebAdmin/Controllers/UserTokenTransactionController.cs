using Azure.Core;
using Business.IServices;
using Business.Services;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;
using Shared.Resources;
using System.Data;
using System.Net;
using WebAdmin.Controllers.Base;

namespace WebAdmin.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserTokenTransactionController : AdminBaseController
    {
        private readonly IUserService _userTokenTransactionServices;
        private readonly AdminPermissionService _adminPermissionService;
        private readonly PermissionsCache _permissions;
        private int _userRole;
        public UserTokenTransactionController(AdminPermissionService adminPermissionService, IUserService userTokenTransactionServices)
        {
            _userTokenTransactionServices = userTokenTransactionServices;
            _adminPermissionService = adminPermissionService;
            _permissions = _adminPermissionService.GetUserPermissions(UserId);
            _userRole = LoginMemberSession.UserDetailSession?.UserTypeId ?? (int)UserTypes.SuperAdmin;
        }

        public IActionResult Index()
        {
            // Check user permission for view
            bool canView = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }

            UserTokenTransactionDto userlst = new UserTokenTransactionDto();
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            return View(userlst);
        }

        [HttpPost]
        public async Task<JsonResult> Index(UserTokenTransactionRequestModel model)
        {
            var items = await _userTokenTransactionServices.UserList(model);
            var users = items.Data;
            var result = new DataTableResult<UserTokenTransactionDto>
            {
                Draw = model.Draw,
                Data = users,
                RecordsFiltered = users?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = users?.Count() ?? 0
            };
            return Json(result);
        }


        public async Task<IActionResult> ExportSpendingReport()
        {
            // Check user permission for view
            bool canView = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }

            UserTokenTransactionRequestModel userTokenTransactionRequestModel = new UserTokenTransactionRequestModel();
            userTokenTransactionRequestModel.Start = 0;
            userTokenTransactionRequestModel.Length = int.MaxValue;
            var items = await _userTokenTransactionServices.UserList(userTokenTransactionRequestModel);
            var columnMapping = new Dictionary<string, string>
            {
                { "Name", "Name" },
                { "PointsPurchased", "Points Purchased" },
                { "PointsSpent", "Points Spent" },
                { "PointsBalance", "Points Balance" }
            };

            return CommonFunctions.GenerateExcelReport(items.Data, columnMapping, "Report", "ReportList.xlsx", null);
        }

        //[Route("Detail/{userId}/{filterBy?}")]        
        public IActionResult GetListByName(int userId, int filterBy = 1)
        {
            // Check user permission for view
            bool canView = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }

            UserTokenTransactionDto userlst = new UserTokenTransactionDto();
            userlst.CurrentId = userId;
            userlst.FilterBy = filterBy;
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            return View(userlst);
        }

        [HttpPost]
        public async Task<JsonResult> GetListByName(UserTokenTransactionRequestModel model)
        {
            var items = await _userTokenTransactionServices.ListByName(model);
            var users = items.Data;
            var result = new DataTableResult<UserTokenTransactionDto>
            {
                Draw = model.Draw,
                Data = users,
                RecordsFiltered = users?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = users?.Count() ?? 0
            };
            return Json(result);
        }

        public async Task<IActionResult> ExportClientBalanceReport(UserTokenTransactionRequestModel userTokenTransactionRequestModel)
        {
            bool canView = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }

            userTokenTransactionRequestModel.Start = 0;
            userTokenTransactionRequestModel.Length = int.MaxValue;

            var items = await _userTokenTransactionServices.GetClientBalanceReport(userTokenTransactionRequestModel, UserId);

            if(items == null || items.Data == null)
            {
                return new EmptyResult();
            }
            
            var dateColumns = new List<string> { "Date" };
            return CommonFunctions.GenerateClientBalanceExcelReport(items.Data, "Report", "ReportList.xlsx", dateColumns);
        }


        public async Task<IActionResult> ExportUserTransactionReport(UserTokenTransactionRequestModel userTokenTransactionRequestModel)
        {
            // Check user permission for view
            bool canView = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }

            userTokenTransactionRequestModel.Start = 0;
            userTokenTransactionRequestModel.Length = int.MaxValue;

            var items = await _userTokenTransactionServices.ListByName(userTokenTransactionRequestModel);
            var columnMapping = new Dictionary<string, string>();

            if (userTokenTransactionRequestModel.FilterBy == 1)
            {
                columnMapping.Add("Date", "Date");
                columnMapping.Add("PurchaseId", "Purchase ID");
                columnMapping.Add("PointsPurchased", "Points Purchased");
                columnMapping.Add("Amount", "Amount");
            }
            else if (userTokenTransactionRequestModel.FilterBy == 2)
            {
                columnMapping.Add("Date", "Date");
                columnMapping.Add("EscortName", "Escort Name");
                columnMapping.Add("PointsSpent", "Points Spent");
                columnMapping.Add("Description", "Description");
            }
            else
            {
                columnMapping.Add("Date", "Date");
                columnMapping.Add("PointsPurchased", "Points Purchased");
                columnMapping.Add("PointsSpent", "Points Spent");
                columnMapping.Add("PointsBalance", "Points Balance");
            }

            var dateColumns = new List<string> { "Date" };

            return CommonFunctions.GenerateExcelReport(items.Data, columnMapping, "Report", "ReportList.xlsx", dateColumns);
        }


        [HttpGet]
        public IActionResult PaymentReport(string? fromDate = "", string? toDate = "", int paidFilter = 1)
        {
            // Check user permission for view
            bool canView = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }

            PaymentReportDto userlst = new PaymentReportDto();
            userlst.PaidFilter = paidFilter;
            userlst.ToDate = toDate;
            userlst.FromDate = fromDate;
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            return View(userlst);
        }


        [HttpPost]
        public async Task<JsonResult> PaymentReport(PaymentReportRequestModel model)
        {
            var items = await _userTokenTransactionServices.PaymentReportList(model);
            var users = items.Data;
            var result = new DataTableResult<PaymentReportDto>
            {
                Draw = model.Draw,
                Data = users,
                RecordsFiltered = users?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = users?.Count() ?? 0

            };
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> EscortPaymentReport(EscortPaymentReportRequestModel model)
        {
            var items = await _userTokenTransactionServices.EscortPaymentReportList(model);
            var users = items.Data;
            var result = new DataTableResult<EscortPaymentAmountFromClientDto>
            {
                Draw = model.Draw,
                Data = users,
                RecordsFiltered = users?.Count() ?? 0,
                RecordsTotal = users?.Count() ?? 0

            };
            ViewBag.EscortName = model.EscortName;
            return View(items.Data);
        }


        //[HttpPost]
        //public async Task<JsonResult> EscortPaymentReport(DataTableParameters model)
        //{
        //    var items = await _userTokenTransactionServices.EscortPaymentReportList(model);
        //    var users = items.Data;
        //    var result = new DataTableResult<EscortPaymentAmountFromClientDto>
        //    {
        //        Draw = model.Draw,
        //        Data = users,
        //        RecordsFiltered = users?.Count() ?? 0,
        //        RecordsTotal = users?.Count() ?? 0

        //    };
        //    return View(result);
        //}





        public async Task<IActionResult> ExportPaymentReport(PaymentReportRequestModel paymentReportRequestModel)
        {
            // Check user permission for view
            bool canView = _adminPermissionService.CanView(Constants.RevenueReport, _permissions);
            if (!canView && _userRole != (int)UserTypes.SuperAdmin) { return RedirectToAction("Index", "Dashboard", new { area = "" }); }

            paymentReportRequestModel.Start = 0;
            paymentReportRequestModel.Length = int.MaxValue;

            var items = await _userTokenTransactionServices.ExportPaymentReportList(paymentReportRequestModel);
            var columnMapping = new Dictionary<string, string>();

            if (paymentReportRequestModel.PaidFilter == 1)
            {
                columnMapping.Add("EscortName", "Escort Name");
                columnMapping.Add("Payment", "Payment");
                columnMapping.Add("BankAccountHolderName", "Account Holder's Name");
                columnMapping.Add("BankName", "Bank Name");
                columnMapping.Add("BsbNumber", "BSB Code");
                columnMapping.Add("BankAccountNumber", "Account Number");
            }
            else
            {
                columnMapping.Add("IsPaymentDone", "Is Payment Done");
                columnMapping.Add("EscortName", "Escort Name");
                columnMapping.Add("Payment", "Payment");
                columnMapping.Add("BankAccountHolderName", "Account Holder's Name");
                columnMapping.Add("BankName", "Bank Name");
                columnMapping.Add("BsbNumber", "BSB Code");
                columnMapping.Add("BankAccountNumber", "Account Number");
            }
            var boolColumns = new List<string> { "IsPaymentDone" };


            return CommonFunctions.GenerateExcelReport(items.Data, columnMapping, "Payment Report", "PaymentReport.xlsx", null, boolColumns);
        }


        [HttpPost]
        public async Task<IActionResult> MarkPaymentDone(string userIds)
        {
            var items = await _userTokenTransactionServices.MarkPaymentDone(userIds);
            if (items.Data > 0)
            {
                return Json(new ApiResponse<bool> { Data = true, Message = ResourceString.PaymentStatusUpdated });
            }
            else
            {
                return Json(new ApiResponse<bool> { Data = false, Message = ResourceString.PaymentStatusFailed });
            }
        }
    }
}