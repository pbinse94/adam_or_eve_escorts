namespace Shared.Model.Request.WebUser
{
    public class SendGiftRequest
    {
        public int EscortId { get; set; }
        public int ClientId { get; set; }
        public int Tokens { get; set; }
        public int GiftIconID { get; set; }
        public short TransactionType { get; set; }
    }
}
