using Business.IServices;
using Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Ivschat;
using Business.Communication;
using Shared.Common;

namespace IOC.Extensions
{
    public static class ContainerExtensionService
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterRepositories(configuration);
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IManageService, ManageService>();
            services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddTransient<IFileStorageService, FileStorageService>();
            services.AddTransient<IEstablishmentService, EstablishmentService>();
            services.AddTransient<ISubscriptionTransactionsService, SubscriptionTransactionsService>();

            services.AddTransient<IEscortServices, EscortServices>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICreditService, CreditService>();
            services.AddTransient<IUserVerificationCodeService, UserVerificationCodeService>();
            services.AddHttpClient<PayPalService>();
            services.AddTransient<AdminPermissionService>();
        }
    }
}