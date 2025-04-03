using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserTokenTransactionDto>>> UserList(UserTokenTransactionRequestModel request);
        Task<ApiResponse<List<UserTokenTransactionDto>>> ListByName (UserTokenTransactionRequestModel request);
        Task<int> SendGift(SendGiftRequest request);

        Task<int> CheckAndInsertIPAddress(string ipAddress);
        Task<ApiResponse<List<PaymentReportDto>>> PaymentReportList(PaymentReportRequestModel request);
        Task<ApiResponse<List<PaymentReportDto>>> ExportPaymentReportList(PaymentReportRequestModel request);
        Task<ApiResponse<int>> MarkPaymentDone(string usersId);
        Task<ApiResponse<List<EscortPaymentAmountFromClientDto>>> EscortPaymentReportList(EscortPaymentReportRequestModel request);
        Task<ApiResponse<GetClientBalanceReportDto>> GetClientBalanceReport(UserTokenTransactionRequestModel request, int loginUserId);
        Task<ApiResponse<List<UserTokenTransactionClientDto>>> GetClientBalanceReportAdmin(UserTokenTransactionRequestModel request);
    }
}
