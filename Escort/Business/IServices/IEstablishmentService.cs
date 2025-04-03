using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Establishment;
using Shared.Model.Request.Admin;

namespace Business.IServices
{
    public interface IEstablishmentService
    {
        Task<ApiResponse<List<EstablishmentModel>>> EstablishmentList(UsersRequestModel request);
        Task<int> ChangeEstablishmentStatus(int userId, bool activeStatus);
        Task<ApiResponse<EscortDetailDto>> GetEstablishmentById(int userId);
    }
}
