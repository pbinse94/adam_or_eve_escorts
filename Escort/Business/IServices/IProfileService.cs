using Shared.Common;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Profile;

namespace Business.IServices
{
    public interface IProfileService
    {
        Task<ApiResponse<GetUserDetailsDto>> GetUserDetails(int userId);
        Task<ApiResponse<bool>> UpdateProfile(UpdateProfileRequest profileRequest, int userId);
        Task<ApiResponse<bool>> MarkFavorites(int escortId, int userId);
        Task<ApiResponse<bool>> DeleteProfile(int userId);
        Task<ApiResponse<int>> Edit(EscortDetailDto requestModel);
        Task<ApiResponse<EscortDetailDto>> GetEscortProfileDetails(int userId);
        Task<ApiResponse<Tuple<EscortDetailDto?, EscortRules?>>> GetEscortFullProfileDetails(int userId, int clientId);
        Task<ApiResponse<EscortDetailDto>> GetEscortFullProfileDetails(int userId);
        Task<int> GetUserIdByEscortId(int escortId);

        Task<int> IsHasAccess(int userId,int establistmentId);
        Task<ApiResponse<bool>> UploadImageFiles(EscortsFileDto requestModel);
        Task<ApiResponse<bool>> UploadVideoFiles(EscortsFileDto requestModel);
        Task<ApiResponse<bool>> DeleteImage(string fileName);
        Task<ApiResponse<int>> EditBankDetails(BankDetails requestModel);
    }
}
