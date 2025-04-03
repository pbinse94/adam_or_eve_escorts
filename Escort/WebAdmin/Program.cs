//using Errlake.Crosscutting;
using IOC.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Shared.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
.AddEnvironmentVariables();

var services = builder.Services;
var configuration = builder.Configuration;

builder.Services.Configure<EmailConfigurationKeys>(configuration.GetSection("EmailConfigurationKeys"));//for Email configuration keys
builder.Services.Configure<SiteKeys>(configuration.GetSection("SiteKeys"));
builder.Services.Configure<AwsKeys>(configuration.GetSection("AwsSettings"));
SiteKeys.SitePhysicalPath = configuration["SiteKeys:SitePhysicalPath"];
SiteKeys.SiteUrl = configuration["SiteKeys:SiteUrl"];
SiteKeys.FrontSiteUrl=configuration["SiteKeys:FrontSiteUrl"];
 
SiteKeys.AdminPercentage = Convert.ToDecimal(configuration["SiteKeys:AdminPercentage"]);
SiteKeys.IsLive = Convert.ToBoolean(configuration["SiteKeys:IsLive"]);

SiteKeys.EncryptDecryptKey = builder.Configuration["SiteKeys:EncryptDecryptKey"];

SiteKeys.RememberMeCookieTimeMinutes = Convert.ToInt32(builder.Configuration["SiteKeys:RememberMeCookieTimeMinutes"]);


services.AddRazorPages().AddRazorRuntimeCompilation();

services.RegisterWebApi(configuration);


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
//services.AddError();
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.ValueCountLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
    x.MemoryBufferThreshold = int.MaxValue;

});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.ConfigureExceptionMiddlewareHandler();


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
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

