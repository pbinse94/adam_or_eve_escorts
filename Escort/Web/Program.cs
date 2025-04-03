using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.IVS;
using Amazon.Ivschat;
using Amazon.Runtime;
using Azure.Storage.Blobs;
using Business.Communication;
//using Errlake.Crosscutting;
using IOC.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Shared.Common;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var services = builder.Services;
var configuration = builder.Configuration;

// Configure SiteKeys dynamically
var sitePhysicalPath = configuration["SiteKeys:SitePhysicalPath"];

if (string.IsNullOrEmpty(sitePhysicalPath))
{
    sitePhysicalPath = Path.Combine(AppContext.BaseDirectory, "Web");
}

SiteKeys.SitePhysicalPath = sitePhysicalPath;
SiteKeys.SiteUrl = configuration["SiteKeys:SiteUrl"];
SiteKeys.GeoLocationApiKey = configuration["SiteKeys:GeoLocationApiKey"];
SiteKeys.EncryptDecryptKey = configuration["SiteKeys:EncryptDecryptKey"];
SiteKeys.StripeSecreatKey = configuration["SiteKeys:StripeSecreatKey"];
SiteKeys.PaypalClientId = configuration["SiteKeys:PaypalClientId"];
SiteKeys.PaypalClientSecret = configuration["SiteKeys:PaypalClientSecret"];
SiteKeys.BaseURL = configuration["SiteKeys:BaseURL"];
SiteKeys.IsLive = Convert.ToBoolean(configuration["SiteKeys:IsLive"]);
SiteKeys.RememberMeCookieTimeMinutes = Convert.ToInt32(configuration["SiteKeys:RememberMeCookieTimeMinutes"]);
SiteKeys.AdminPercentage = Convert.ToDecimal(configuration["SiteKeys:AdminPercentage"]);

FirebaseKeys.FCMServerKeyFilePath = configuration["FirebaseKeys:FCMServerKeyFilePath"];
FirebaseKeys.FCMProjectId = configuration["FirebaseKeys:FCMProjectId"];
FirebaseKeys.FirebaseMessagingUrl = configuration["FirebaseKeys:FirebaseMessagingUrl"];

// Configure Email
builder.Services.Configure<EmailConfigurationKeys>(configuration.GetSection("EmailConfigurationKeys"));

// Register other configurations
builder.Services.Configure<SiteKeys>(configuration.GetSection("SiteKeys"));
builder.Services.Configure<AwsKeys>(configuration.GetSection("AwsSettings"));

// Add Razor Pages with runtime compilation
services.AddRazorPages().AddRazorRuntimeCompilation();

// Register custom services
services.RegisterWebApi(configuration);

// Initialize AWS region
var region = RegionEndpoint.APSoutheast2; // Set AWS region

// Set AWS region globally for all SDK clients
AWSConfigs.AWSRegion = region.SystemName;
services.AddAWSService<IAmazonIVS>();
services.AddAWSService<IAmazonIvschat>();

// Configure authentication & authorization
services.AddAuthorization(config =>
{
    config.AddPolicy("AuthorisedUser", policy =>
    {
        policy.RequireClaim("UserId");
    });

    config.AddPolicy("AdminRolePolicy", policy =>
    {
        policy.RequireClaim("Device");
        policy.RequireClaim("DeviceType");
        policy.RequireClaim("Offset");
    });
});

services.AddAuthentication("CookiesAuth").AddCookie("CookiesAuth", config =>
{
    config.Cookie.Name = "Identitye.Cookie";
    config.LoginPath = "/Account/login";
});

services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization();

services.AddControllersWithViews();
services.AddSession();
services.AddHttpClient();

// Configure form options
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.ValueCountLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
    x.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapGet("/healthcheck", () => Results.Ok("Healthy")).WithMetadata(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute());

app.UseHttpsRedirection();
app.UseStatusCodePagesWithRedirects("/Error/Error{0}");
app.UseAuthentication();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.UseCookiePolicy();

app.MapControllers();
app.MapRazorPages();
app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();