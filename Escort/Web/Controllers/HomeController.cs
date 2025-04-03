using Amazon.IVS.Model;
using Amazon.IVS;
using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.DTO;
using Shared.Model.Request.WebUser;
using System.Diagnostics;
using Web.Models;
using Amazon.Ivschat;
using Amazon;
using Amazon.Ivschat.Model;
using Amazon.Runtime;
using System.Linq;
using Shared.Model.Escort;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Shared.Model.Request.Profile;
using Business.Communication;
using System.Drawing.Printing;
using CliWrap;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Web.Controllers.Base;

namespace Web.Controllers
{

    [AllowAnonymous]
    public class HomeController : UserBaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEscortServices _escortServices;
        private readonly ICommonService _commonService;
        private readonly AwsKeys _awsConfig;
        public HomeController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger, IAccountService accountService, IFileStorageService fileStorageService, IEscortServices escortServices, IOptions<AwsKeys> awsConfig, ICommonService commonService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _fileStorageService = fileStorageService;
            _escortServices = escortServices;
            _awsConfig = awsConfig.Value;
            _commonService = commonService;
        }
         
        public IActionResult Index()
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            var context = _httpContextAccessor.HttpContext; 

            ViewBag.SexualPreferences = EnumExtensions.EnumValuesAndDescriptionsToList<SexualPreferencesTypes>().Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value,
                Selected = false
            }).ToList();



            var countryCodes = _accountService.GetCountryCodes();
            ViewBag.CountryItems = countryCodes.Result.Select(x => new SelectListItem()
            {
                Text = x.CountryName,
                Value = $"{x.Abbreviation}|{x.CountryName}"
            }).ToList();



            var categories = _commonService.GetCategories().Result;

            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryID.ToString(),
                Text = c.CategoryName
            }).ToList();

            SiteKeys.UtcOffset = Convert.ToInt32(context?.Request.Cookies["timezoneoffset"]);
            if (Request.Cookies["timezoneoffset"] != null)
            {
                _httpContextAccessor.HttpContext?.Session.SetInt32("UtcOffsetInSecond", Convert.ToInt32(Request.Cookies["timezoneoffset"]) * 60);
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> FeaturedEscorts(string ageGroup, List<string> sexualPreferences, List<string> escortCategories, string gender, string rates, string search, string location, int escortType, int pageIndex = 1, int pageSize = 12, int callType = 0, int profileType = -1)
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            EscortSearchRequest escortSearch = new()
            {
                LoginUserId = userId,
                StartAge = string.IsNullOrWhiteSpace(ageGroup) ? 0 : Convert.ToInt32(ageGroup.Split("-")[0]),
                EndAge = string.IsNullOrWhiteSpace(ageGroup) ? 0 : Convert.ToInt32(ageGroup.Split("-")[1]),
                StartRate = string.IsNullOrWhiteSpace(rates) ? 0 : Convert.ToInt32(rates.Split("-")[0]),
                EndRate = string.IsNullOrWhiteSpace(rates) ? 0 : Convert.ToInt32(rates.Split("-")[1]),
                Gender = gender,
                Name = search,
                Country = location,
                EscortType = escortType,
                Services = sexualPreferences,
                 EsortCategories = escortCategories,
                PageIndex = pageIndex,
                PageSize = pageSize,
                ProfileType = profileType,
                CallType = callType,
            };
            ViewBag.PageIndex = pageIndex;
            List<EscortSearchDto> featuredEscorts = await _accountService.GetFeaturedEscorts(escortSearch);
            return View("_FeaturedEscorts", featuredEscorts);
        }


        [HttpGet]
        public async Task<IActionResult> VipEscorts(string country)
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            EscortSearchRequest escortSearch = new()
            {
                LoginUserId = userId,
                Country = country
            };
            List<EscortSearchDto> vipEscorts = await _accountService.GetVipEscorts(escortSearch);

            return View(vipEscorts);
        }


        [HttpPost]
        public async Task<IActionResult> GetVipEscorts(string country)
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            EscortSearchRequest escortSearch = new()
            {
                LoginUserId = userId,
                Country = country
            };
            List<EscortSearchDto> vipEscorts = await _accountService.GetVipEscorts(escortSearch);
            return View("_VipEscorts", vipEscorts);
        }

        [HttpPost]
        public async Task<IActionResult> GetVipEscortsPage(string country)
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            EscortSearchRequest escortSearch = new()
            {
                LoginUserId = userId,
                Country = country
            };
            List<EscortSearchDto> vipEscorts = await _accountService.GetVipEscorts(escortSearch);
            return View("_VipEscortsPage", vipEscorts);
        }

        [HttpGet]
        public IActionResult PopularEscorts()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetHomePopularEscorts(int pageIndex, string countryName)
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            PopularEscortRequest escortSearch = new()
            {
                LoginUserId = userId,
                PageIndex = pageIndex,
                PageSize = 12 ,
                Country = countryName,
            };
            List<EscortSearchDto> popularEscorts = await _accountService.GetPopularEscorts(escortSearch);
            return View("_HomePopularEscorts", popularEscorts);
        }

        [HttpPost]
        public async Task<IActionResult> GetPopularEscorts(int pageIndex, string searchText, int pageSize = 12)
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            PopularEscortRequest escortSearch = new()
            {
                LoginUserId = userId,
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchText = searchText
            };
            ViewBag.PageIndex = pageIndex;
            List<EscortSearchDto> popularEscorts = await _accountService.GetPopularEscorts(escortSearch);
            return View("_PopularEscorts", popularEscorts);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<string> GetDocumentUrl(string pathToImage, string mediaType = "")
        {
            var result = await _fileStorageService.GetDocumentUrl(pathToImage, mediaType);
            return result ?? string.Empty;
        }

        [HttpGet]
        public async Task<PartialViewResult> GetHeaderCountries()
        {
            var model = await _accountService.GetCountryCodes();
            return PartialView("_HeaderCountries", model);
        }

        [HttpGet]
        public IActionResult MyFavorite()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> FavoriteEscorts(string search, int pageIndex = 1, int pageSize = 12)
        {
            var userId = LoginMemberSession.UserDetailSession?.UserId ?? 0;
            EscortSearchRequest escortSearch = new()
            {
                LoginUserId = userId,
                Name = search,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            ViewBag.PageIndex = pageIndex;
            List<EscortSearchDto> featuredEscorts = await _accountService.GetFavoriteEscorts(escortSearch);
            return View("_FavoriteEscorts", featuredEscorts);
        }
       
        [HttpPost]
        public IActionResult SendEmail(EmailRequestModel model)
        {
            if (ModelState.IsValid)
            {
                _escortServices.SendContactUsMailToEscort("Contact form", model);

                return Json(new { success = true, message = "Email sent successfully!" });
            }

            // Collect validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            return Json(new { success = false, errors = errors });
        }
        [HttpPost]
        public async Task<IActionResult> VideoCutter(IFormFile videoFile, double startTime, double duration)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest("Invalid video file.");
            }

            var inputPath = Path.Combine(Path.GetTempPath(), videoFile.FileName);
            var outputPath = Path.Combine(Path.GetTempPath(), $"trimmed_{videoFile.FileName}");

            using (var stream = new FileStream(inputPath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            try
            {
                await TrimVideo(inputPath, outputPath, TimeSpan.FromSeconds(startTime), TimeSpan.FromSeconds(duration));

                var memory = new MemoryStream();
                using (var stream = new FileStream(outputPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/octet-stream", Path.GetFileName(outputPath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(inputPath))
                {
                    System.IO.File.Delete(inputPath);
                }
                if (System.IO.File.Exists(outputPath))
                {
                    System.IO.File.Delete(outputPath);
                }
            }
        }

        private async Task TrimVideo(string inputPath, string outputPath, TimeSpan startTime, TimeSpan duration)
        {
            var result = await Cli.Wrap("ffmpeg")
                .WithArguments($"-ss {startTime} -i \"{inputPath}\" -t {duration} -c copy \"{outputPath}\"")
                .ExecuteAsync();

            if (result.ExitCode != 0)
            {
                throw new Exception($"FFmpeg exited with code {result.ExitCode}");
            }
        }

        public class VideoUploadModel
        {
            #nullable disable
            public IFormFile VideoFile { get; set; }
            public Double StartTime { get; set; } // The start time for trimming
            public Double Duration { get; set; }  // The duration of the trimmed video
        }

        //    [HttpPost]
        //    public async Task<IActionResult> VideoCutter()
        //    {
        //        var model = Request.Form.Files.GetFile("VideoStream");
        //        if (model == null || model == null)
        //        {
        //            return BadRequest(new { error = "Please upload a video file." });
        //        }

        //        var inputPath = Path.Combine(Path.GetTempPath(), model.FileName);
        //        var outputPath = Path.Combine(Path.GetTempPath(), $"trimmed_{model.FileName}");

        //        using (var stream = new FileStream(inputPath, FileMode.Create))
        //        {
        //            await model.CopyToAsync(stream);
        //        }

        //        try
        //        {
        //            await TrimVideoToTenSeconds(inputPath, outputPath);

        //            using (var memory = new MemoryStream())
        //            {
        //                using (var stream = new FileStream(outputPath, FileMode.Open))
        //                {
        //                    await stream.CopyToAsync(memory);
        //                }
        //                memory.Position = 0;

        //                var fileBytes = memory.ToArray();
        //                return Ok(new
        //                {

        //                    fileBytes = Convert.ToBase64String(fileBytes)
        //                });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
        //        }
        //        finally
        //        {
        //            if (System.IO.File.Exists(inputPath))
        //            {
        //                System.IO.File.Delete(inputPath);
        //            }
        //            if (System.IO.File.Exists(outputPath))
        //            {
        //                System.IO.File.Delete(outputPath);
        //            }
        //        }
        //    }

        //    private async Task TrimVideoToTenSeconds(string inputPath, string outputPath)
        //    {
        //        var result = await Cli.Wrap("ffmpeg")
        //.WithArguments($"-i \"{inputPath}\" -t 00:00:10 -c copy \"{outputPath}\"")
        //.ExecuteAsync();

        //        if (result.ExitCode != 0)
        //        {
        //            throw new Exception($"FFmpeg exited with code {result.ExitCode}");
        //        }
        //    }

        //public class VideoUploadModel
        //{
        //    public required IFormFile VideoStream { get; set; }
        //}
    }

}