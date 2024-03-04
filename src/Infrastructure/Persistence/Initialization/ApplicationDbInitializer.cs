using EcoMotorsPractice.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcoMotorsPractice.Infrastructure.Persistence.Initialization;

internal class ApplicationDbInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ApplicationDbSeeder _dbSeeder;
    private readonly ILogger<ApplicationDbInitializer> _logger;

    public ApplicationDbInitializer(ApplicationDbContext dbContext, ApplicationDbSeeder dbSeeder, ILogger<ApplicationDbInitializer> logger)
    {
        _dbContext = dbContext;
        _dbSeeder = dbSeeder;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_dbContext.Database.GetMigrations().Any())
        {
            if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                _logger.LogInformation("Applying Migrations");
                await _dbContext.Database.MigrateAsync(cancellationToken);
            }

            if (await _dbContext.Database.CanConnectAsync(cancellationToken))
            {
                _logger.LogInformation("Connection to Database Succeeded.");

                await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
            }

            //await _dbContext.Database.ExecuteSqlRawAsync(
            //"CREATE OR ALTER VIEW ReferentialUser AS SELECT Id, FirstName, LastName, UserName, ImageUrl, DeviceToken, PhoneNumber, IsActive, ObjectId FROM \"Identity\".\"Users\"");

            await _dbContext.Database.ExecuteSqlRawAsync(
            @"DO
            $$
            BEGIN
                BEGIN
                    -- Try to drop the existing view
                    DROP VIEW IF EXISTS ""Identity"".""ReferentialUser"";
                EXCEPTION
                    -- Ignore the exception if the view doesn't exist
                    WHEN OTHERS THEN
                        NULL;
                END;

                -- Create the view
                CREATE VIEW ""Identity"".""ReferentialUser"" AS
                SELECT ""Id"", ""FirstName"", ""LastName"",""Email"", ""UserName"", ""ImageUrl"", ""DeviceToken"", ""PhoneNumber"", ""IsActive"", ""ObjectId""
                FROM ""Identity"".""Users"";
            END
            $$;");
        }
    }
}
