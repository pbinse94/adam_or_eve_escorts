using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.DTO;
using Shared.Model.Request.AdminUser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class SubscriptionTransactionsRepository : ISubscriptionTransactionsRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public SubscriptionTransactionsRepository(IDbConnectionFactory dbConnection) 
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<SubscriptionTransactionsDto>> GetSubscriptionTransactions(SubscriptionTransactionsRequestModel request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @PageNo = request.Start,
                @PageSize = request.Length,
                @SearchKeyword = request.SearchKeyword,
                @SortColumn = request.SortColumn,
                @SortOrder = request.SortOrder,
                @FromDate = request.FromDate,
                @ToDate = request.ToDate
            });
            return (await connection.QueryAsync<SubscriptionTransactionsDto>("GetSubscriptionTransactions", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }
    }
}
