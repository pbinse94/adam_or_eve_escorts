using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Api.Gax;
using Microsoft.VisualBasic;
using Shared.Common.Enums;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Utility;
using System.Data;

namespace Data.Repository
{
    public class ProfileRepository : CurdRepository<EscortDetail>, IProfileRepository
    {
        private readonly IDbConnectionFactory _dbConnection;
        public ProfileRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> EditBankDetails(BankDetails request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EscortID", request.EscortID);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@BankAccountHolderName", request.BankAccountHolderName);
            parameters.Add("@BankAccountNumber", request.BankAccountNumber);
            parameters.Add("@BankName", request.BankName);
            parameters.Add("@BSBNumber", request.BSBNumber);
            return await connection.ExecuteScalarAsync<int>("EditBankDetails", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> Edit(EscortDetailDto request)
        {
            var locations = new List<LocationDto>();
            locations.AddRange(request.BaseLocation);
            locations.AddRange(request.TourLocation);

            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EscortID", request.EscortID);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@ProfileImage", request.ProfileImage);
            parameters.Add("@FirstName", request.FirstName);
            parameters.Add("@LastName", request.LastName);
            parameters.Add("@Email", request.Email);
            parameters.Add("@PhoneNumber", request.PhoneNumber);
            parameters.Add("@DisplayName", request.DisplayName);
            parameters.Add("@Age", request.Age);
            parameters.Add("@Height", request.Height);
            parameters.Add("@Eyes", request.Eyes);
            parameters.Add("@Category", request.Category != null ? string.Join(",", request.Category) : null);
            parameters.Add("@BodyType", request.BodyType);
            parameters.Add("@Language", request.Language != null ? string.Join(",", request.Language) : null);
            parameters.Add("@Bio", request.Bio);
            parameters.Add("@IsPhotoVerified", request.IsPhotoVerified);
            parameters.Add("@SexualPreferences", request.SexualPreferences != null ? string.Join(",", request.SexualPreferences) : null);
            parameters.Add("@Categories", request.Categories != null ? string.Join(",", request.Categories) : null);
            parameters.Add("@Gender", request.Gender);
            parameters.Add("@FacebookUrl", request.FaceBookUrl);
            parameters.Add("@TwitterUrl", request.TwitterUrl);
            parameters.Add("@InstagramUrl", request.InstagramUrl);
            parameters.Add("@YouTubeUrl", request.YouTubeUrl);
            parameters.Add("@TikTokUrl", request.TikTokUrl);
            parameters.Add("@SnapChatUrl", request.SnapChatUrl);
            parameters.Add("@LinkedinUrl", request.LinkedinUrl);
            parameters.Add("@Country", request.Country);
            parameters.Add("@CountryCode", request.CountryCode);
            parameters.Add("@Proffered", request.Proffered);
            parameters.Add("@UpdatedBy", request.UpdatedBy);

            parameters.Add("@Ethnicity", request.Ethnicity);
            parameters.Add("@Weight", request.Weight);
            parameters.Add("@Bust", request.Bust);
            parameters.Add("@Dress", request.Dress);
            parameters.Add("@HairColor", request.HairColor);
            parameters.Add("@RatesNotes", request.RatesNotes);
            parameters.Add("@AvailablityNotes", request.AvailablityNotes);

            parameters.Add("@UpdatedOnUTC", DateTime.UtcNow);
            parameters.AddTable("@LocationTableType", "LocationsTableType", locations);
            parameters.AddTable("@EscortRatesTableType", "EscortRatesTableType", request.Rates);
            parameters.AddTable("@AvailabilityCalendarTableType", "AvailabilityCalendarTableType", request.AvailabilityCalendar);

            return await connection.ExecuteScalarAsync<int>("EditProfile", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> SaveImagesAndVideoUrls(EscortGallery request)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EscortID", request.EscortId);
            parameters.Add("@FileName", request.FileName);
            parameters.Add("@MediaType", request.MediaType);
            return await connection.ExecuteAsync("SaveImagesAndVideoUrls", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> DeleteImages(string fileName)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ImageName", fileName);
            return await connection.ExecuteAsync("DeleteImage", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<EscortDetailDto> GetEscortDetailsById(int userId)
        {
            var escortDetails = new EscortDetailDto();

            using var connection = _dbConnection.CreateDBConnection();
            var parms = new { @UserId = userId };
            var result = await connection.QueryMultipleAsync("GetEscortDetailsById", param: parms, commandType: CommandType.StoredProcedure);
            escortDetails = (await result.ReadAsync<EscortDetailDto>()).FirstOrDefault();
            if (escortDetails != null)
            {
                if (!string.IsNullOrEmpty(escortDetails.SexualPreferencesId))
                {
                    escortDetails.SexualPreferences = escortDetails.SexualPreferencesId.Split(',');
                }

                if (!string.IsNullOrEmpty(escortDetails.CategoriesId))
                {
                    escortDetails.Categories = escortDetails.CategoriesId.Split(',');
                }



                if (!string.IsNullOrEmpty(escortDetails.CategoryId))
                {
                    escortDetails.Category = escortDetails.CategoryId.Split(',');
                }

                if (!string.IsNullOrEmpty(escortDetails.LanguageId))
                {
                    escortDetails.Language = escortDetails.LanguageId.Split(',');
                }

                escortDetails.Rates = (await result.ReadAsync<EscortRatesDto>()).ToList();
                escortDetails.AvailabilityCalendar = (await result.ReadAsync<AvailabilityCalendarDto>()).ToList();

                var location = (await result.ReadAsync<LocationDto>()).ToList();
                if (location.Count > 0)
                {
                    escortDetails.BaseLocation = location.Where(x => x.AddressType == (byte)LocationTypes.BaseLocation).ToList();
                    escortDetails.TourLocation = location.Where(x => x.AddressType == (byte)LocationTypes.TourLocation).ToList();
                }

                escortDetails.EscortGallery = (await result.ReadAsync<EscortGalleryDto>()).ToList();

                escortDetails.EscortSubscription = (await result.ReadAsync<EscortSubscriptionDto>()).FirstOrDefault();
            }
            return escortDetails ?? new EscortDetailDto();
        }

        public async Task<int> UpdateExcortViewCount(int excortId, int userId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EscortID", excortId);
            parameters.Add("@UserId", userId);
            return await connection.ExecuteAsync("AddUpdateEscortViewCount", parameters, commandType: CommandType.StoredProcedure); 
        }

        public async Task<int> IsHasAccess(int userId, int establishmentId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@establishmentId", establishmentId);
            return await connection.ExecuteScalarAsync<int>("IsHasAccess", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> GetUserIdByEscortId(int escortId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@escortId", escortId);
            return await connection.ExecuteScalarAsync<int>("GetUserIdByEscortId", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> MarkFavorites(int escortId, int userId)
        {
            using var connection = _dbConnection.CreateDBConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@escortId", escortId);
            return await connection.ExecuteAsync("MarkFavorite", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
