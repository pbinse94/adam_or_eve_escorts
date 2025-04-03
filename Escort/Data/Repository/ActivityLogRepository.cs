using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.AdminUser;
using System.Data;

namespace Data.Repository
{
    public class ActivityLogRepository : CurdRepository<ActivityLog>, IActivityLogRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public ActivityLogRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public async Task<List<ActivityLogDto>> ActivityLogHistory(ActivityLogHistoryRequestModel request)
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
                @AdminUserId = request.AdminUserId,
                @FromDate = request.FromDate,
                @ToDate = request.ToDate
            });
            return (await connection.QueryAsync<ActivityLogDto>("GetAllActivityLog", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }
    }
}
