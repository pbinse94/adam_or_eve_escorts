using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;
using Shared.Model.Request.WebUser;

namespace Data.IRepository
{
    public interface IAccountRepository : ICurdRepository<UserDetail>
    {
        Task<int> ManageLoginAccessDetail(ManageLoginAccessDetailRequest request);
        Task<UserDetailsDto> FindByEmailAsync(string email);
        Task<List<UsersDto>> UserList(UsersRequestModel request);
        Task<int> ChangePassword(ChangePasswordModel model, long userId);

        #region Web User

        Task<ForgotPasswordDto> ResetPasswordTokenAsync(long userId, string forgotPasswordToken);
        Task<bool> CheckResetPasswordTokenExist(string token);
        Task<UserDetailsDto> GetUserDetailByToken(string token);
        Task<int> ResetPassword(ResetPasswordModel model);
        #endregion
        CheckUserAccessTokenDto CheckUserAccessToken(string accessToken);
        bool CheckAppVersion(string appVersion, short deviceTypeId);
        Task<int> LogoutUser(int id, string accessToken);
        Task<int> PauseEscort(int userId, bool isPause);
        Task<int> ApproveEscort(int userId, bool isApprove, int loginUserId);
        Task<int> AddTestimonials(int escortId, string testimonials, int userId);
        Task<List<TestimonialModel>> GetTestimonialAsync(int escortId);
        Task<List<GetCountryCodesDto>> GetCountryCodes();
        Task<List<EscortSearchDto>> GetSearchedEscorts();
        Task<List<EscortSearchDto>> GetFeaturedEscorts(EscortSearchRequest searchRequest);
        Task<List<EscortSearchDto>> GetVipEscorts(EscortSearchRequest searchRequest);
        Task<List<EscortSearchDto>> GetFavoriteEscorts(EscortSearchRequest searchRequest); 
        Task<bool> EmailVerifyByToken(string emailVerifiedToken);
        Task<List<UsersDto>> AdminUserList(UsersRequestModel request);

        Task<List<AdminUserPermissionDto>> GetUserPermission(int userId);
        Task<AddUpdateAdminUserResponseModel> AddUpdateAdminUser(AdminUserRequestModel requestModel, List<AdminUserPermissionRequestModel> permissions, int loogedInUserId);
        Task<int> IsUserEmailInUse(string email, int id);
        Task<bool> CheckEmailVerificationTokenExist(string token);
        Task<int> GetEstablishmentEscortsCount(int establishmentId);
        Task<List<LoginHistoryDto>> UserLoginHistory(LoginHistoryRequestModel request);
        Task<int> AddSupportTicket(ContactUsRequestModel request);
        Task<List<EscortSearchDto>> GetPopularEscorts(PopularEscortRequest searchRequest);
        Task<LoginDevicInfo> GetLoginDeviceInfo(int userId);
        Task<UserDetail> GetAdminDetailById(int id, string accessToken);
        Task<int> LogoutAllUser(string accessToken);
        Task<UserDetailsDto> FindByEmailAndUpdateAsync(string email, string accessToken);
    }
}
