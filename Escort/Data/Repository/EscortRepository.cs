using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Common.Enums;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Escort;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class EscortRepository : CurdRepository<UserDetail>,IEscortRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public EscortRepository(IDbConnectionFactory dbConnection) : base(dbConnection) 
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<EscortModel>> EscortList(UsersRequestModel request)
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
                @Role = request.UserType,
                @EstablishmentId=request.EstablishmentId,
                @Country = request.Country,
                @Gender = request.Gender,
                @Status=request.Status
            });
            return (await connection.QueryAsync<EscortModel>("GetEscortList", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<EscortSearchDto>> GetEscortsByIds(List<RequestId> ids)
        {
            using var connection = _dbConnection.CreateDBConnection();

            DynamicParameters parameters = new DynamicParameters();
            parameters.AddTable("@EscortIds", "UDDT_Id", ids);
            return (await connection.QueryAsync<EscortSearchDto>("GetEscortByIds", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<CountryWiseEscortDto>> GetCountryWiseEscortCount()
        {
            using var connection = _dbConnection.CreateDBConnection();

            return (await connection.QueryAsync<CountryWiseEscortDto>("CountryWiseEscortCount", null, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<GetLastTwelveMonthGiftTokensDto>> GetLastTwelveMonthGiftTokens()
        {
            using var connection = _dbConnection.CreateDBConnection();

            return (await connection.QueryAsync<GetLastTwelveMonthGiftTokensDto>("GetLastTwelveMonthGiftTokens", null, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<GetLastTwelveMonthSubscriptionReportDto>> GetLastTwelveMonthSubscriptionReport()
        {
            using var connection = _dbConnection.CreateDBConnection();

            return (await connection.QueryAsync<GetLastTwelveMonthSubscriptionReportDto>("GetLastTwelveMonthSubscriptionReport", null, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<GetLastTwelveMonthRevenueReportDto>> GetLastTwelveMonthRevenueReport(decimal adminPercentage)
        {
            using var connection = _dbConnection.CreateDBConnection();

            return (await connection.QueryAsync<GetLastTwelveMonthRevenueReportDto>("GetLastTwelveMonthRevenueReport", new { @AdminPercentage = adminPercentage }, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<AdminDashboardStatisticsDto> AdminDashboardStatistics(decimal adminPercentage)
        {
            using var connection = _dbConnection.CreateDBConnection();

            return await connection.QueryFirstOrDefaultAsync<AdminDashboardStatisticsDto>("AdminDashboardStatistics", new { @AdminPercentage = adminPercentage}, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetEscortScore(decimal userId)
        {
            using var connection = _dbConnection.CreateDBConnection();

            return await connection.ExecuteScalarAsync<int>("GetEscortScore", new { @UserId = userId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> StartLiveCam(int escortUserId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EscortUserId", escortUserId);
            return await connection.ExecuteScalarAsync<int>("StartLiveCam", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> StopLiveCam(int escortUserId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EscortUserId", escortUserId);
            return await connection.ExecuteScalarAsync<int>("StopLiveCam", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> JoinLiveCam(int clientId, int escortId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EscortId", escortId);
            parameters.Add("@ClientId", clientId);
            return await connection.ExecuteScalarAsync<int>("JoinLiveCam", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<GetEscortDashboardStatisticsDto> GetEscortDashboardStatistics(int escortUserId, decimal adminPercentage)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EscortUserId", escortUserId);
            parameters.Add("@AdminPercentage", adminPercentage);
            return await connection.QueryFirstOrDefaultAsync<GetEscortDashboardStatisticsDto>("GetEscortDashboardStatistics", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<EscortLiveCamGiftSummeryDto>> GetEscortLiveCamGiftSummery(EscortLiveCamGiftSummeryRequest request, decimal adminPercentage)
        {
            using var connection = _dbConnection.CreateDBConnection();

            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @EscortUserId = request.EscortUserId,
                @FromDate = request.FromDate,
                @ToDate = request.ToDate,
                @AdminPercentage = adminPercentage
            });
            return (await connection.QueryAsync<EscortLiveCamGiftSummeryDto>("GetEscortLiveCamGiftSummery", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

    }
}
