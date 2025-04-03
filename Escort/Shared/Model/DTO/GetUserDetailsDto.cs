namespace Shared.Model.DTO
{
    public class GetUserDetailsDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImage { get; set; }
        public string? PhoneNumber { get; set; }
        public int UserType { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
        public int CreditBalance { get; set; }
    }
}
