using Data.Factory;
using Data.IFactory;
using Data.IRepository;
using Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOC.Extensions
{
    public static class ContainerExtensionRepositry
    {
        public static void RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("DbConnection");
            if (string.IsNullOrEmpty(dbConnectionString)) throw new Exception("Empty Database connection string");

            services.AddTransient(typeof(IRepositoryConfiguration), ctx => new RepositoryConfiguration(dbConnectionString));

            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IActivityLogRepository, ActivityLogRepository>();
            services.AddTransient<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            services.AddTransient(typeof(ICurdRepository<>), typeof(CurdRepository<>));
            services.AddTransient<ICommonRepository, CommonRepository>();
            services.AddTransient<IProfileRepository, ProfileRepository>();
            services.AddTransient<ISubscriptionTransactionsRepository, SubscriptionTransactionsRepository>();


            services.AddTransient<IEscortRepository, EscortRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IEstablishmentRepository, EstablishmentRepository>();
            services.AddTransient<ICreditRepository, CreditRepository>();
            services.AddTransient<IUserVerificationCodeRepository, UserVerificationCodeRepository>();
            

        }
    }
}
