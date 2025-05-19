using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Web.Controllers.Base;

namespace Web.Controllers
{
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AdminTestimonialController : AdminBaseController
    {
        private readonly IManageService _manageService;

        public AdminTestimonialController(IManageService manageService, IAccountService accountService)
        {
            _manageService = manageService;

        }

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
