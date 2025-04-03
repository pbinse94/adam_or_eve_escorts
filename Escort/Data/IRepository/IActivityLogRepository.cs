using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.AdminUser;

namespace Data.IRepository
{
    public interface IActivityLogRepository : ICurdRepository<ActivityLog>
    {
        Task<List<ActivityLogDto>> ActivityLogHistory(ActivityLogHistoryRequestModel request);

    }
}
