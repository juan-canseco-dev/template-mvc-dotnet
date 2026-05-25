using LoginMVC.Identity;
using LoginMVC.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Data.Seeders;

public class AdminUserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(
        UserManager<ApplicationUser> userManager,
        ILogger<AdminUserSeeder> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {

            _logger.LogInformation("Starting admin user seeding.");

            if (await _userManager.Users.AnyAsync())
            {
                _logger.LogInformation(
                    "Admin user seeding skipped because users already exist in the database.");

                return;
            }

            var applicationUser = new ApplicationUser
            {
                UserName = AdminConstants.ADMIN_EMAIL,
                Email = AdminConstants.ADMIN_EMAIL,
                Fullname = AdminConstants.ADMIN_NAME
            };

            _logger.LogInformation(
                "Creating admin user with email {Email}.",
                AdminConstants.ADMIN_EMAIL);

            var result = await _userManager.CreateAsync(
                applicationUser,
                AdminConstants.ADMIN_PASSWORD);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Admin user {Email} created successfully.",
                    AdminConstants.ADMIN_EMAIL);

                return;
            }

            _logger.LogError(
                "Failed to create admin user {Email}. Errors: {Errors}",
                AdminConstants.ADMIN_EMAIL,
                string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
          "An unexpected error occurred while seeding the admin user.");

            throw;
        }
    }
}