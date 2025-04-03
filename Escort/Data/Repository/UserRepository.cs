using Dapper;
using Data.IFactory;
using Data.IRepository;
using Newtonsoft.Json.Linq;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class UserRepository : CurdRepository<UserDetail>, IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public UserRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<UserTokenTransactionDto>> UserList(UserTokenTransactionRequestModel request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @PageNo = request.Start,
                @PageSize = request.Length,
                @SearchKeyword = request.Search?.Value,
                @SortColumn = request.SortColumn,
                @SortOrder = request.SortOrder,
            });
            return (await connection.QueryAsync<UserTokenTransactionDto>("GetUserTokenTransactionList", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<UserTokenTransactionDto>> ListByName(UserTokenTransactionRequestModel request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @PageNo = request.Start,
                @PageSize = request.Length,
                @SearchKeyword = request.Search?.Value,
                @SortColumn = request.SortColumn,
                @SortOrder = request.SortOrder,
                @FilterBy = request.FilterBy,
                @CurrentId = request.UserId,
                @FromDate = request.FromDate,
                @ToDate = request.ToDate
            });
            return (await connection.QueryAsync<UserTokenTransactionDto>("GetUserTokenTransactionByClient", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<GetClientBalanceReportDto> GetClientBalanceReport(UserTokenTransactionRequestModel request, int loginUserId)
        {
            GetClientBalanceReportDto clientTransactionDetail = new();

            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @ClientId = request.UserId,
                @LoginUserId = loginUserId,
            });
            
            var result = await connection.QueryMultipleAsync("GetClientBalanceReport", param: parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                clientTransactionDetail.Client = (await result.ReadAsync<GetClientDetailDto>()).FirstOrDefault();
                clientTransactionDetail.Transactions = (await result.ReadAsync<GetClientTransactionReportDto>()).ToList();
            }
            
            return clientTransactionDetail;
        }

        public async Task<int> SendGift(SendGiftRequest request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new {
                @ClientId = request.ClientId,
                @EscortId = request.EscortId,
                @Tokens = request.Tokens,
                @GiftIconID = request.GiftIconID,
                @CreatedOnUTC = DateTime.UtcNow,
                @TransactionType = request.TransactionType
            };
            return (await connection.ExecuteAsync("SendGift", parameters, commandType: CommandType.StoredProcedure));
        }

        public async Task<int> CheckAndInsertIPAddress(string request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IPAddress", request);
            parameters.Add("@IsFound", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@UserId", LoginMemberSession.UserDetailSession?.UserId);


            await connection.ExecuteAsync("CheckAndInsertIPAddress", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<Int32>("@IsFound");
        }


        public async Task<List<PaymentReportDto>> PaymentReportList(PaymentReportRequestModel request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @PageNo = request.Start,
                @PageSize = request.Length,
                @PaidFilter = request.PaidFilter,// request.IsPaid,
                @SortColumn = request.SortColumn,
                @SortOrder = request.SortOrder,
                @FromDate = request.FromDate,
                @ToDate = request.ToDate,
                @SearchKeyword = request.Search?.Value,

            });
            return (await connection.QueryAsync<PaymentReportDto>("GetPaymentReport", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }


        public async Task<List<UserTokenTransactionClientDto>> GetClientBalanceReportAdmin(UserTokenTransactionRequestModel request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @PageNo = request.Start,
                @PageSize = request.Length, 
                @SortColumn = request.SortColumn,
                @SortOrder = request.SortOrder, 
                @SearchKeyword = request.Search?.Value,
                @ClientId= request.ClientId

            });
            return (await connection.QueryAsync<UserTokenTransactionClientDto>("GetUserTokenTransactionClientById", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<EscortPaymentAmountFromClientDto>> EscortPaymentReportList(EscortPaymentReportRequestModel request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @EscortId = request.EscortId,
                @IsPaid = request.IsPaid,
                @AdminPercentage = request.AdminPercentage
            });
            return (await connection.QueryAsync<EscortPaymentAmountFromClientDto>("EscortPaymentAmountFromClient", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }


        public async Task<int> MarkPaymentDone(string usersid)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @UsersId = usersid,
            });
            return (await connection.QueryFirstOrDefaultAsync<int>("MarkUsersPaymentDone", parameters, commandType: CommandType.StoredProcedure));
        }
    }
}