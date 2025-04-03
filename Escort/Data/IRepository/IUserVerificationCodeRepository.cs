using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.AdminUser;

namespace Data.IRepository
{
    public interface IUserVerificationCodeRepository : ICurdRepository<UserVerificationCode>
    {
        Task<UserVerificationCode> FindByEmailAsync(string email);
    }
}
