using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;

namespace Data.IRepository
{
    public interface IProfileRepository : ICurdRepository<EscortDetail>
    {
        Task<int> Edit(EscortDetailDto request);
        Task<int> SaveImagesAndVideoUrls(EscortGallery request);
        Task<EscortDetailDto> GetEscortDetailsById(int userId);
        Task<int> DeleteImages(string fileName);
        Task<int> MarkFavorites(int escort, int userId);
        Task<int> IsHasAccess(int userId, int establishmentId);
        Task<int> GetUserIdByEscortId(int escortId);
        Task<int> EditBankDetails(BankDetails request);
        Task<int> UpdateExcortViewCount(int excortId, int userId);
    }
}
