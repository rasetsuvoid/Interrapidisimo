using Interrapidisimo.Domain.Interfaces;
using Interrapidisimo.Infrastructure.Persistence;
using Interrapidisimo.Infrastructure.Persistence.Repositories;
using Interrapidisimo.Infrastructure.Persistence.Seeders;
using Interrapidisimo.Infrastructure.Services;
using Interrapidisimo.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Interrapidisimo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure(3);
                    sql.CommandTimeout(30);
                });
            options.EnableSensitiveDataLogging(false);
        });

        services.AddScoped<IUnitOfWork>(sp =>
            new UnitOfWork(sp.GetRequiredService<ApplicationDbContext>()));

        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IRsaEncryptionService, RsaEncryptionService>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
