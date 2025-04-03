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
    public class TestimonialController : UserBaseController
    {
        private readonly IManageService _manageService;
        
        public TestimonialController(IManageService manageService, IAccountService accountService)
        {
            _manageService = manageService;
           
        }
       

     
        
        [HttpPost]
        public async Task<ActionResult> AddTestimonial(int escortId, string testimonialDetails)
        {
           
            var updateUser = await _manageService.AddTestimonials(escortId, testimonialDetails,UserId);
            if (updateUser == 1)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetTestimonial(int id)
        {
            var testimonial = await _manageService.GetTestimonialAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
          
            return View("_Testimonial", testimonial);
        }
    }
}
