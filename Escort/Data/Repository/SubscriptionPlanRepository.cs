using Dapper;
using Data.IFactory;
using Data.IRepository;
using DocumentFormat.OpenXml.VariantTypes;
using Shared.Common.Enums;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Subscription;
using System.Data;
using System.Transactions;

namespace Data.Repository
{
    public class SubscriptionPlanRepository : CurdRepository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public SubscriptionPlanRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetail(int userId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @UserId = userId
            };
            return await connection.QueryFirstOrDefaultAsync<GetUserSubscriptionDetailDto>("GetUserSubscriptionDetail", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetailById(int userSubscriptionId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @Id = userSubscriptionId
            };
            return await connection.QueryFirstOrDefaultAsync<GetUserSubscriptionDetailDto>("GetUserSubscriptionDetailById", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> SaveUserSubscription(UserSubscriptionRequest request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                UserId = request.UserId,
                SubscriptionId = request.SubscriptionId,
                PaymentGatewaySubscriptionId = request.PaymentGatewaySubscriptionId,
                TransactionStatus = request.TransactionStatus,
                SubscriptionAmount = request.Price,
                TransactionAmount = request.Price,
                CheckoutSessionId = request.CheckoutSessionId,
                PurchaseDate = request.PurchaseDate,
                ExpiryDate = request.ExpiryDate,
                AddedOnUTC = System.DateTime.UtcNow
            };
            return await connection.ExecuteScalarAsync<int>("SaveUserSubscription", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateUserSubscription(UserSubscriptionRequest request, int userSubscriptionId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                Id = userSubscriptionId,  // Pass the ID of the subscription to update
                UserId = request.UserId,
                SubscriptionId = request.SubscriptionId,
                PaymentGatewaySubscriptionId = request.PaymentGatewaySubscriptionId,
                Amount = request.Price,
                TransactionStatus = request.TransactionStatus,
                CheckoutSessionId = request.CheckoutSessionId,
                PurchaseDate = request.PurchaseDate,
                ExpiryDate = request.ExpiryDate,
                AddedOnUTC = DateTime.UtcNow
            };

            // Execute the stored procedure and return the updated subscription ID
            return await connection.ExecuteScalarAsync<int>("UpdateUserSubscription", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CancelUserSubscription(string paypalSubscriptionId, int userId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                PaypalSubscriptionId = paypalSubscriptionId,
                UserId = userId 
            };

            // Execute the stored procedure and return the updated subscription ID
            return await connection.ExecuteAsync("CancelUserSubscription", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ActivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                Id = userSubscriptionId,  // Pass the ID of the subscription to update
                PaypalSubscriptionId = paypalSubscriptionId
            };

            // Execute the stored procedure and return the updated subscription ID
            return await connection.ExecuteScalarAsync<int>("ActivateUserSubscription", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeactivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId, short status)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                Id = userSubscriptionId,  // Pass the ID of the subscription to update
                PaypalSubscriptionId = paypalSubscriptionId,
                Status = status
            };

            // Execute the stored procedure and return the updated subscription ID
            return await connection.ExecuteScalarAsync<int>("DeactivateUserSubscription", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<SubscriptionPlan> GetSubscriptionDetailByPriceId(string priceId)
        {
            try
            {
                using var connection = _dbConnection.CreateDBConnection();
                var parms = new
                {
                    @PlanId = priceId
                };
                return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>("GetSubscriptionDetailByPlanId", param: parms, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new SubscriptionPlan();
            }

        }

        public async Task<SubscriptionPlan> GetFreeSubscriptionPlanDetail()
        {
            using var connection = _dbConnection.CreateDBConnection();
            return await connection.QueryFirstOrDefaultAsync<SubscriptionPlan>("GetFreeSubscriptionPlanDetail", param: null, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> SaveSubscriptionPaymentDetail(SaveSubscriptionPaymentDetailRequest request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @UserSubscriptionId = request.UserSubscriptionId,
                @VendorSubscriptionId = request.VendorSubscriptionId,
                @TransactionId = request.TransactionId,
                @TransactionStatus = request.TransactionStatus,
                @TransactionAmount = request.TransactionAmount,
                @TransactionFee = request.TransactionFee,
                @Currency = request.Currency,
                @TransactionDateTimeUTC = request.TransactionDateTimeUTC
            };
            return await connection.ExecuteAsync("SaveSubscriptionPaymentDetail", param: parms, commandType: CommandType.StoredProcedure);
        }
    }
}
