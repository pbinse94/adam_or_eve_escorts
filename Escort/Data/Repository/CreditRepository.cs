using Dapper;
using Data.IFactory;
using Data.IRepository;
using Newtonsoft.Json.Linq;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Subscription;
using System.Data;

namespace Data.Repository
{
    public class CreditRepository : CurdRepository<CreditPlan>, ICreditRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public CreditRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<decimal> CalculateCreditPrice(int creditQuantity)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new { CreditQuantity = creditQuantity };
            return await connection.QueryFirstOrDefaultAsync<decimal>("CalculateCreditPrice", parameters, commandType: CommandType.StoredProcedure);
        }


        public async Task<int> SaveUserCredit(int creditQuantity, string token, int userId, decimal paidAmount)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new { 
                CreditQuantity = creditQuantity, 
                Status= (int)TransactionPaymentStatus.Pending,
                TransactionId = token,
                UserId = userId,
                PaidAmount = paidAmount
            };
            return await connection.QueryFirstOrDefaultAsync<int>("SaveUserCreditDetails", parameters, commandType: CommandType.StoredProcedure);
        }


        public async Task<int> UpdateCreditPaymentStatus(string transactionId, string paymentStatus, int userId) 
        {
            using var connection = _dbConnection.CreateDBConnection();
            if (string.IsNullOrEmpty(paymentStatus))
            {
                paymentStatus = "failed";
            }

            var parameters = new
            {
                Status = paymentStatus.Equals("completed", StringComparison.CurrentCultureIgnoreCase) ? (int)TransactionPaymentStatus.Success : (int)TransactionPaymentStatus.Cancelled,
                TransactionId = transactionId,
                UserId = userId,
                IsPaymentSuccess = paymentStatus.Equals("completed", StringComparison.CurrentCultureIgnoreCase)
            };
            return await connection.QueryFirstOrDefaultAsync<int>("UpdateCreditPaymentStatus", parameters, commandType: CommandType.StoredProcedure);

        }

    }
}
