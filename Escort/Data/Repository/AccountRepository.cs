using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Common.Enums;
using Shared.Model.Common;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;
using Shared.Model.Request.WebUser;
using Shared.Utility;
using System.Data;

namespace Data.Repository
{
    public class AccountRepository : CurdRepository<UserDetail>, IAccountRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public AccountRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<GetCountryCodesDto>> GetCountryCodes()
        {
            using var connection = _dbConnection.CreateDBConnection();

            return (await connection.QueryAsync<GetCountryCodesDto>("GetCountryCodes", param: null, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<UserDetailsDto> FindByEmailAsync(string email)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @Email = email
            };
            return await connection.QueryFirstOrDefaultAsync<UserDetailsDto>("GetUserByEmail", param: parms, commandType: CommandType.StoredProcedure);
        }


        public async Task<UserDetailsDto> FindByEmailAndUpdateAsync(string email, string accessToken)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @Email = email,
                @AccessToken = accessToken
            };
            return await connection.QueryFirstOrDefaultAsync<UserDetailsDto>("CheckAndUpdateUser", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ManageLoginAccessDetail(ManageLoginAccessDetailRequest request)
        {
            using var connection = _dbConnection.CreateDBConnection();

            var parms = new
            {
                @Email = request.Email,
                @DeviceType = request.DeviceType,
                @DeviceToken = request.DeviceToken,
                @AccessToken = request.AccessToken,
                @IpAddress = request.IpAddress,
                @DeviceInfo = request.DeviceInfo
            };
            return await connection.ExecuteAsync("ManageLoginAccessDetail", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<UsersDto>> UserList(UsersRequestModel request)
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
                @Role = (int)UserTypes.Client,
                @Country = request.Country,
                @Gender = request.Gender,
            });
            return (await connection.QueryAsync<UsersDto>("GetClientsList", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }


        public async Task<int> ChangePassword(ChangePasswordModel model, long userId)
        {
            using var connection = _dbConnection.CreateDBConnection();

            return await connection.ExecuteAsync("ChangePassword", new
            {
                UserId = userId,
                Password = model.Password,
            }, commandType: CommandType.StoredProcedure);
        }


        public async Task<ForgotPasswordDto> ResetPasswordTokenAsync(long userId, string forgotPasswordToken)
        {
            using var connection = _dbConnection.CreateDBConnection();

            var parms = new
            {
                @UserId = userId,
                @ForgotPasswordToken = forgotPasswordToken
            };
            return await connection.QueryFirstOrDefaultAsync<ForgotPasswordDto>("UpdateForgotPasswordToken", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> CheckResetPasswordTokenExist(string token)
        {
            using var connection = _dbConnection.CreateDBConnection();
            return await connection.QueryFirstOrDefaultAsync<bool>("CheckResetPasswordTokenExistByToken", new { Token = token }, commandType: CommandType.StoredProcedure);
        }

        public async Task<UserDetailsDto> GetUserDetailByToken(string token)
        {
            using var connection = _dbConnection.CreateDBConnection();
            return await connection.QueryFirstOrDefaultAsync<UserDetailsDto>("GetUserDetailByToken", new { Token = token }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ResetPassword(ResetPasswordModel model)
        {
            using var connection = _dbConnection.CreateDBConnection();
            return await connection.ExecuteAsync("UpdateUserByToken", new
            {
                ForgotPasswordToken = model.Token,
                Password = Encryption.ComputeHash(model.Password),
            }, commandType: CommandType.StoredProcedure);
        }

        public CheckUserAccessTokenDto CheckUserAccessToken(string accessToken)
        {
            using var connection = _dbConnection.CreateDBConnection();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AccessToken", accessToken);
            return connection.QueryFirstOrDefault<CheckUserAccessTokenDto>("CheckAccessTokenExists", parameters, commandType: CommandType.StoredProcedure);
        }


        public async Task<UserDetail> GetAdminDetailById(int id, string accessToken)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new { Id = id, @AccessToken = accessToken };
            return await connection.QueryFirstOrDefaultAsync<UserDetail>("GetAdminDetailById", parameters, commandType: CommandType.StoredProcedure);
        }
        public bool CheckAppVersion(string appVersion, short deviceTypeId)
        {
            using var connection = _dbConnection.CreateDBConnection();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AppVersion", appVersion);
            parameters.Add("@DeviceTypeId", deviceTypeId);
            return connection.QueryFirstOrDefault<bool>("CheckAppVersion", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> LogoutUser(int id, string accessToken)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new { Id = id, @AccessToken = accessToken };
            return await connection.ExecuteAsync("LogoutUser", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> PauseEscort(int userId, bool isPause)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new { UserId = userId, IsPause = isPause };
            return await connection.ExecuteAsync("PauseEscort", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ApproveEscort(int userId, bool isApprove, int loginUserId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new { UserId = userId, IsApprove = isApprove, LoginUserId = loginUserId };
            return await connection.ExecuteAsync("ApproveEscort", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddTestimonials(int escortId, string testimonials, int userId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new
            {
                EscortId = escortId,
                UserId = userId,
                Testimonial = testimonials,
                AddedOnUTC = DateTime.UtcNow
            };
            return await connection.ExecuteAsync("InsertTestimonial", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<TestimonialModel>> GetTestimonialAsync(int escortId)
        {
            using var connection = _dbConnection.CreateDBConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@EscortId", escortId);


            return (await connection.QueryAsync<TestimonialModel>("GetTestimonialByEscortId", param: parameters, commandType: CommandType.StoredProcedure)).AsList();



        }

        public async Task<bool> UpdateStripeCustomerId(string stripeCustomerId, int userId)
        {
            using IDbConnection connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters(new
            {
                @UserId = userId,
                @CustomerIdStripe = stripeCustomerId
            });
            return await connection.QueryFirstOrDefaultAsync<bool>("UpdateCustomerIdStripe", param: parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<EscortSearchDto>> GetSearchedEscorts()
        {
            using var connection = _dbConnection.CreateDBConnection();

            //var parms = new
            //{
            //    @UserId = userId,
            //    @ForgotPasswordToken = 
            //};
            return (await connection.QueryAsync<EscortSearchDto>("SearchEscorts", param: null, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<EscortSearchDto>> GetFeaturedEscorts(EscortSearchRequest searchRequest)
        {

            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LoginUserId", searchRequest.LoginUserId);
            parameters.Add("@StartAge", searchRequest.StartAge);
            parameters.Add("@EndAge", searchRequest.EndAge);
            parameters.Add("@StartRate", searchRequest.StartRate);
            parameters.Add("@EndRate", searchRequest.EndRate);
            parameters.Add("@Gender", searchRequest.Gender);
            parameters.Add("@Name", searchRequest.Name ?? "");
            parameters.Add("@EscortType", searchRequest.EscortType);
            parameters.Add("@Country", searchRequest.Country ?? "");
            parameters.Add("@Services", string.Join(",", searchRequest.Services) ?? "");
            parameters.Add("@EscortCategories", string.Join(",", searchRequest.EsortCategories) ?? "");
            parameters.Add("@PageIndex ", searchRequest.PageIndex);
            parameters.Add("@PageSize ", searchRequest.PageSize);
            parameters.Add("@ProfileType ", searchRequest.ProfileType);
            parameters.Add("@CallType ", searchRequest.CallType);
            return (await connection.QueryAsync<EscortSearchDto>("GetFilteredEscorts", parameters, commandType: CommandType.StoredProcedure)).ToList();

        }

        public async Task<List<EscortSearchDto>> GetVipEscorts(EscortSearchRequest searchRequest)
        {

            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LoginUserId", searchRequest.LoginUserId);
            parameters.Add("@Country", searchRequest.Country);

            return (await connection.QueryAsync<EscortSearchDto>("GetVipEscorts", parameters, commandType: CommandType.StoredProcedure)).ToList();

        }

        public async Task<List<EscortSearchDto>> GetPopularEscorts(PopularEscortRequest searchRequest)
        {

            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LoginUserId", searchRequest.LoginUserId);
            parameters.Add("@PageIndex", searchRequest.PageIndex);
            parameters.Add("@PageSize", searchRequest.PageSize);
            parameters.Add("@SearchText", searchRequest.SearchText);
            parameters.Add("@Country", searchRequest.Country ?? "");

            return (await connection.QueryAsync<EscortSearchDto>("GetPopularEscorts", parameters, commandType: CommandType.StoredProcedure)).ToList();

        }

        public async Task<bool> EmailVerifyByToken(string emailVerifiedToken)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmailVerifiedToken", emailVerifiedToken);
            return await connection.ExecuteScalarAsync<bool>("EmailVerifyByToken", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetEstablishmentEscortsCount(int establishmentId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EstablishmentId", establishmentId);
            return await connection.ExecuteScalarAsync<int>("GetEstablishmentEscortsCount", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<EscortSearchDto>> GetFavoriteEscorts(EscortSearchRequest searchRequest)
        {
            try
            {
                using var connection = _dbConnection.CreateDBConnection();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@LoginUserId", searchRequest.LoginUserId);
                parameters.Add("@Name", searchRequest.Name ?? "");
                parameters.Add("@PageIndex ", searchRequest.PageIndex);
                parameters.Add("@PageSize ", searchRequest.PageSize);
                return (await connection.QueryAsync<EscortSearchDto>("GetFavoriteEscorts", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex) { ex.ToString(); }
            return [];
        }

        // admin users

        public async Task<List<UsersDto>> AdminUserList(UsersRequestModel request)
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
            return (await connection.QueryAsync<UsersDto>("GetAdminUserList", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<AdminUserPermissionDto>> GetUserPermission(int userId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @UserId = userId,
            });
            return (await connection.QueryAsync<AdminUserPermissionDto>("GetUserPermissions", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<AddUpdateAdminUserResponseModel> AddUpdateAdminUser(AdminUserRequestModel requestModel, List<AdminUserPermissionRequestModel> permissions, int loogedInUserId)
        {
            using var connection = _dbConnection.CreateDBConnection();


            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", requestModel.Id);
            parameters.Add("@LoogedInUserId", loogedInUserId);
            parameters.Add("@FirstName", requestModel.FirstName);
            parameters.Add("@LastName", requestModel.LastName);
            parameters.Add("@Email", requestModel.Email);
            parameters.Add("@UserType", requestModel.UserType);
            parameters.Add("@PasswordHash", requestModel.PasswordHash);
            parameters.Add("@DisplayName", requestModel.DisplayName);
            parameters.Add("@EmailVerifiedToken", requestModel.EmailVerifiedToken);
            parameters.Add("@IsActive", requestModel.IsActive);
            parameters.AddTable("@Permissions", "PermissionTableType", permissions);

            return await connection.QueryFirstOrDefaultAsync<AddUpdateAdminUserResponseModel>("AddUpdateAdminUser", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> IsUserEmailInUse(string email, int id)
        {
            using var connection = _dbConnection.CreateDBConnection();


            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@Email", email);

            string query = "SELECT 1 FROM UserDetail WHERE Email = @Email AND UserId != @Id";

            return await connection.ExecuteScalarAsync<int>(query, parameters);
        }


        public async Task<bool> CheckEmailVerificationTokenExist(string token)
        {
            using var connection = _dbConnection.CreateDBConnection();
            return await connection.QueryFirstOrDefaultAsync<bool>("CheckResetPasswordTokenExistByToken", new { Token = token }, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<LoginHistoryDto>> UserLoginHistory(LoginHistoryRequestModel request)
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
                @UserId = request.UserId,
                @FromDate = request.FromDate,
                @ToDate = request.ToDate
            });
            return (await connection.QueryAsync<LoginHistoryDto>("GetUserLoginHistory", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<int> AddSupportTicket(ContactUsRequestModel request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CountryCode = request.CountryCode,
                PhoneNumber = request.PhoneNumber,
                Message = request.Query
            };
            return await connection.ExecuteAsync("AddSupportTicket", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<LoginDevicInfo> GetLoginDeviceInfo(int userId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parms = new
            {
                @UserId = userId
            };
            return await connection.QueryFirstOrDefaultAsync<LoginDevicInfo>("[GetLoginDeviceInfo]", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> LogoutAllUser(string accessToken)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new { @AccessToken = accessToken };
            return await connection.ExecuteAsync("[LogoutAllUser]", parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
