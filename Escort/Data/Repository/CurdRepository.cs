using Dapper;
using Data.IFactory;
using Data.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Data.Repository
{
    public class CurdRepository<T> : ICurdRepository<T>
    {

        private readonly IDbConnection _dbConnection;
        public CurdRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return (await _dbConnection.QueryAsync<T>($"GetAll{typeof(T).Name}s", commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var parameters = new { Id = id };
            return await _dbConnection.QueryFirstOrDefaultAsync<T>($"Get{typeof(T).Name}ById", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddUpdateScalerAsync(T entity)
        {
            try
            {
                return await _dbConnection.ExecuteScalarAsync<int>($"AddUpdate{typeof(T).Name}", entity, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return 0;
        }
        public async Task<int> AddUpdateAsync(T entity)
        {
            return await _dbConnection.ExecuteAsync($"AddUpdate{typeof(T).Name}", entity, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync($"Delete{typeof(T).Name}", parameters, commandType: CommandType.StoredProcedure);
        }

    }
}