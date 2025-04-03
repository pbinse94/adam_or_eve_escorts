using Amazon.S3.Model;
using Amazon.S3;
using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Resources;
using System.Collections.Generic;
using System.Reflection;
using Web.Controllers.Base;
using Amazon;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Shared.Model.Request.Profile;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.AspNetCore.Authentication;

namespace Web.Controllers
{
    [Route("profile")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ProfileController : UserBaseController
    {
        private readonly IManageService _manageService;
        private readonly ICommonService _commonService;
        private readonly IProfileService _profileService;
        private readonly IAccountService _accountService;
        private readonly AwsKeys _awsKeys;
        private readonly ISubscriptionPlanService _subscriptionPlanService;

        public ProfileController(IAccountService accountService, IManageService manageService, ICommonService commonService, IProfileService profileService, IOptions<AwsKeys> awsKeys, ISubscriptionPlanService subscriptionPlanService)
        {
            _manageService = manageService;
            _commonService = commonService;
            _profileService = profileService;
            _accountService = accountService;
            _awsKeys = awsKeys.Value;
            _subscriptionPlanService = subscriptionPlanService;
        }
        public IActionResult Index()
        {
            return View();
        }




        [Route("profileDetails")]
        [HttpGet]
        public async Task<ActionResult> ProfileDetails()
        {
            var response = await _profileService.GetEscortProfileDetails(UserId);
            var escortDetail = new EscortDetailDto();

            if (response.Data != null)
            {
                escortDetail = response.Data;
            }
            return View(escortDetail);
        }

        [Route("profileDetails")]
        [HttpPost]
        public async Task<IActionResult> ProfileDetails(BankDetails requestModel)
        {
            if (requestModel.UserId > 0)
            {
                var updateUser = await _profileService.EditBankDetails(requestModel);
                return Json(updateUser);
            }
            else
            {
                return BadRequest(new ApiResponse<int> { Data = 0, Message = ResourceString.BankDetailUpdateFailed });
            }
        }

        [AllowAnonymous]
        [Route("fullProfile")]
        [HttpGet]
        public async Task<ActionResult> FullProfile(int? id)
        {

            var response = await _profileService.GetEscortFullProfileDetails((int)(id == null ? UserId : id), UserId);

            var escortDetail = new EscortDetailDto();
            var escortRule = new EscortRules(null,null);


            if (response.Data != null && response.Data.Item1 != null && response.Data.Item2 != null)
            {
                escortDetail = response.Data.Item1;
                escortDetail.EscortGallery = escortDetail.EscortGallery.OrderBy(item => item.MediaType != 1)
                             .ToList();
                escortRule = response.Data.Item2;
            }

            return View(new Tuple<EscortDetailDto, EscortRules>(escortDetail, escortRule));
        }



        [Route("edit")]
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            var escortDetail = new EscortDetailDto();
            if (id != 0)
            {
                if (id > 0) {
                    var results = await _profileService.IsHasAccess((int)id, UserId);
                    if (results != id)
                    {
                        return  RedirectToAction("Logout", "Account", new { area = "" });
                    }
                }

                var res = await _profileService.GetEscortProfileDetails((int)(id == null ? UserId : id));
                if (res.Data != null)
                {
                    escortDetail = res.Data;
                }
            }

            if (escortDetail.Rates.Count == 0)
            {
                escortDetail.Rates.Add(new EscortRatesDto { Duration = null });
            }

            if (escortDetail.AvailabilityCalendar.Count == 0)
            {
                escortDetail.AvailabilityCalendar = Enum.GetValues(typeof(DayOfWeekType)).Cast<DayOfWeekType>().Select(dayOfWeek => new AvailabilityCalendarDto
                {
                    DayNumber = (byte)dayOfWeek,
                    IsNotAvailable = false,
                    IsAvailable24X7 = true
                }).ToList();
            }


            var language = EnumExtensions.EnumValuesAndDescriptionsToList<LanguageTypes>();
            // var category = EnumExtensions.EnumValuesAndDescriptionsToList<CategoryTypes>();
            // var category = _commonService.GetCategory();
            var categories = _commonService.GetCategories().Result;

            var sexualPreferences = EnumExtensions.EnumValuesAndDescriptionsToList<SexualPreferencesTypes>();
            var durations = EnumExtensions.EnumValuesAndDescriptionsToList<DurationTypes>();

            // Insert default value
            durations.Insert(0, new() { Text = "Select duration...", Value = "" });


            ViewBag.Language = language;
            // ViewBag.Category = category;
            //  ViewBag.categories = categories;

            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryID.ToString(),
                Text = c.CategoryName
            }).ToList();

            ViewBag.SexualPreferences = sexualPreferences;
            ViewBag.Duration = durations;

            //-- Age - 18 to 60
            ViewBag.Age = Enumerable.Range(18, 43).Select(x => new SelectListItem
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = false
            }).ToList();

            //-- Height - 106 to 214 cm
            ViewBag.Height = Enumerable.Range(106, 109).Select(x => new SelectListItem
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = false
            }).ToList();
            var countryCodes = await _accountService.GetCountryCodes();
            ViewBag.SelectListItems = countryCodes.Select(x => new SelectListItem() { Text = x.CountryName, Value = x.CountryName }).ToList();

            return View(escortDetail);
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(EscortDetailDto requestModel)
        {
            if (requestModel.UserId == 0)
            {
                SubscriptionPlanDurationType? establishSubscriptionPlan = null;
                if (LoginMemberSession.UserDetailSession?.SubscriptionPlanId != null && Enum.IsDefined(typeof(SubscriptionPlanDurationType), (int)LoginMemberSession.UserDetailSession.SubscriptionPlanId))
                {
                    // Safe cast
                    establishSubscriptionPlan =
                        (SubscriptionPlanDurationType)LoginMemberSession.UserDetailSession.SubscriptionPlanId;

                    // Use establishSubscriptionPlan as needed
                }

                if (establishSubscriptionPlan == null)
                {
                    return BadRequest(new ApiResponse<int> { Data = 0, Message = ResourceString.EstablishmentSubscriptionNotExists });
                }

                var escortRules = new EscortRules(UserTypes.Establishment, establishSubscriptionPlan);

                var establishmentEscortCounts = await _accountService.GetEstablishmentEscortsCount(UserId);

                if (escortRules.CanContainEscorts <= establishmentEscortCounts)
                {
                    return BadRequest(new ApiResponse<int> { Data = 0, Message = ResourceString.EstablishmentEscortLimitExceeded });
                }

                RegistrationRequest request = new RegistrationRequest
                {
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    DisplayName = requestModel.DisplayName,
                    Email = requestModel.Email,
                    Country = requestModel.Country,
                    CountryCode = requestModel.CountryCode,
                    Gender = requestModel.Gender,
                    UserType = UserTypes.EstablishmentEscort,

                };
                if (requestModel.PhoneNumber != null)
                {
                    request.PhoneNumber = requestModel.PhoneNumber;
                }

                var registrationResponse = await _accountService.SignUp(request);
                if (registrationResponse?.Data?.UserId == 0)
                {
                    return Json(new ApiResponse<int> { Data = registrationResponse.Data.UserId, Message = registrationResponse.Message });                   
                }
                requestModel.UserId = registrationResponse?.Data?.UserId ?? 0;
            }

            if (requestModel.UserId > 0)
            {
                requestModel.UpdatedBy = UserId;
                requestModel.CroppedProfileFile = Request.Form["CroppedProfileFile"].ToString();
                var updateUser = await _profileService.Edit(requestModel);
                var res = await _profileService.GetEscortProfileDetails(UserId);
                if (res.Data != null && LoginMemberSession.UserDetailSession != null)
                {
                    LoginSessionModel sessionobj = LoginMemberSession.UserDetailSession;

                    sessionobj.UserId = res.Data.UserId;
                    sessionobj.FirstName = res.Data.FirstName;
                    sessionobj.LastName = res.Data.LastName;
                    sessionobj.DisplayName = res.Data.DisplayName;
                    sessionobj.UserTypeId = res.Data.UserType;
                    sessionobj.EmailId = res.Data.Email;
                    sessionobj.ProfileImage = res.Data.ProfileImage;

                    LoginMemberSession.UserDetailSession = sessionobj;
                }
                var responseMessage = updateUser.Message;
                var responseEscortId = updateUser.Data;
                return Json(new { message = responseMessage, escortId = responseEscortId, userId = requestModel.UserId });
            }
            else
            {
                return BadRequest(new ApiResponse<int> { Data = 0, Message = ResourceString.ProfileUpdateFailed });
            }
        }

        [Route("uploadImages")]
        [HttpPost]
        public async Task<IActionResult> UploadImages(EscortsFileDto requestModel)
        {
            var res = await _profileService.UploadImageFiles(requestModel);
            if (res.Data)
            {
                var userId = await _profileService.GetUserIdByEscortId(requestModel.EscortID);
                var result = await _profileService.GetEscortProfileDetails(requestModel.UserId == 0 ? userId : requestModel.UserId);
                if (result.Data != null)
                {
                    return View("_ImageUpload", result.Data);
                }
            }
            return Json(res);
        }

        [Route("uploadVideos")]
        [HttpPost]
        public IActionResult UploadVideos(EscortsFileDto requestModel)
        {
            var res = _profileService.UploadVideoFiles(requestModel);
            return Json(res);
        }

        [Route("getCities")]
        [HttpGet]
        public async Task<IActionResult> GetCities(int stateId, int cityId = 0)
        {
            var cityList = (await _commonService.GetCity(stateId: stateId, cityId: cityId > 0 ? cityId : null)).Select(x => new SelectListItem
            {
                Text = x.CityName.ToString(),
                Value = x.CityID.ToString(),
                Selected = false
            }).ToList();

            cityList.Insert(0, new() { Text = "Select city...", Value = "" });

            return Json(cityList);
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> DeleteImages(string fileName)
        {
            await _profileService.DeleteImage(fileName);
            return Ok(new ApiResponse<bool> { Message = ResourceString.PasswordUpdated });
        }

        [Route("DeleteAccount")]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            if (UserId <= 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var cancelStatus = await _subscriptionPlanService.CancelUserSubscriptionByUser(UserId, 0);

            if (!cancelStatus.Data)
            {
                var activeSubscription = await _subscriptionPlanService.GetUserSubscriptionDetail(UserId);

                if (activeSubscription != null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ApiResponse<bool>(false, message: ResourceString.ProfileCanNotDeleteDueToActiveSubscription));
                }
            }


            var updateUserProfile = await _profileService.DeleteProfile(UserId);
            if (updateUserProfile.Data)
            {
                await HttpContext.SignOutAsync();
                HttpContext.Session.Clear();
                return StatusCode(StatusCodes.Status200OK, updateUserProfile);
            }
            return StatusCode(StatusCodes.Status404NotFound, updateUserProfile);
        }


        [Route("PauseAccount")]
        [HttpPost]
        public async Task<IActionResult> PauseAccount(bool isPause)
        {
            if (UserId <= 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var updateUserProfile = await _accountService.PauseEscort(UserId, isPause);
            if (updateUserProfile.Data)
            {
                return StatusCode(StatusCodes.Status200OK, updateUserProfile);
            }
            return StatusCode(StatusCodes.Status404NotFound, updateUserProfile);
        }

        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            ChangePasswordModel model = new();
            return View(model);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var changePasswordResponse = await _manageService.ChangePassword(model, UserId);
            if (changePasswordResponse == ResponseTypes.Success)
            {
                return Ok(new ApiResponse<bool> { Message = ResourceString.PasswordUpdated });
            }
            else if (changePasswordResponse == ResponseTypes.OldPasswordWrong)
            {
                return BadRequest(new ApiResponse<bool> { Message = ResourceString.WrongOldPassword });
            }
            else if (changePasswordResponse == ResponseTypes.OldNewPasswordMatched)
            {
                return BadRequest(new ApiResponse<bool> { Message = ResourceString.OldNewPasswordNotSame });
            }
            else
            {
                return BadRequest(new ApiResponse<bool> { Message = ResourceString.FailedToSetNewPassword });
            }
        }




        // For Video

        [Route("initiatemultipartupload")]
        [HttpGet]
        public IActionResult InitiateMultipartUpload(string bucketName, string objectKey)
        {
            try
            {
                var client = new AmazonS3Client(_awsKeys.AwsAccessKeyId, _awsKeys.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
                var initiateRequest = new InitiateMultipartUploadRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                var response = client.InitiateMultipartUploadAsync(initiateRequest).Result;

                return Ok(new { UploadId = response.UploadId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error initiating multipart upload: {ex.Message}");
            }
        }
        [Route("getpresignedurlforpart")]
        [HttpGet]
        public IActionResult GetPresignedUrlForPart(string bucketName, string objectKey, string uploadId, int partNumber)
        {
            try
            {
                var client = new AmazonS3Client(_awsKeys.AwsAccessKeyId, _awsKeys.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    UploadId = uploadId,
                    PartNumber = partNumber,
                    Expires = DateTime.UtcNow.AddMinutes(5), // Set expiration time for the URL
                    Verb = HttpVerb.PUT // Use PUT verb for upload
                };

                var presignedUrl = client.GetPreSignedURL(request);
                return Ok(new { PresignedUrl = presignedUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating pre-signed URL for part {partNumber}: {ex.Message}");
            }
        }
        [Route("completemultipartupload")]
        [HttpPost]
        public IActionResult CompleteMultipartUpload(string bucketName, string objectKey, string uploadId, string partETags)
        {
            try
            {
                List<PartETag> partETagsresult = (from part in JsonConvert.DeserializeObject<List<PartInfo>>(partETags)
                                                  select new PartETag { PartNumber = part.PartNumber, ETag = part.ETag }).ToList();
                var client = new AmazonS3Client(_awsKeys.AwsAccessKeyId, _awsKeys.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
                var completeRequest = new CompleteMultipartUploadRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    UploadId = uploadId,
                    PartETags = partETagsresult
                };

                var response = client.CompleteMultipartUploadAsync(completeRequest).Result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error completing multipart upload: {ex.Message}");
            }
        }


        [Route("markFavorites")]
        [HttpPost]
        public async Task<IActionResult> MarkFavorites(int escortId)
        {
            var updateUser = await _profileService.MarkFavorites(escortId, UserId);

            return Json(updateUser);
        }


        [Route("largefilecompletemultipartupload")]
        [HttpPost]
        public async Task<IActionResult> LargeFileCompleteMultipartUpload(string bucketName, string objectKey, string uploadId, string partETags, string escortID, string userId)
        {

            List<PartETag> partETagsresult = (from part in JsonConvert.DeserializeObject<List<PartInfo>>(partETags)
                                              select new PartETag { PartNumber = part.PartNumber, ETag = part.ETag }).ToList();

            //var Result = await _accountService.LargeFileCompleteMultipartUpload(bucketName, objectKey, uploadId);
            try
            {


                var client = new AmazonS3Client(_awsKeys.AwsAccessKeyId, _awsKeys.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
                var completeRequest = new CompleteMultipartUploadRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    UploadId = uploadId,
                    PartETags = partETagsresult
                };

                var response = await client.CompleteMultipartUploadAsync(completeRequest);


                EscortsFileDto requestModel = new EscortsFileDto
                {
                    VideoName = objectKey,
                    EscortID = Convert.ToInt32(escortID),
                    UserId = Convert.ToInt32(userId),
                };

                var res = await _profileService.UploadVideoFiles(requestModel);
                //UploadfileResponseModelWithLocalPath objUploadfileResponseModelWithLocalPath = await AWS.CovertMp4VideoTriggertranscoding(objectKey, bucketName, ImageVideo, uploadingtype);
                //if (objUploadfileResponseModelWithLocalPath == null)
                //{
                //    objUploadfileResponseModelWithLocalPath = new UploadfileResponseModelWithLocalPath();
                //}
                var userIdold = await _profileService.GetUserIdByEscortId(requestModel.EscortID);
                var result = await _profileService.GetEscortProfileDetails(requestModel.UserId == 0 ? userIdold : requestModel.UserId);
                if (result.Data != null)
                {
                    return View("_VideoUpload", result.Data);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error completing multipart upload: {ex.Message}");
            }
        }

        [Route("verifyUser")]
        [HttpPost]
        public async Task<IActionResult> VerifyUser([FromForm] string capturedImage, [FromForm] string referenceImage)
        {
            if (string.IsNullOrEmpty(capturedImage) || string.IsNullOrEmpty(referenceImage))
            {
                return Json(new { success = false, message = "Image data is missing." });
            }

            try
            {
                // Convert the Base64 strings to byte arrays
                byte[] capturedImageBytes = Convert.FromBase64String(capturedImage.Split(',')[1]);
                byte[] referenceImageBytes = Convert.FromBase64String(referenceImage.Split(',')[1]);

                // Compare the images using AWS Rekognition
                bool isIdentityVerified = await CompareImagesWithRekognition(capturedImageBytes, referenceImageBytes);

                if (isIdentityVerified)
                {
                    return Json(new { success = true, verified = true, message = "The profile image has been successfully verified. Kindly save your profile." });
                }
                else
                {
                    return Json(new { success = true, verified = false, message = "The profile image does not verified with your current image. Please retry." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private async Task<bool> CompareImagesWithRekognition(byte[] capturedImageBytes, byte[] referenceImageBytes)
        {
            var rekognitionClient = new AmazonRekognitionClient(_awsKeys.AwsAccessKeyId, _awsKeys.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);

            var compareFacesRequest = new CompareFacesRequest
            {
                SourceImage = new Image { Bytes = new MemoryStream(referenceImageBytes) },
                TargetImage = new Image { Bytes = new MemoryStream(capturedImageBytes) },
                SimilarityThreshold = 90F
            };

            try
            {
                var compareFacesResponse = await rekognitionClient.CompareFacesAsync(compareFacesRequest);
                if (compareFacesResponse.FaceMatches.Count > 0)
                {
                    var similarity = compareFacesResponse.FaceMatches[0].Similarity;
                    return similarity >= 90;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Handle errors (e.g., logging)
                return false;
            }
        }



    }
}
