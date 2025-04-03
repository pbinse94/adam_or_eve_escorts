using Amazon;
using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Escort;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;
using Shared.Resources;
using WebAdmin.Controllers.Base;

namespace WebAdmin.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly IFileStorageService _fileStorageService;

        public HomeController( IFileStorageService fileStorageService )
        {
          
            _fileStorageService = fileStorageService;
         
        }

        [HttpGet]
        public async Task<string> GetDocumentUrl(string pathToImage, string mediaType = "")
        {
            var result = await _fileStorageService.GetDocumentUrl(pathToImage, mediaType);
            return result ?? string.Empty;
        }

        [HttpGet]
        public ActionResult IsSessionActive()
        {
            LoginSessionModel? userObj = HttpContext.Session.GetComplexData<LoginSessionModel>("LoginMemberSession");
            if (userObj != null)  // Check if session value exists
            {
                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }

    }

    public static class SessionExtensions
    {
        public static void SetComplexData<T>(this ISession session, string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            session.SetString(key, jsonData);
        }

        public static T? GetComplexData<T>(this ISession session, string key)
        {
            var jsonData = session.GetString(key);
            if (jsonData == null)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
