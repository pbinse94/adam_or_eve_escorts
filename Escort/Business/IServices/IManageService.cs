using Microsoft.AspNetCore.Http;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;

namespace Business.IServices
{
    public interface IManageService
    {
        Task<ApiResponse<List<UsersDto>>> UserList(UsersRequestModel request);
        Task<ApiResponse<UserDetailsDto>> GetUserDetails(int userId);
        Task<ApiResponse<UserDetailsDto>> UpdateUserDetail(UserDetailsDto requestModel);
        Task<ResponseTypes> ChangePassword(ChangePasswordModel model, int userId);
        Task<int> ChangeUserStatus(int userId, bool activeStatus, bool deleteStatus);
        Task<int> AddTestimonials(int escortId, string testomonials,int userId);
        Task<List<TestimonialModel>> GetTestimonialAsync(int escortId);
        Task<ApiResponse<List<UsersDto>>> AdminUserList(UsersRequestModel request);
        Task<ApiResponse<List<ModulePermissionModel>>> GetUserPermission(int userId);
        Task<ApiResponse<bool>> AddUpdateAdminUser(AdminUserRequestModel requestModel, int loogedInUserId);
        Task<ApiResponse<bool>> IsUserEmailInUse(string email, int id);
        Task<ApiResponse<List<LoginHistoryDto>>> UserLoginHistory(LoginHistoryRequestModel request);
        Task<ApiResponse<List<ActivityLogDto>>> GetAdminUserActivities(ActivityLogHistoryRequestModel request);
    }
}
