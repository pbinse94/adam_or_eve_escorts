using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.WebUser;

namespace Business.IServices
{
    public interface IAccountService
    {
        Task<ApiResponse<ProfileDto>> Login(ApiLoginRequest request);
        Task<UserDetailsDto> FindByEmailAsync(string email);
        Task<ApiResponse<ProfileDto>> SignUp(RegistrationRequest request);
        Task<ApiResponse<bool>> VerifyEmail(EmailVerifyRequest request);
        Task<ApiResponse<bool>> ForgetPassword(ForgetPasswordRequest request);
        Task<ApiResponse<bool>> ResendVerificationLink(ResendVerificationLinkRequest request);

        Task<ApiResponse<bool>> SaveContactUsDetails(ContactUsRequestModel requestModel);

        #region Web User
        Task<ApiResponse<ForgotPasswordDto>> ResetPasswordTokenAsync(long userId);
        Task<bool> CheckResetPasswordTokenExist(string token);
        Task<ResponseTypes> ResetPassword(ResetPasswordModel model);
        Task<ApiResponse<bool>> UpdateEmailVerificationToken(int userId, string email, string name);
        #endregion
        CheckUserAccessTokenDto CheckUserAccessToken(string accessToken);
        bool CheckAppVersion(string appVersion, short deviceTypeId);
        Task<UserDetail> GetByIdAsync(int userId);
        Task<List<GetCountryCodesDto>> GetCountryCodes();
        Task<List<EscortSearchDto>> GetSearchedEscorts();
        Task<List<EscortSearchDto>> GetFeaturedEscorts(EscortSearchRequest searchRequest);
        Task<List<EscortSearchDto>> GetVipEscorts(EscortSearchRequest searchRequest);
        Task<List<EscortSearchDto>> GetFavoriteEscorts(EscortSearchRequest searchRequest);
        Task<bool> CheckEmailVerificationTokenExist(string token);
        Task<ApiResponse<ProfileDto>> AdminLogin(ApiLoginRequest request);
        Task<int> GetEstablishmentEscortsCount(int establishmentId);
        Task<ApiResponse<bool>> LogoutUser(int id, string accessToken);
        Task<ApiResponse<bool>> LogoutAllUser(string accessToken);
        Task<UserDetailsDto> FindByEmailAndUpdateAsync(string email, string accessToken);
      
        Task<List<EscortSearchDto>> GetPopularEscorts(PopularEscortRequest searchRequest);
        Task<ApiResponse<bool>> PauseEscort(int userId, bool isPause);
        Task<ApiResponse<bool>> ApproveEscort(int userId, bool isApprove, int loginUserId);
        Task<UserDetail> GetAdminDetailById(int userId, string accessToken);
        Task<ApiResponse<UserDetailsDto>> SaveFreeSubscription(string userEmail);

    }
}
