using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.AdminUser;
using System.Data;

namespace Data.Repository
{
    public class UserVerificationCodeRepository : CurdRepository<UserVerificationCode>, IUserVerificationCodeRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public UserVerificationCodeRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<UserVerificationCode> FindByEmailAsync(string email)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @Email = email
            };
            return await connection.QueryFirstOrDefaultAsync<UserVerificationCode>("GetUserVerificationCodeByEmail", param: parms, commandType: CommandType.StoredProcedure);
        }
    }
}
