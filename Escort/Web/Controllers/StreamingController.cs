using Amazon;
using Amazon.IVS;
using Amazon.IVS.Model;
using Amazon.Ivschat;
using Amazon.Ivschat.Model;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Azure.Core;
using Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Escort;
using Shared.Model.Request.WebUser;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Channels;
using Web.Controllers.Base;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    [Route("streaming")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class StreamingController : UserBaseController
    {
        private readonly AmazonIVSClient _ivsClient;
        private readonly AwsKeys _awsConfig;
        private readonly IEscortServices _escortServices;
        private readonly IUserService _userService;
        private readonly IProfileService _profileService;
        public StreamingController(IOptions<AwsKeys> awsConfig, IEscortServices escortServices, IUserService userService,
            IProfileService profileService)
        {
            _awsConfig = awsConfig.Value;
            _ivsClient = new AmazonIVSClient(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
            _escortServices = escortServices;
            _userService = userService;
            _profileService = profileService;
        }

        [AllowAnonymous]
        [HttpGet("index")]
        public async Task<IActionResult> Index(string request)
        {
            if(LoginMemberSession.UserDetailSession != null && LoginMemberSession.UserDetailSession.UserTypeId != (short)UserTypes.Client)
            {
                return RedirectToAction("Index", "Home");
            }
            var publicipAddress = GetPublicIpAddress().Result;
            var isExistingUser = _userService.CheckAndInsertIPAddress(publicipAddress ?? "").Result;
            var decryptedData = EncryptDecryptExtensions.ToDecrypt(request);
            var parameters = JsonConvert.DeserializeObject<ReturnUrlParameters>(decryptedData);
            
            var viewModel = new ClientStreamingViewModel
            {
                IpAddress = isExistingUser,
                RoomId = parameters?.Arn,
                PlaybackUrl = parameters?.PlaybackUrl,
                UserName = LoginMemberSession.UserDetailSession?.DisplayName,
                ProfileImage = parameters?.ProfileImage,
                Id = parameters?.Id,
                Name = parameters?.Name
            };

            if(LoginMemberSession.UserDetailSession != null && LoginMemberSession.UserDetailSession.UserTypeId == (short)UserTypes.Client)
            {
                var userDetail = await _profileService.GetUserDetails(LoginMemberSession.UserDetailSession.UserId);
                viewModel.CreditBalance = userDetail.Data?.CreditBalance ?? 0;
                await _escortServices.JoinLiveCam(UserId, Convert.ToInt32(viewModel.Id));
            }

            var htmlString = GenerateIconHtml(parameters?.Id);
            ViewBag.IconHtml = htmlString;

            return View(viewModel);
        }




        [HttpGet("startVideo")]
        public async Task<IActionResult> StartVideo()
        {
            if (LoginMemberSession.UserDetailSession?.UserTypeId != (int)UserTypes.IndependentEscort)
            {
                return RedirectToAction("Index", "Home");
            }

            if (LoginMemberSession.UserDetailSession?.SubscriptionPlanExpireDateTime < DateTime.UtcNow)
            {
                return RedirectToAction("Index", "Subscription");
            }

            var escortScore = await _escortServices.GetEscortScore(UserId);

            var response = new CreateChannelRequest
            {
                Name = UserId.ToString(),
                LatencyMode = "LOW",
                Type = "BASIC"
            };
            var ivschatClient = new AmazonIvschatClient(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
            var listChannelsResponse = await _ivsClient.ListChannelsAsync(new ListChannelsRequest());

            var channel = listChannelsResponse.Channels.FirstOrDefault(c => c.Name == UserId.ToString());
            if (channel == null)
            {
                var responseChannel = await _ivsClient.CreateChannelAsync(response);
                //need to save this in database
                var createRoomRequest = new CreateRoomRequest
                {
                    Name = UserId.ToString(), // Replace with your room name
                };


                var createRoomResponse = await ivschatClient.CreateRoomAsync(createRoomRequest);
                ViewBag.RoomId = createRoomResponse.Arn;
                ViewBag.profileImage = LoginMemberSession.UserDetailSession?.ProfileImage;
                ViewBag.escortName = LoginMemberSession.UserDetailSession?.DisplayName;
                ViewBag.escortScore = escortScore;
                await _escortServices.StartLiveCam(UserId);
                return View(responseChannel);
            }
            else
            {

                var responseClient = await ivschatClient.ListRoomsAsync(new ListRoomsRequest());
                // Find the room with the specified name
                var room = responseClient.Rooms.FirstOrDefault(r => r.Name == UserId.ToString());

                var viewModel = new StreamCheckModalViewModel
                {
                    ChannelArn = channel.Arn,
                    RoomArn = room?.Arn
                };
                
                await _escortServices.StopLiveCam(UserId);

                return PartialView("_StreamCheckModal", viewModel);
            }


        }

        //[HttpPost("StartLiveCam")]
        //public async Task<JsonResult> StartLiveCam()
        //{
        //    await _escortServices.StartLiveCam(UserId);
        //    return Json(new { status = (int)HttpStatusCode.OK });
        //}


        [HttpPost("stop")]
        public async Task<IActionResult> StopStream(string channelArn, string roomArn, bool forceDelete)
        {
            
            var stopStreamRequest = new StopStreamRequest
            {
                ChannelArn = channelArn
            };

            var deleteChannelRequest = new DeleteChannelRequest
            {
                Arn = channelArn
            };

            // Initialize IVS client
            var ivsClient = new AmazonIvschatClient(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);

            var deleteRoomRequest = new DeleteRoomRequest
            {
                Identifier = roomArn
            };

            try
            {
                var getStreamRequest = new GetStreamRequest
                {
                    ChannelArn = channelArn
                };
                var response = await _ivsClient.GetStreamAsync(getStreamRequest);

                // Check if the channel is live
                await _ivsClient.StopStreamAsync(stopStreamRequest);
                await Task.Delay(5000);
                await _ivsClient.DeleteChannelAsync(deleteChannelRequest);
                await ivsClient.DeleteRoomAsync(deleteRoomRequest);
            }
            catch (Amazon.IVS.Model.ChannelNotBroadcastingException)
            {
                await _ivsClient.DeleteChannelAsync(deleteChannelRequest);
                await ivsClient.DeleteRoomAsync(deleteRoomRequest);                
            }
            //catch(Exception ex)
            //{
            //    //Errlake.Crosscutting.ErrLog.LogInfo("Streaming", "StopStream", ex.ToString(), "");
            //}
            finally
            { 
                await _escortServices.StopLiveCam(UserId);
            }
            await Task.Delay(5000);
            return Ok(new { message = "Stream stopped successfully." });
        }
        public string GenerateIconHtml(int? userId)
        {
            var icons = Enum.GetValues(typeof(GiftIconType)).Cast<GiftIconType>();
            var htmlBuilder = new StringBuilder();


            int index = 1; // Start index from 1
            foreach (GiftIconType icon in Enum.GetValues(typeof(GiftIconType)))
            {
                int iconIndex = (int)icon;
                string iconName = icon.ToString();
                string displayName = CommonFunctions.GetDescription(icon);
                if (UserId == 0)
                {
                    htmlBuilder.AppendLine($"<a href=\"javascript:void(0)\" data-index=\"{iconIndex}\"  data-popup=\"{displayName} {iconIndex}\"  title=\"Index: {index}, Name: {iconName}, Value: {iconIndex} \" data-bs-toggle=\"modal\" data-bs-target=\"#login\"><img src=\"../assets/images/gifts/ico{index}.png\"></a>");
                }
                else
                {
                    htmlBuilder.AppendLine($"<a href=\"javascript:void(0)\" data-index=\"{iconIndex}\"  data-popup=\"{displayName} {iconIndex}\" onclick=\"giftsend(this,{iconIndex}, '{userId}','{iconName}')\" title=\"Index: {index}, Name: {iconName}, Value: {iconIndex}\"><img src=\"../assets/images/gifts/ico{index}.png\"></a>");
                }

                index++;
            }


            return htmlBuilder.ToString();
        }

        [AllowAnonymous]
        [HttpGet("LiveCams")]
        public async Task<IActionResult> LiveCams()
        {
            if (LoginMemberSession.UserDetailSession != null && LoginMemberSession.UserDetailSession.UserTypeId != (short)UserTypes.Client)
            {
                return RedirectToAction("Index", "Home");
            }
            List<LiveStreamInfo> liveStreams = new List<LiveStreamInfo>();
            var channels = await ListLiveChannelsAsync();
            if (channels != null)
            {
                List<RequestId> escortIds = channels.Select(x => new RequestId { Id = Convert.ToInt32(x.ChannelName) }).ToList();
                List<EscortSearchDto> featuredEscorts = await _escortServices.GetEscortsByIds(escortIds);

                liveStreams = CombineLiveChannelsWithEscorts(channels, featuredEscorts);
            }
            return View(liveStreams);
        }

        [AllowAnonymous]
        [HttpGet("GetHomeLiveStreams")]
        public async Task<PartialViewResult> GetHomeLiveStreams()
        {
            await Task.Delay(3000);
            List<LiveStreamInfo> liveStreams = new List<LiveStreamInfo>();
            var channels = await ListLiveChannelsAsync();
            if (channels != null)
            {
                List<RequestId> firstTwelveEscortIds = channels.Select(x => new RequestId { Id = Convert.ToInt32(x.ChannelName) }).Take(12).ToList();
                List<EscortSearchDto> featuredEscorts = await _escortServices.GetEscortsByIds(firstTwelveEscortIds);

                liveStreams = CombineLiveChannelsWithEscorts(channels, featuredEscorts);
            }
            return PartialView("_LiveCams", liveStreams);
        }


        private async Task<List<LiveStreamInfo>> ListLiveChannelsAsync()
        {
            var liveStreams = new List<LiveStreamInfo>();

            var listChannelsRequest = new ListChannelsRequest();
            var listChannelsResponse = await _ivsClient.ListChannelsAsync(listChannelsRequest);

            foreach (var channel in listChannelsResponse.Channels)
            {
                var listStreamKeysRequest = new ListStreamKeysRequest
                {
                    ChannelArn = channel.Arn
                };
                var listStreamKeysResponse = await _ivsClient.ListStreamKeysAsync(listStreamKeysRequest);

                foreach (var streamKey in listStreamKeysResponse.StreamKeys)
                {
                    var getStreamRequest = new GetStreamRequest
                    {
                        ChannelArn = channel.Arn
                    };
                    try
                    {
                        var ivschatClient = new AmazonIvschatClient(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
                        var getStreamResponse = await _ivsClient.GetStreamAsync(getStreamRequest);
                        var stream = getStreamResponse.Stream;
                        if (stream != null && stream.State == "LIVE")
                        {

                            var listRoomsResponse = await ivschatClient.ListRoomsAsync(new ListRoomsRequest());
                            var room = listRoomsResponse.Rooms.FirstOrDefault(rm => rm.Name == channel.Name);

                            liveStreams.Add(new LiveStreamInfo
                            {
                                ChannelName = channel.Name,
                                ChannelArn = room?.Arn,
                                StreamKey = getStreamResponse.Stream.StreamId,
                                ViewerCount = (int)stream.ViewerCount,
                                StartTime = stream.StartTime,
                                PlaybackUrl = stream.PlaybackUrl,

                            });
                        }

                    }
                    catch (Amazon.Ivschat.Model.ResourceNotFoundException)
                    {
                        // Stream is not live or does not exist, ignore and continue
                        continue;
                    }
                    catch (Exception ex)
                    {
                        // Handle other potential exceptions
                        Console.WriteLine($"Error getting stream for channel {channel.Name}: {ex.Message}");
                    }
                }
            }

            return liveStreams;
        }

        private static List<LiveStreamInfo> CombineLiveChannelsWithEscorts(List<LiveStreamInfo> liveChannels, List<EscortSearchDto> featuredEscorts)
        {
            var combinedDetails = new List<LiveStreamInfo>();

            foreach (var escort in featuredEscorts)
            {
                var matchingChannel = liveChannels.Find(channel => channel.ChannelName == escort.UserId.ToString());
                if (matchingChannel != null)
                {

                    ReturnUrlParameters parameters = new()
                    {
                        Arn = matchingChannel.ChannelArn,
                        Id = escort.EscortId,
                        Name = escort.DisplayName,
                        PlaybackUrl = matchingChannel.PlaybackUrl,
                        ProfileImage = escort.ProfileImage
                    };

                    var jsonString = JsonConvert.SerializeObject(parameters);
                    var encryptedData = EncryptDecryptExtensions.ToEncrypt(jsonString);
                    var encryptedUrl = $"/Streaming/Index?request={Uri.EscapeDataString(encryptedData)}";

                    combinedDetails.Add(new LiveStreamInfo
                    {
                        ChannelName = matchingChannel.ChannelName,
                        Profile = escort.ProfileImage,
                        ChannelArn = matchingChannel.ChannelArn,
                        ViewerCount = matchingChannel.ViewerCount,
                        StartTime = matchingChannel.StartTime,
                        PlaybackUrl = matchingChannel.PlaybackUrl,
                        DisplayName = escort.DisplayName,
                        EscortId = escort.EscortId,
                        Age = escort.Age,
                        ReturnUrl = encryptedUrl
                    });
                }
            }

            return combinedDetails;
        }

        public static async Task<string> GetPublicIpAddress()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Use a reliable external service to get your public IP
                    var response = await client.GetStringAsync("https://api.ipify.org");
                    return response.Trim();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, such as network errors
                Console.WriteLine($"Error getting public IP address: {ex.Message}");
                return "";
            }
        }

        [HttpGet("GetEscortScore")]
        public async Task<JsonResult> GetEscortScore()
        {
            int result = 0;
            result = await _escortServices.GetEscortScore(UserId);
            return Json(new { score = result });
        }
    }




}
