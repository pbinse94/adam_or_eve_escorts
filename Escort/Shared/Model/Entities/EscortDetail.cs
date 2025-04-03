using Shared.Model.Base;
using Shared.Model.DTO;

namespace Shared.Model.Entities
{
    public class EscortDetail : BaseModel
    {
        public int EscortID { get; set; }
        public int UserId { get; set; }
        public int? Age { get; set; }
        public string? Bio { get; set; }
        public string? SexualPreferences { get; set; }
        public int? Height { get; set; }
        public string? BodyType { get; set; }
        public string? BankAccountHolderName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BSBNumber { get; set; }        
        public string? Eyes { get; set; }
        public string? Category { get; set; }
        public string? Language { get; set; }

        
    }
}
