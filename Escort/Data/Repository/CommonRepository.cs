using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.DTO;
using Shared.Model.Entities;
using System.Data;

namespace Data.Repository
{
    public class CommonRepository : CurdRepository<Country>, ICommonRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public CommonRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<CountryDto>> GetCountry(int? id = null)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @Id = id
            };
            return (await connection.QueryAsync<CountryDto>("GetCountry", param: parms, commandType: CommandType.StoredProcedure)).AsList();
        }
        public async Task<List<StateDto>> GetStates(int? countryId = null, int? stateId = null)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @StateId = stateId,
                @CountryId = countryId
            };
            return (await connection.QueryAsync<StateDto>("GetStates", param: parms, commandType: CommandType.StoredProcedure)).AsList();
        }
        public async Task<List<CityDto>> GetCity(int? stateId = null, int? cityId = null)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @CityId = cityId,
                @StateId = stateId
            };
            return (await connection.QueryAsync<CityDto>("GetCity", param: parms, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<CategoriesDto>> GetCategories()
        {
            using var connection = _dbConnection.CreateDBConnection(); 
            return (await connection.QueryAsync<CategoriesDto>("GetCategories", null, commandType: CommandType.StoredProcedure)).AsList();
        }
    }
}
