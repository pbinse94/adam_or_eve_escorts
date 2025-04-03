using Amazon.IVS;
using Amazon.Ivschat.Model;
using Amazon.Ivschat;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Web.Controllers.Base;
using Amazon;
using Microsoft.Extensions.Options;
using Business.Services;
using Business.IServices;
using Shared.Model.Request.WebUser;
using FirebaseAdmin.Messaging;
using Shared.Model.Entities;
using Amazon.Runtime;
using Shared.Common.Enums;

namespace Web.Controllers
{
    [Route("chatauth")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ChatAuthController : UserBaseController
    {
        private readonly ILogger<ChatAuthController> _logger;


        private readonly AwsKeys _awsConfig;
        private readonly IUserService _userService;
        private readonly AmazonIvschatClient _ivsChatClient;

        public ChatAuthController(ILogger<ChatAuthController> logger, IOptions<AwsKeys> awsConfig, IUserService userService)
        {
            _logger = logger;

            _awsConfig = awsConfig.Value;
            _userService = userService;
            _ivsChatClient = new AmazonIvschatClient(_awsConfig.AwsAccessKeyId, _awsConfig.AwsSecretAccessKey, RegionEndpoint.APSoutheast2);
        }


        [Route("getchatToken")]
        [HttpPost]
        public async Task<IActionResult> GetChatToken(ChatTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RoomIdentifier) || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { error = "Missing parameters: `arn or roomIdentifier`, `userId`" });
            }

            var param = new CreateChatTokenRequest
            {
                RoomIdentifier = request.RoomIdentifier,
                UserId = request.UserId,
                Attributes = request.Attributes ?? new System.Collections.Generic.Dictionary<string, string>(),
                Capabilities = request.Capabilities ?? new System.Collections.Generic.List<string>(),
                SessionDurationInMinutes = request.DurationInMinutes ?? 55
            };

            try
            {
                var data = await _ivsChatClient.CreateChatTokenAsync(param);
                _logger.LogInformation("Got data: {data}", data);
                return Ok(data);
            }
            catch (AmazonServiceException ex)
            {
                // Log the exception details
                Console.WriteLine($"AWS Service Exception: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat token");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendEvent([FromBody] ChatEventRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Arn) || string.IsNullOrEmpty(request.EventName))
            {
                return BadRequest(new { error = "Missing parameters: `arn`, `eventName`" });
            }

            var param = new SendEventRequest
            {
                RoomIdentifier = request.Arn,
                EventName = request.EventName,
                Attributes = request.EventAttributes ?? new System.Collections.Generic.Dictionary<string, string>()
            };

            try
            {
                await _ivsChatClient.SendEventAsync(param);
                _logger.LogInformation("chatEventHandler > IVSChat.sendEvent > Success");
                return Ok(new { arn = request.Arn, status = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending chat event");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Route("sendgift")]
        [HttpGet]
        public async Task<IActionResult> SendGift(int giftindex, int escortId)
        {
            SendGiftRequest sendGiftRequest = new()
            {
                EscortId = escortId,
                ClientId = UserId,
                Tokens = giftindex == 0 ? 0 : giftindex, // when message send then currently no token will be deduct 
                GiftIconID = giftindex,
                TransactionType = giftindex == 0 ? (short)GiftTransactionType.Message : (short)GiftTransactionType.Gift
            };

            try
            {
                var response = await _userService.SendGift(sendGiftRequest);
                return Ok(new { Data = response });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(204, new { Message = "Don't have sufficient credits" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListRooms()
        {
            try
            {
                var response = await _ivsChatClient.ListRoomsAsync(new ListRoomsRequest());
                _logger.LogInformation("chatListHandler > IVSChat.listRooms > Success");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing chat rooms");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }


}






