using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Business.Common.Token;
using Data.Context;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using BeemaEdgeApi.Utilities.ResponseWrapper;

namespace BeemaEdgeApi.Extensions.Application;

public static class AuthenticationServiceExtension
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = false;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;

        })
            .AddEntityFrameworkStores<ApplicationDataContext>()
            .AddDefaultTokenProviders();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];
        
        services.AddAuthentication(i =>
        {
            i.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            i.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            i.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            i.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
            opt.SaveToken = true;
            opt.Events = new JwtBearerEvents
            {
                // This method is first event in authentication pipeline
                // we have chance to wait until TokenValidationParameters
                // is loaded.
                OnMessageReceived = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/hangfire-admin-dashboard"))
                        return Task.CompletedTask;

                    if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                    {
                        var cookieValue = context.Request.Cookies["X-Access-Token"];
                        if (!string.IsNullOrEmpty(cookieValue))
                        {
                            // Decode the URL-encoded token from cookie
                            context.Token = HttpUtility.UrlDecode(cookieValue);
                        }
                    }
                    else if (context.Request.Cookies.ContainsKey("X-Refresh-Token"))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Fail("");
                    }
                    else
                    {
                        // Don't fail if no token - let the authorization handle it
                        // This allows public endpoints to work
                        return Task.CompletedTask;
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    if (context.Response.StatusCode != (int)HttpStatusCode.Forbidden)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                    else
                    {
                        context.HandleResponse();
                        context.Response.ContentType = "application/json";
                        var errorObject = ErrorApiResponse.WrapError("Forbidden", 4031);
                        var json = JsonSerializer.Serialize(errorObject);

                        return context.Response.WriteAsync(json);
                    }

                }
            };
        });

        services.AddScoped<ITokenService,TokenService>();
        services.AddHttpContextAccessor();
        return services;
    }
}
