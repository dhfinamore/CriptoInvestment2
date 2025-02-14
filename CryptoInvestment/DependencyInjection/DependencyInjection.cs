using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Infrastucture.Authentication;
using CryptoInvestment.Infrastucture.Common;
using CryptoInvestment.Infrastucture.Customers.Persistance;
using CryptoInvestment.Infrastucture.SecurityQuestions.Persistance;
using CryptoInvestment.Services;
using CryptoInvestment.Services.ConfigurationModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace CryptoInvestment.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ServiceCollectionExtensions));
        });

        services.AddSessionAuthentication(configuration);
        services.AddPersistance(configuration);
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }
    
    private static IServiceCollection AddSessionAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Authentication/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
                options.SlidingExpiration = true;
            });

        services.AddSession(options => {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddScoped<ICustomerAuthenticationService, CustomerAuthenticationService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services;
    }
    
    private static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionsSettings = new ConnectionSettings();
        configuration.Bind(ConnectionSettings.Section, connectionsSettings);

        services.AddDbContext<CryptoInvestmentDbContext>(options =>
            options.UseMySql(connectionsSettings.CryptoInvestmentDb, ServerVersion.AutoDetect(connectionsSettings.CryptoInvestmentDb))
        );

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISecurityQuestionRepository, SecurityQuestionRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CryptoInvestmentDbContext>());
        
        return services;
    }
}