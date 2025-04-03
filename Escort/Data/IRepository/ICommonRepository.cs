using Shared.Model.Base;
using Shared.Model.DTO;

namespace Data.IRepository
{
    public interface ICommonRepository
    {
        Task<List<CountryDto>> GetCountry(int? id = null);
        Task<List<StateDto>> GetStates(int? countryId = null, int? stateId = null);
        Task<List<CityDto>> GetCity(int? stateId = null, int? cityId = null);
        Task<List<CategoriesDto>> GetCategories();
    }
}
