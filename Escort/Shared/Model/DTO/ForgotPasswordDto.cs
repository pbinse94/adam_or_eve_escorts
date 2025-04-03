namespace Shared.Model.DTO
{
    public class ForgotPasswordDto
    {
        public long UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ForgotPasswordToken { get; set; } = string.Empty;
        public short IsValid { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
