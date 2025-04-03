using Shared.Model.Entities;
using Shared.Model.Establishment;
using Shared.Model.Request.Admin;

namespace Data.IRepository
{
    public interface IEstablishmentRepository : ICurdRepository<UserDetail>
    {
        Task<List<EstablishmentModel>> GetEstablishments(UsersRequestModel request);
        Task<int> ChangeEstablishmentStatus(int userId, bool activeStatus);
    }
}
