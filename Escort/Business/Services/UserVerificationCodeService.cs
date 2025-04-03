using Business.Communication;
using Business.IServices;
using Data.IRepository;
using Microsoft.AspNetCore.Http;
using Shared.Model.Request.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserVerificationCodeService : IUserVerificationCodeService
    {
        private readonly IUserVerificationCodeRepository _userVerificationCodeRepository;

        public UserVerificationCodeService(IUserVerificationCodeRepository userVerificationCodeRepository)
        {
            _userVerificationCodeRepository = userVerificationCodeRepository;
        }
        public async Task<UserVerificationCode> GetByEmail(string email)
        {
            return await _userVerificationCodeRepository.FindByEmailAsync(email);
        }
        public async Task<int> AddUpdate(UserVerificationCode verificationCode)
        {
            return await _userVerificationCodeRepository.AddUpdateAsync(verificationCode);
        }
     
    }
}
