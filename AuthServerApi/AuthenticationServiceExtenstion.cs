using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuthServerApi
{
    public static class AuthenticationServiceExtenstion
    {
        public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfigurationManager configuration)
        {
            var clientid = configuration.GetSection("GoogleAuthentication:client-id").Value;
            var clientsecret = configuration.GetSection("GoogleAuthentication:client-secret").Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie()
                .AddGoogle("Google", options => {
                    options.ClientId = clientid;
                    options.ClientSecret = clientsecret;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("Authentication:AccessTokenSecret").Value)),
                        ValidIssuer = configuration.GetSection("Authentication:Issuer").Value,
                        ValidAudience = configuration.GetSection("Authentication:Audience").Value,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };

                    option.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx => {
                            ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                            if (!string.IsNullOrEmpty(accessToken))
                                ctx.Token = accessToken;
                            return Task.CompletedTask;
                        }
                    };
                });
            return services;
        }
    }
}
