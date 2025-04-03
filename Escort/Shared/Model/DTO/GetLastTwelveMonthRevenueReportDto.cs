namespace Shared.Model.DTO
{
    public class GetLastTwelveMonthRevenueReportDto
    {
        public int Year { get; set; }
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
