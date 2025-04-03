using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Entities;
using Shared.Model.Establishment;
using Shared.Model.Request.Admin;
using System.Data;

namespace Data.Repository
{
    public class EstablishmentRepository : CurdRepository<UserDetail>, IEstablishmentRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public EstablishmentRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<EstablishmentModel>> GetEstablishments(UsersRequestModel request)
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
                @Country = request.Country
            });
            return (await connection.QueryAsync<EstablishmentModel>("GetEstablishments", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<int> ChangeEstablishmentStatus(int userId, bool activeStatus)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var param = new {
                IsActive = activeStatus,
                AccessToken = Guid.NewGuid().ToString(),
                Id = userId
            };
            return await connection.ExecuteAsync("ChangeEstablishmentStatus", param, commandType: CommandType.StoredProcedure);
        }
    }
}
