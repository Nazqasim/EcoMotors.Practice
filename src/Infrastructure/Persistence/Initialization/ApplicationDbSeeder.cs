using EcoMotorsPractice.Infrastructure.Identity;
using EcoMotorsPractice.Infrastructure.Persistence.Context;
using EcoMotorsPractice.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcoMotorsPractice.Infrastructure.Persistence.Initialization;

internal class ApplicationDbSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CustomSeederRunner _seederRunner;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, CustomSeederRunner seederRunner, ILogger<ApplicationDbSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _seederRunner = seederRunner;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedRolesAsync(dbContext);
        await SeedAdminUserAsync();
        await _seederRunner.RunSeedersAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(ApplicationDbContext dbContext)
    {
        foreach (string roleName in AppRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                _logger.LogInformation("Seeding {role} Role.", roleName);
                role = new ApplicationRole(roleName, $"{roleName} Role");
                await _roleManager.CreateAsync(role);
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        string adminEmail = "admin@boilerplate.com";
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail)
            is not ApplicationUser adminUser)
        {
            string adminUserName = AppRoles.Admin.ToLowerInvariant();
            adminUser = new ApplicationUser
            {
                FirstName = "admin",
                Email = adminEmail,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = adminEmail?.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true
            };

            _logger.LogInformation("Seeding Default Admin User");
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, "123");
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User");
            await _userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }

        // second admin
        string secondAdminEmail = "boilerplateadmin@yopmail.com";
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == secondAdminEmail)
            is not ApplicationUser secondAdminUser)
        {
            string secondAdminUserName = AppRoles.Admin.ToLowerInvariant() + "boilerplate";
            secondAdminUser = new ApplicationUser
            {
                FirstName = "adminboilerplate",
                Email = secondAdminEmail,
                UserName = secondAdminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = adminEmail?.ToUpperInvariant(),
                NormalizedUserName = secondAdminUserName.ToUpperInvariant(),
                IsActive = true
            };

            _logger.LogInformation("Seeding Default Admin User");
            var password = new PasswordHasher<ApplicationUser>();
            secondAdminUser.PasswordHash = password.HashPassword(secondAdminUser, "123");
            await _userManager.CreateAsync(secondAdminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(secondAdminUser, AppRoles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User");
            await _userManager.AddToRoleAsync(secondAdminUser, AppRoles.Admin);
        }
    }
}