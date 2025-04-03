namespace Shared.Model.DTO
{
    public class LoginDto
    {
        public long UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public short UserType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
