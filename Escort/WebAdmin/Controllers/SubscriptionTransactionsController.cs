using Business.IServices;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Model.DTO;
using Shared.Model.Request.AdminUser;
using WebAdmin.Controllers.Base;

namespace WebAdmin.Controllers
{
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class SubscriptionTransactionsController : AdminBaseController
    {
        private readonly ISubscriptionTransactionsService _subscriptionTransactionsService;
        public SubscriptionTransactionsController(ISubscriptionTransactionsService subscriptionTransactionsService)
        {
            _subscriptionTransactionsService = subscriptionTransactionsService;
        }

        public IActionResult Index()
        {
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSubscriptionTransactions(SubscriptionTransactionsRequestModel model)
        {
            var items = await _subscriptionTransactionsService.GetSubscriptionTransactions(model);
            var subscriptionTransactionsHistory = items.Data;
            var result = new DataTableResult<SubscriptionTransactionsDto>
            {
                Draw = model.Draw,
                Data = subscriptionTransactionsHistory,
                RecordsFiltered = subscriptionTransactionsHistory?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = subscriptionTransactionsHistory?.Count() ?? 0

            };
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ExportSubscriptionTransactionsReport(SubscriptionTransactionsRequestModel request)
        {
            request.Start = 0;
            request.Length = int.MaxValue;
            var items = await _subscriptionTransactionsService.GetSubscriptionTransactions(request);
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transactions Report List");

            var currentRow = 1;
            var headerRange = worksheet.Range(currentRow, 1, currentRow, 35);

            worksheet.Cell(currentRow, 1).Value = "S.No";
            worksheet.Cell(currentRow, 2).Value = "Name";
            worksheet.Cell(currentRow, 3).Value = "Plan Name";
            worksheet.Cell(currentRow, 4).Value = "Plan Duration";
            worksheet.Cell(currentRow, 5).Value = "Transaction Date";
            worksheet.Cell(currentRow, 6).Value = "Subscription Id";
            worksheet.Cell(currentRow, 7).Value = "Transaction Id";
            worksheet.Cell(currentRow, 8).Value = "Transaction Status";
            worksheet.Cell(currentRow, 9).Value = "Status";
            worksheet.Cell(currentRow, 10).Value = "Next Billing Date";
            worksheet.Cell(currentRow, 11).Value = "Paypal Fee($)";
            worksheet.Cell(currentRow, 12).Value = "Subscription Amount($)";
            worksheet.Columns().AdjustToContents();

            if (items.Data != null)
            {
                foreach (var item in items.Data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = currentRow - 1;
                    worksheet.Cell(currentRow, 2).Value = item.UserName;
                    worksheet.Cell(currentRow, 3).Value = item.PlanName;
                    worksheet.Cell(currentRow, 4).Value = item.PlanDuration;
                    worksheet.Cell(currentRow, 5).Value = item.TransactionDateString;
                    worksheet.Cell(currentRow, 6).Value = item.PaymentSubscriptionId;
                    worksheet.Cell(currentRow, 7).Value = item.TransactionId;
                    worksheet.Cell(currentRow, 8).Value = item.TransactionStatus;
                    worksheet.Cell(currentRow, 9).Value = item.RenewalStatus;
                    worksheet.Cell(currentRow, 10).Value = item.NextBillingTimeString;
                    worksheet.Cell(currentRow, 11).Value = item.TransactionFee;
                    worksheet.Cell(currentRow, 12).Value = item.TransactionAmount;
                }
            }
           
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TransactionReportList.xlsx");
        }
    }
}
