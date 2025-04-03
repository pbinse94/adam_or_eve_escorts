namespace Shared.Model.DTO
{
    public class EscortPaymentAmountFromClientDto
    {
        public int Id { get; set; } = 0;
        public DateTime SpentDate { get; set; }
        public string SpentDateOn { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
