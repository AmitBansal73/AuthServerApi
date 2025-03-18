using AuthServerApi.Service;
using AuthServerApi.Service.Authenticators;
using AuthServerApi.Service.RefreshTokenRepositories;
using AuthServerApi.Service.TokenGenerator;
using AuthServerApi.Service.TokenValidator;
using AuthServerApi.Service.UserRepositories;

namespace AuthServerApi
{
    public static class Dependenciescs
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services) {
            services.AddSingleton<AccessTokenGenerator>();
            services.AddSingleton<RefreshTokenGenerator>();
            services.AddSingleton<RefreshTokenValidator>();
            //services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddScoped<IUserRepository, SqlUserRepository>();
            services.AddScoped<IRefreshTokenRepository, SqlRefreshTokenRepository>();
            services.AddScoped<Authenticator>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }
    }
}
