using Shared.Model.DTO;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.IRepository
{
    public interface IUserRepository
    {
        Task<List<UserTokenTransactionDto>> UserList(UserTokenTransactionRequestModel request);
        Task<List<UserTokenTransactionDto>> ListByName(UserTokenTransactionRequestModel request);
        Task<int> SendGift(SendGiftRequest request);
        Task<int> CheckAndInsertIPAddress(string request);
        Task<List<PaymentReportDto>> PaymentReportList(PaymentReportRequestModel request);
        Task<int> MarkPaymentDone(string usersid);
        Task<List<EscortPaymentAmountFromClientDto>> EscortPaymentReportList(EscortPaymentReportRequestModel request);
        Task<GetClientBalanceReportDto> GetClientBalanceReport(UserTokenTransactionRequestModel request, int loginUserId);
        Task<List<UserTokenTransactionClientDto>> GetClientBalanceReportAdmin(UserTokenTransactionRequestModel request);
    }
}
