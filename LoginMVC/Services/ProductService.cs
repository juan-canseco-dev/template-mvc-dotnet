using LoginMVC.Data;
using LoginMVC.Domain.Entities;
using LoginMVC.Domain.Errors;
using LoginMVC.Extensions;
using LoginMVC.Services.Interfaces;
using LoginMVC.Shared.Pagination;
using LoginMVC.Shared.Results;
using LoginMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> CreateAsync(
        CreateProductViewModel request, 
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (request.SalePrice <= request.PurchasePrice)
            {
                return Result.Failure<int>(ProductErrors.InvalidSalePrice);
            }

            var newProduct = new Product
            {
                Name = request.Name,
                SalePrice = request.SalePrice,
                PurchasePrice = request.PurchasePrice,
                Quantity = request.Quantity
            };


            _context.Products.Add(newProduct);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product {ProductId} created successfully.", newProduct.Id);


            return Result.Success(newProduct.Id);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex,
            "An unexpected error occurred while creating product {ProductName}.", request.Name);
            return Result.Failure<int>(Error.InternalServerError);
        }
    }

    public async Task<Result> UpdateAsync(UpdateProductViewModel request, CancellationToken cancellationToken = default)
    {
        try 
        {
            var product = await _context.Products.FindAsync([request.ProductId], cancellationToken);
            if (product is null)
            {
                return Result.Failure(ProductErrors.NotFound);
            }

            if (request.SalePrice <= request.PurchasePrice)
            {
                return Result.Failure(ProductErrors.InvalidSalePrice);
            }

            product.Name = request.Name;
            product.SalePrice = request.SalePrice;
            product.PurchasePrice = request.PurchasePrice;
            product.Quantity = request.Quantity;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating product {ProductId}.", request.ProductId);
            return Result.Failure(Error.InternalServerError);
        }
    }

    public async Task<Result> DeleteAsync(int productId, CancellationToken cancellationToken = default)
    {
        try
        {

            var product = await _context.Products.FindAsync([productId], cancellationToken);
            if (product is null)
            {
                return Result.Failure(ProductErrors.NotFound);
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting product {ProductId}.", productId);
            return Result.Failure(Error.InternalServerError);
        }
    }

    public async Task<Result<ProductViewModel>> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await ApplyProjection(_context.Products)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            return product is null
                ? Result.Failure<ProductViewModel>(ProductErrors.NotFound)
                : Result.Success(product);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving product {ProductId}.", productId);
            return Result.Failure<ProductViewModel>(Error.InternalServerError);
        }
    }

    private IQueryable<Product> ApplyFiltering(
        IQueryable<Product> query,
        GetProductsViewModel request
    )
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(p =>
                EF.Functions.Like(p.Name!, $"%{request.Name}%"));
        }

        return query;
    }

    private IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        GetProductsViewModel request
    )
    {
        var descending = string.Equals(
            request.SortOrder,
            "desc",
            StringComparison.OrdinalIgnoreCase);

        return (request.OrderBy?.ToLowerInvariant()) switch
        {
            "id" => descending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id),

            "name" => descending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            "purchaseprice" => descending
                ? query.OrderByDescending(p => p.PurchasePrice)
                : query.OrderBy(p => p.PurchasePrice),

            "saleprice" => descending
                ? query.OrderByDescending(p => p.SalePrice)
                : query.OrderBy(p => p.SalePrice),

            "quantity" => descending
                ? query.OrderByDescending(p => p.Quantity)
                : query.OrderBy(p => p.Quantity),

            _ => query.OrderBy(p => p.Id)
        };
    }

    private IQueryable<Product> BuildQuery(GetProductsViewModel request)
    {
        var query = _context.Products.AsNoTracking();

        query = ApplyFiltering(query, request);
        query = ApplySorting(query, request);

        return query;
    }

    private IQueryable<ProductViewModel> ApplyProjection(IQueryable<Product> query)
    {
        return query.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            PurchasePrice = p.PurchasePrice,
            SalePrice = p.SalePrice,
            Quantity = p.Quantity
        });
    }
    public async Task<Result<IReadOnlyCollection<ProductViewModel>>> GetAsync(
        GetProductsViewModel request, 
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await ApplyProjection(BuildQuery(request))
              .ToListAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving products.");
            return Result.Failure<IReadOnlyCollection<ProductViewModel>>(Error.InternalServerError);
        }
    }

    public async Task<Result<PaginatedList<ProductViewModel>>> GetPaginatedAsync(
        GetProductsPageViewModel request, 
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await ApplyProjection(BuildQuery(request))
               .ToPaginatedListAsync(
                   request.PageNumber,
                   request.PageSize,
                   cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving paginated products.");
            return Result.Failure<PaginatedList<ProductViewModel>>(Error.InternalServerError);
        }
    }

}
