using Azure;
using Business.IServices;
using Data.IRepository;
using Google.Api.Gax;
using Microsoft.AspNetCore.Http;

//using Microsoft.AspNetCore.Http.Internal;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Profile;
using Shared.Resources;
using static Google.Apis.Requests.BatchRequest;

namespace Business.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IFileStorageService _fileStorageService;
        public ProfileService(IAccountRepository accountRepository, IProfileRepository profileRepository, IFileStorageService fileStorageService)
        {
            _accountRepository = accountRepository;
            _profileRepository = profileRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<GetUserDetailsDto>> GetUserDetails(int userId)
        {
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);

            if (getUserDetail is null)
            {
                return new ApiResponse<GetUserDetailsDto>(null, message: ResourceString.UserDetailsNotFound, apiName: "GetUserDetails");
            }

            GetUserDetailsDto userDetailsDto = new GetUserDetailsDto
            {
                FirstName = getUserDetail.FirstName,
                LastName = getUserDetail.LastName,
                Email = getUserDetail.Email,
                ProfileImage = getUserDetail.ProfileImage,
                PhoneNumber = getUserDetail.PhoneNumber,
                UserType = Convert.ToInt16(getUserDetail.UserType),
                CreditBalance = getUserDetail.TotalToken
            };

            userDetailsDto.ProfileImage = CommonFunctions.GetRelativeFilePath(userDetailsDto.ProfileImage, Constants.UserImageFolderPath, Constants.DefaultUserPng);
            return new ApiResponse<GetUserDetailsDto>(userDetailsDto, message: ResourceString.GetUserDetails, apiName: "GetUserDetails");
        }

        public async Task<ApiResponse<EscortDetailDto>> GetEscortProfileDetails(int userId)
        {
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);

            if (getUserDetail is null)
            {
                return new ApiResponse<EscortDetailDto>(null, message: ResourceString.UserDetailsNotFound, apiName: "GetEscortProfileDetails");
            }

            var getEscortDetail = await _profileRepository.GetEscortDetailsById(userId);

            EscortDetailDto userDetailsDto = new EscortDetailDto
            {
                //-- User details
                UserId = userId,
                FirstName = getUserDetail.FirstName,
                LastName = getUserDetail.LastName,
                Email = getUserDetail.Email,
                ProfileImage = getUserDetail.ProfileImage,
                PhoneNumber = getUserDetail.PhoneNumber,
                DisplayName = getUserDetail.DisplayName,
                UserType = Convert.ToInt16(getUserDetail.UserType),

                UpdatedOnUTC = getUserDetail.UpdatedOnUTC,
                //-- Escorts details
                EscortID = getEscortDetail.EscortID,
                Age = getEscortDetail.Age,
                Height = getEscortDetail.Height,
                Eyes = getEscortDetail.Eyes,
                Category = getEscortDetail.Category,
                BodyType = getEscortDetail.BodyType,
                Language = getEscortDetail.Language,
                Bio = getEscortDetail.Bio,
                SexualPreferences = getEscortDetail.SexualPreferences,
               Categories=getEscortDetail.Categories,
                BankAccountHolderName = getEscortDetail.BankAccountHolderName,
                BankAccountNumber = getEscortDetail.BankAccountNumber,
                BSBNumber = getEscortDetail.BSBNumber,
                Rates = getEscortDetail.Rates,
                AvailabilityCalendar = getEscortDetail.AvailabilityCalendar,
                TourLocation = getEscortDetail.TourLocation,
                BaseLocation = getEscortDetail.BaseLocation,
                EscortGallery = getEscortDetail.EscortGallery,
                Gender = getUserDetail.Gender,
                Country = getUserDetail.Country,
                CountryCode = getUserDetail.CountryCode,
                Proffered = getEscortDetail.Proffered,
                BankName = getEscortDetail.BankName,
                FaceBookUrl = getEscortDetail.FaceBookUrl,
                TwitterUrl = getEscortDetail.TwitterUrl,
                LinkedinUrl = getEscortDetail.LinkedinUrl,
                YouTubeUrl = getEscortDetail.YouTubeUrl,
                TikTokUrl = getEscortDetail.TikTokUrl,
                SnapChatUrl = getEscortDetail.SnapChatUrl,
                InstagramUrl = getEscortDetail.InstagramUrl,
                AvailablityNotes = getEscortDetail.AvailablityNotes,
                RatesNotes = getEscortDetail.RatesNotes,
                HairColor = getEscortDetail.HairColor,
                Weight = getEscortDetail.Weight,
                Ethnicity = getEscortDetail.Ethnicity,
                Dress = getEscortDetail.Dress,
                Bust = getEscortDetail.Bust,
                IsPhotoVerified = getEscortDetail.IsPhotoVerified,
                EscortSubscription= getEscortDetail.EscortSubscription,
                IsPaused = getEscortDetail.IsPaused,
                IsApprove = getEscortDetail.IsApprove
            };

            //userDetailsDto.ProfileImage = CommonFunctions.GetRelativeFilePath(userDetailsDto.ProfileImage, Constants.UserImageFolderPath, Constants.DefaultUserPng);
            return new ApiResponse<EscortDetailDto>(userDetailsDto, message: ResourceString.GetUserDetails, apiName: "GetEscortProfileDetails");
        }

        public async Task<ApiResponse<Tuple<EscortDetailDto?, EscortRules?>>> GetEscortFullProfileDetails(int userId, int clientId)
        {
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);

            if (getUserDetail is null)
            {
                return new ApiResponse<Tuple<EscortDetailDto?, EscortRules?>>(new Tuple<EscortDetailDto?, EscortRules?>(null, null), message: ResourceString.UserDetailsNotFound, apiName: "GetEscortProfileDetails");
            }

            var getEscortDetail = await _profileRepository.GetEscortDetailsById(userId);

            EscortDetailDto userDetailsDto = new EscortDetailDto
            {
                //-- User details
                UserId = userId,
                FirstName = getUserDetail.FirstName,
                LastName = getUserDetail.LastName,
                Email = getUserDetail.Email,
                ProfileImage = getUserDetail.ProfileImage,
                PhoneNumber = getUserDetail.PhoneNumber,
                DisplayName = getUserDetail.DisplayName,
                UserType = Convert.ToInt16(getUserDetail.UserType),

                UpdatedOnUTC = getUserDetail.UpdatedOnUTC,
                //-- Escorts details
                EscortID = getEscortDetail.EscortID,
                Age = getEscortDetail.Age,
                Height = getEscortDetail.Height,
                Eyes = getEscortDetail.Eyes,
                Category = getEscortDetail.Category,
                BodyType = getEscortDetail.BodyType,
                Language = getEscortDetail.Language,
                Bio = getEscortDetail.Bio,
                SexualPreferences = getEscortDetail.SexualPreferences,
                BankAccountHolderName = getEscortDetail.BankAccountHolderName,
                BankAccountNumber = getEscortDetail.BankAccountNumber,
                BSBNumber = getEscortDetail.BSBNumber,
                Rates = getEscortDetail.Rates,
                AvailabilityCalendar = getEscortDetail.AvailabilityCalendar,
                TourLocation = getEscortDetail.TourLocation,
                BaseLocation = getEscortDetail.BaseLocation,
                EscortGallery = getEscortDetail.EscortGallery,
                Gender = getUserDetail.Gender,
                Country = getUserDetail.Country,
                CountryCode = getUserDetail.CountryCode,
                Proffered = getEscortDetail.Proffered,
                BankName = getEscortDetail.BankName,
                FaceBookUrl = getEscortDetail.FaceBookUrl,
                TwitterUrl = getEscortDetail.TwitterUrl,
                LinkedinUrl = getEscortDetail.LinkedinUrl,
                YouTubeUrl = getEscortDetail.YouTubeUrl,
                TikTokUrl = getEscortDetail.TikTokUrl,
                SnapChatUrl = getEscortDetail.SnapChatUrl,
                InstagramUrl = getEscortDetail.InstagramUrl,
                AvailablityNotes = getEscortDetail.AvailablityNotes,
                RatesNotes = getEscortDetail.RatesNotes,
                HairColor = getEscortDetail.HairColor,
                Weight = getEscortDetail.Weight,
                Ethnicity = getEscortDetail.Ethnicity,
                Dress = getEscortDetail.Dress,
                Bust = getEscortDetail.Bust,
                EscortSubscription = getEscortDetail.EscortSubscription,
                IsPhotoVerified = getEscortDetail.IsPhotoVerified,
                CategoriesNames = getEscortDetail.CategoriesNames,
                TotalViews = getEscortDetail.TotalViews,
                TotalFavoriteMarkByUsers = getEscortDetail.TotalFavoriteMarkByUsers,
                 IsApprove = getEscortDetail.IsApprove,
                 IsPaused = getEscortDetail.IsPaused
            };

            //userDetailsDto.ProfileImage = CommonFunctions.GetRelativeFilePath(userDetailsDto.ProfileImage, Constants.UserImageFolderPath, Constants.DefaultUserPng);
            var subscriptionPlanId = userDetailsDto.EscortSubscription?.SubscriptionPlanId;
            var userType = userDetailsDto.EscortSubscription?.UserType;
            var escortViewRules = new EscortRules(userType, subscriptionPlanId);
            userDetailsDto = RemapModelAsPerSubscription(userDetailsDto, escortViewRules);

            if(clientId > 0 && LoginMemberSession.UserDetailSession?.UserTypeId == (int)UserTypes.Client)
            {
                await _profileRepository.UpdateExcortViewCount(getEscortDetail.EscortID, clientId);
            }

            return new ApiResponse<Tuple<EscortDetailDto?, EscortRules?>>(new Tuple<EscortDetailDto?, EscortRules?>(userDetailsDto, escortViewRules), message: ResourceString.GetUserDetails, apiName: "GetEscortProfileDetails");
        }


   

        private EscortDetailDto RemapModelAsPerSubscription(EscortDetailDto userDetailsDto, EscortRules escortViewRules)
        {
            userDetailsDto.BaseLocation = userDetailsDto.BaseLocation.Take(escortViewRules.AllowedBaseLocations).ToList();
            userDetailsDto.TourLocation = userDetailsDto.TourLocation.Take(escortViewRules.AllowedTourLocations).ToList();
            //userDetailsDto.VideoUrls = userDetailsDto.EscortGallery.Take(escortViewRules?.AllowedPhotoGallery ?? 0).ToList();
            return userDetailsDto;
        }

        public async Task<ApiResponse<EscortDetailDto>> GetEscortFullProfileDetails(int userId)
        {
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);

            if (getUserDetail is null)
            {
                return new ApiResponse<EscortDetailDto>(null, message: ResourceString.UserDetailsNotFound, apiName: "GetEscortProfileDetails");
            }

            var getEscortDetail = await _profileRepository.GetEscortDetailsById(userId);

            EscortDetailDto userDetailsDto = new EscortDetailDto
            {
                //-- User details
                UserId = userId,
                FirstName = getUserDetail.FirstName,
                LastName = getUserDetail.LastName,
                Email = getUserDetail.Email,
                ProfileImage = getUserDetail.ProfileImage,
                PhoneNumber = getUserDetail.PhoneNumber,
                DisplayName = getUserDetail.DisplayName,
                UserType = Convert.ToInt16(getUserDetail.UserType),

                UpdatedOnUTC = getUserDetail.UpdatedOnUTC,
                //-- Escorts details
                EscortID = getEscortDetail.EscortID,
                Age = getEscortDetail.Age,
                Height = getEscortDetail.Height,
                Eyes = getEscortDetail.Eyes,
                Category = getEscortDetail.Category,
                BodyType = getEscortDetail.BodyType,
                Language = getEscortDetail.Language,
                Bio = getEscortDetail.Bio,
                SexualPreferences = getEscortDetail.SexualPreferences,
                BankAccountHolderName = getEscortDetail.BankAccountHolderName,
                BankAccountNumber = getEscortDetail.BankAccountNumber,
                BSBNumber = getEscortDetail.BSBNumber,
                Rates = getEscortDetail.Rates,
                AvailabilityCalendar = getEscortDetail.AvailabilityCalendar,
                TourLocation = getEscortDetail.TourLocation,
                BaseLocation = getEscortDetail.BaseLocation,
                EscortGallery = getEscortDetail.EscortGallery,
                Gender = getUserDetail.Gender,
                Country = getUserDetail.Country,
                CountryCode = getUserDetail.CountryCode,
                Proffered = getEscortDetail.Proffered,
                BankName = getEscortDetail.BankName,
                FaceBookUrl = getEscortDetail.FaceBookUrl,
                TwitterUrl = getEscortDetail.TwitterUrl,
                LinkedinUrl = getEscortDetail.LinkedinUrl,
                YouTubeUrl = getEscortDetail.YouTubeUrl,
                TikTokUrl = getEscortDetail.TikTokUrl,
                SnapChatUrl = getEscortDetail.SnapChatUrl,
                InstagramUrl = getEscortDetail.InstagramUrl,
                AvailablityNotes = getEscortDetail.AvailablityNotes,
                RatesNotes = getEscortDetail.RatesNotes,
                HairColor = getEscortDetail.HairColor,
                Weight = getEscortDetail.Weight,
                Ethnicity = getEscortDetail.Ethnicity,
                Dress = getEscortDetail.Dress,
                Bust = getEscortDetail.Bust,
                EscortSubscription = getEscortDetail.EscortSubscription,
                IsPhotoVerified = getEscortDetail.IsPhotoVerified,
                CategoriesNames = getEscortDetail.CategoriesNames,
                TotalViews = getEscortDetail.TotalViews,
                TotalFavoriteMarkByUsers = getEscortDetail.TotalFavoriteMarkByUsers,
                IsApprove = getEscortDetail.IsApprove,
                IsPaused = getEscortDetail.IsPaused
            };

            
            return new ApiResponse<EscortDetailDto>(userDetailsDto, message: ResourceString.GetUserDetails, apiName: "GetEscortProfileDetails");
        }


        public async Task<ApiResponse<bool>> UpdateProfile(UpdateProfileRequest profileRequest, int userId)
        {
            int updateUser = await _accountRepository.AddUpdateAsync(
                new UserDetail()
                {
                    Id = userId,
                    FirstName = profileRequest.FirstName,
                    LastName = profileRequest.LastName,
                    PhoneNumber = profileRequest.PhoneNumber,
                });

            if (updateUser <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UpdateNotProfile, apiName: "UpdateProfile");
            }

            return new ApiResponse<bool>(true, message: ResourceString.UpdateProfile, apiName: "UpdateProfile");
        }
        public async Task<ApiResponse<bool>> MarkFavorites(int escort, int userId)
        {
            int result = await _profileRepository.MarkFavorites(escort, userId);
            if (result <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UnMarkFavoritesSuccess, apiName: "MarkFavorites");
            }

            return new ApiResponse<bool>(true, message: ResourceString.MarkFavoritesSuccess, apiName: "MarkFavorites");
        }


        public async Task<ApiResponse<bool>> DeleteProfile(int userId)
        {
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);
            if (getUserDetail is null)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserDetailsNotFound, apiName: "DeleteProfile");
            }
            int updateUser = await _accountRepository.AddUpdateAsync(
                new UserDetail()
                {
                    Id = userId,
                    //FirstName = "deleted",
                    //LastName = "deleted",
                    PhoneNumber = "deleted",
                    Email = "deleted",
                    IsDeleted = true,
                    AccessToken = "deleted"
                });

            if (updateUser <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.ProfileNotDeleted, apiName: "DeleteProfile");
            }

            return new ApiResponse<bool>(true, message: ResourceString.ProfileDeleted, apiName: "DeleteProfile");
        }


        public async Task<ApiResponse<int>> EditBankDetails(BankDetails requestModel)
        {
            ApiResponse<int> response = new();
            var updateUser = await _profileRepository.EditBankDetails(requestModel);

            if (updateUser > 0)
            {
                response.Message = ResourceString.BankDetailUpdateSuccess;
                response.Data = updateUser;
            }
            else
            {
                response.Message = ResourceString.BankDetailUpdateFailed;
                response.Data = 0;
            }
            return response;
        }
        public async Task<ApiResponse<int>> Edit(EscortDetailDto requestModel)
        {
            ApiResponse<int> response = new();
            string newImageName = string.Empty;

            var getUserDetail = await _accountRepository.GetByIdAsync(requestModel.UserId);
            if (getUserDetail is null)
            {
                return new ApiResponse<int>(0, message: ResourceString.UserDetailsNotFound, apiName: "Edit");
            }

            if (requestModel.CroppedProfileFile != null)
            {
                if (requestModel.CroppedProfileFile != "")
                {
                    newImageName = await _fileStorageService.UploadFileByBase64(requestModel.CroppedProfileFile, Constants.ProfileImage, "images");
                }

            }

            if (requestModel == null)
            {
                response.Message = ResourceString.ProfileUpdateFailed;
                response.Data = 0;
                return response;
            }
            else
            {
                if (!string.IsNullOrEmpty(newImageName))
                {
                    requestModel.ProfileImage = newImageName;
                }
                else
                {
                    requestModel.ProfileImage = getUserDetail.ProfileImage;
                }

                var updateUser = await _profileRepository.Edit(requestModel);

                if (updateUser > 0)
                {
                    response.Message = ResourceString.ProfileUpdateSuccess;
                    response.Data = updateUser;
                }
                else
                {
                    response.Message = ResourceString.ProfileUpdateFailed;
                    response.Data = 0;
                }
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UploadImageFiles(EscortsFileDto requestModel)
        {
            ApiResponse<bool> response = new();
            if (requestModel != null)
            {
                string newImageName = string.Empty;
                if (requestModel.ListImages != null)
                {

                    foreach (var item in requestModel.ListImages)
                    {
                        var fileNameList = await _fileStorageService.UploadFileByBase64(item, Constants.UserImages, "images");
                        var galleryModel = new EscortGallery();
                        galleryModel.EscortId = requestModel.EscortID;
                        galleryModel.FileName = fileNameList;
                        galleryModel.MediaType = (byte)MediaTypes.Image;
                        await _profileRepository.SaveImagesAndVideoUrls(galleryModel);
                    }
                    response.Message = ResourceString.Success;
                    response.Data = true;
                }
            }
            else
            {
                response.Message = ResourceString.Error;
                response.Data = false;
            }
            return response;
        }


        public async Task<ApiResponse<bool>> DeleteImage(string fileName)
        {
            ApiResponse<bool> response = new();
            if (fileName != null)
            {
                await _fileStorageService.DeleteFile(fileName);
                _ = _profileRepository.DeleteImages(fileName);
                response.Message = ResourceString.Success;
            }
            else
            {
                response.Message = ResourceString.Error;
                response.Data = false;
            }
            return response;

        }
        public async Task<ApiResponse<bool>> UploadVideoFiles(EscortsFileDto requestModel)
        {
            ApiResponse<bool> response = new();
            if (requestModel.VideoName != null)
            {
                //var newVideoName = await _fileStorageService.UploadFile(requestModel.Videos, Constants.UserVideos, "videos");
                var galleryModel = new EscortGallery();
                galleryModel.EscortId = requestModel.EscortID;
                galleryModel.FileName = requestModel.VideoName;
                galleryModel.MediaType = (byte)MediaTypes.Video;
                await _profileRepository.SaveImagesAndVideoUrls(galleryModel);
            }
            else
            {
                response.Message = ResourceString.Error;
                response.Data = false;
            }
            return response;
        }

        public async Task<int> IsHasAccess(int userId, int establishmentId)
        {
            return await _profileRepository.IsHasAccess(userId, establishmentId);

        }

        public async Task<int> GetUserIdByEscortId(int escortId)
        {
            return await _profileRepository.GetUserIdByEscortId(escortId);
        }

      
    }
}
