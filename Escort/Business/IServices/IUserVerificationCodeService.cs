using Shared.Model.Request.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.IServices
{
    public interface IUserVerificationCodeService
    {
        Task<UserVerificationCode> GetByEmail(string email);
        Task<int> AddUpdate(UserVerificationCode verificationCode);
     
    }
}
