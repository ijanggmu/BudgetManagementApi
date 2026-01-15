using BeemaEdgeApi.Extensions.PaginationAndFilters;
using Business.Common.Email;
using Business.Common.Otp;
using Business.Common.Sms;
using Common.Mail;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Config;
using SharedKernel.Config.Mail;
using Sieve.Services;

namespace BeemaEdgeApi.Extensions.Application;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplicationExtension(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IMailService, SmtpMailService>();
        services.AddScoped<IMailLogService, MailLogService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<ISmsLogService, SmsLogService>();
        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
        services.AddScoped<ISieveExtension, SieveExtension>();
        services.AddScoped<OtpGeneratorService>();
        services.AddScoped<IOtpService, OptService>();


        return services;
    }
}

public static class ApplicationConfigExtension
{
    public static IServiceCollection AddApplicationConfigExtension(this IServiceCollection services)
    {
        services.AddOptions<MailOptions>().BindConfiguration(nameof(MailOptions));
        services.AddOptions<CoreApiOption>().BindConfiguration(nameof(CoreApiOption));
        services.AddOptions<AesConfig>().BindConfiguration(nameof(AesConfig));
        services.AddOptions<LoginSettings>().BindConfiguration(nameof(LoginSettings));
        services.AddOptions<PaymentGatewayOptions>().BindConfiguration(nameof(PaymentGatewayOptions));
        services.AddOptions<FrontEndUrlOptions>().BindConfiguration(nameof(FrontEndUrlOptions));
        services.AddOptions<MinioSettings>().BindConfiguration(nameof(MinioSettings));
        services.AddOptions<HmacAuthSettings>().BindConfiguration(nameof(HmacAuthSettings));
        services.AddOptions<SmsOptions>().BindConfiguration(nameof(SmsOptions));
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}
