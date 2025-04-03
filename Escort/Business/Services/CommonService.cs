using Business.IServices;
using Data.IRepository;
using Shared.Model.Base;
using Shared.Model.DTO;

namespace Business.Services
{
    public class CommonService : ICommonService
    {
        private readonly ICommonRepository _commonRepository;
        public CommonService(ICommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        public async Task<List<CountryDto>> GetCountry(int? id = null)
        {
            return await _commonRepository.GetCountry(id);
        }
        public async Task<List<StateDto>> GetStates(int? countryId = null, int? stateId = null)
        {
            return await _commonRepository.GetStates(countryId, stateId);
        }
        public async Task<List<CityDto>> GetCity(int? stateId = null, int? cityId = null)
        {
            return await _commonRepository.GetCity(stateId, cityId);
        }

        public async Task<List<CategoriesDto>> GetCategories()
        {
            return await _commonRepository.GetCategories();
        }

      
    }
}
