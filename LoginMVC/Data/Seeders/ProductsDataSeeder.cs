using Bogus;
using LoginMVC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Data.Seeders;

public class ProductsDataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsDataSeeder> _logger;

    public ProductsDataSeeder(ApplicationDbContext context, ILogger<ProductsDataSeeder> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public  Faker<Product> Faker => new Faker<Product>()
          .RuleFor(p => p.Name, f => f.Commerce.ProductName())
          .RuleFor(p => p.PurchasePrice, f => Math.Round(f.Random.Decimal(10m, 1000m), 2))
          .RuleFor(p => p.SalePrice, (f, p) =>
              Math.Round(p.PurchasePrice + f.Random.Decimal(1m, p.PurchasePrice * 0.5m), 2))
          .RuleFor(p => p.Quantity, f => f.Random.Int(1, 1000));
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting products data seeding.");

            if (await _context.Products.AnyAsync())
            {
                _logger.LogInformation(
                    "Products data seeding skipped because products already exist in the database.");
                return;
            }

            var products = Faker.Generate(300);
            _context.Products.AddRange(products);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Products data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while seeding products data.");
        }
    }
}
