using Business.IServices;
using Business.Services;
using Dapper;
//using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Resources;
using System.Data.SqlClient;
using System.Data;
using Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserController : UserBaseController
    {
        private readonly IManageService _manageService;
        private readonly IAccountService _accountService;
        public UserController(IManageService manageService, IAccountService accountService)
        {
            _manageService = manageService;
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Profile()
        {
            var countryCodes = await _accountService.GetCountryCodes();
            ViewBag.SelectListItems = countryCodes.Select(x => new SelectListItem() { Text = x.CountryName, Value = x.CountryName }).ToList();
            var customerDetail = await _manageService.GetUserDetails(UserId);
            return View(customerDetail.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserDetailsDto requestModel)
        {
            if (requestModel.UserId > 0)
            {

                requestModel.CroppedProfileFile = Request.Form["CroppedProfileFile"].ToString();
                var updateUser = await _manageService.UpdateUserDetail(requestModel);
                if (updateUser.Data != null)
                {
                    LoginSessionModel sessionobj = new();
                    sessionobj.FirstName = requestModel.FirstName;
                    sessionobj.LastName = requestModel.LastName;
                    sessionobj.UserId = requestModel.UserId;
                    sessionobj.UserTypeId = Convert.ToInt16(UserTypes.Client);
                    sessionobj.EmailId = requestModel.Email;
                    sessionobj.ProfileImage = updateUser.Data.ProfileImage;
                    sessionobj.DisplayName = requestModel.DisplayName;
                    LoginMemberSession.UserDetailSession = sessionobj;
                }
                return Json(updateUser);
            }
            else
            {
                return BadRequest(new ApiResponse<int> { Data = 0, Message = ResourceString.ProfileUpdateFailed });
            }

        }

        
        
    }
}
