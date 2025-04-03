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
using Shared.Model.Escort;
using Shared.Model.Request.Admin;

namespace Web.Controllers
{
    [Route("establishment")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class EstablishmentController : UserBaseController
    {
        private readonly IEscortServices _escortServices;
        private readonly IAccountService _accountService;
        public EstablishmentController(IEscortServices escortServices, IAccountService accountService)
        {
            _escortServices = escortServices;
            _accountService = accountService;
        }

        [Route("index")]
        [HttpGet]
        public  IActionResult Index()
        {
            EscortModel escortlst = new EscortModel();
            ViewBag.PageIndex = Constants.DefultPageNumber;
            ViewBag.PageSize = Constants.DefultPageSize;
            return View(escortlst);
        }

        [Route("list")]
        [HttpPost]
        public async Task<IActionResult> List(UsersRequestModel model)
        {
            model.EstablishmentId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            model.UserType = (int)UserTypes.EstablishmentEscort;
            var items = await _escortServices.EscortList(model);
            var escortlist = items.Data;
            var result = new DataTableResult<EscortModel>
            {
                Draw = model.Draw,
                Data = escortlist,
                RecordsFiltered = escortlist?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = escortlist?.Count() ?? 0

            };
            return Json(result);
        }
        [Route("changeescortliststatus")]
        [HttpPost]
        public async Task<IActionResult> ChangeEscortListStatus(int userId, bool activeStatus, bool deleteStatus, bool isActiveStatusChange)
        {
            var changeEscortStatus = await _escortServices.ChangeEscortStatus(userId, activeStatus, deleteStatus);
            if (changeEscortStatus > 0)
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.EscortActivated });
                    }
                    else
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.EscortDeactivate });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.EscortDeleteSuccess });
                    }
                    else
                    {
                        return Ok(new ApiResponse<bool> { Message = ResourceString.RecoverEscort });
                    }
                }
            }
            else
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortActivateFailed });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortDeactivateFailed });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortDeleteFailed });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse<bool> { Message = ResourceString.EscortAccountRecoverFailed });
                    }
                }
            }

        }

        [Route("PauseAccount")]
        [HttpPost]
        public async Task<IActionResult> PauseAccount(int userId, bool isPause)
        {
            UsersRequestModel model = new();
            model.EstablishmentId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            model.UserType = (int)UserTypes.EstablishmentEscort;
            model.Length = int.MaxValue;
            
            var establishmentEscorts = await _escortServices.EscortList(model);
            var escortExistUnderEstablishment = establishmentEscorts.Data?.Exists(x => x.Id == userId);
            
            if (escortExistUnderEstablishment.HasValue)
            {
                var updateUserProfile = await _accountService.PauseEscort(userId, isPause);
                if (updateUserProfile.Data)
                {
                    return StatusCode(StatusCodes.Status200OK, updateUserProfile);
                }
            }
            
            return StatusCode(StatusCodes.Status404NotFound, new ApiResponse<bool>(false, message: ResourceString.SomethingWrong, apiName: "PauseEscort"));
        }
    }
}
