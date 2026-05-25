using Dapper;
using LoginMVC.Data;
using LoginMVC.Domain.Errors;
using LoginMVC.Services.Interfaces;
using LoginMVC.Shared.Pagination;
using LoginMVC.Shared.Results;
using LoginMVC.ViewModels;
using System.Data;

namespace LoginMVC.Services;



public class ProductServiceStoreProcedureImpl : IProductService
{
    private readonly DapperContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductServiceStoreProcedureImpl(
        DapperContext context,
        ILogger<ProductService> logger)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));

        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> CreateAsync(
        CreateProductViewModel request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.SalePrice <= request.PurchasePrice)
            {
                return Result.Failure<int>(
                    ProductErrors.InvalidSalePrice);
            }

            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Name", request.Name);
            parameters.Add("@PurchasePrice", request.PurchasePrice);
            parameters.Add("@SalePrice", request.SalePrice);
            parameters.Add("@Quantity", request.Quantity);

            parameters.Add(
                "@ProductId",
                dbType: DbType.Int32,
                direction: ParameterDirection.Output);

            var command = new CommandDefinition(
                "dbo.usp_Product_Create",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            await connection.ExecuteAsync(command);

            var productId = parameters.Get<int>("@ProductId");

            _logger.LogInformation(
                "Product {ProductId} created successfully.",
                productId);

            return Result.Success(productId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred while creating product {ProductName}.",
                request.Name);

            return Result.Failure<int>(
                Error.InternalServerError);
        }
    }

    public async Task<Result> UpdateAsync(
        UpdateProductViewModel request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.SalePrice <= request.PurchasePrice)
            {
                return Result.Failure(
                    ProductErrors.InvalidSalePrice);
            }

            using var connection = _context.CreateConnection();

            var command = new CommandDefinition(
                "dbo.usp_Product_Update",
                new
                {
                    request.ProductId,
                    request.Name,
                    request.PurchasePrice,
                    request.SalePrice,
                    request.Quantity
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var affectedRows =
                await connection.ExecuteScalarAsync<int>(command);

            if (affectedRows == 0)
            {
                return Result.Failure(
                    ProductErrors.NotFound);
            }

            _logger.LogInformation(
                "Product {ProductId} updated successfully.",
                request.ProductId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred while updating product {ProductId}.",
                request.ProductId);

            return Result.Failure(
                Error.InternalServerError);
        }
    }

    public async Task<Result> DeleteAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _context.CreateConnection();

            var command = new CommandDefinition(
                "dbo.usp_Product_Delete",
                new
                {
                    ProductId = productId
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var affectedRows =
                await connection.ExecuteScalarAsync<int>(command);

            if (affectedRows == 0)
            {
                return Result.Failure(
                    ProductErrors.NotFound);
            }

            _logger.LogInformation(
                "Product {ProductId} deleted successfully.",
                productId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred while deleting product {ProductId}.",
                productId);

            return Result.Failure(
                Error.InternalServerError);
        }
    }

    public async Task<Result<ProductViewModel>> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _context.CreateConnection();

            var command = new CommandDefinition(
                "dbo.usp_Product_GetById",
                new
                {
                    ProductId = productId
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var product =
                await connection.QuerySingleOrDefaultAsync<ProductViewModel>(
                    command);

            if (product is null)
            {
                return Result.Failure<ProductViewModel>(
                    ProductErrors.NotFound);
            }

            return Result.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred while retrieving product {ProductId}.",
                productId);

            return Result.Failure<ProductViewModel>(
                Error.InternalServerError);
        }
    }

    public async Task<Result<IReadOnlyCollection<ProductViewModel>>> GetAsync(
        GetProductsViewModel request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _context.CreateConnection();

            var command = new CommandDefinition(
                "dbo.usp_Product_Get",
                new
                {
                    request.Name,
                    OrderBy = request.OrderBy ?? "id",
                    SortOrder = request.SortOrder ?? "asc"
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var products =
                (await connection.QueryAsync<ProductViewModel>(command))
                .ToList();

            return Result.Success<IReadOnlyCollection<ProductViewModel>>(
                products);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred while retrieving products.");

            return Result.Failure<IReadOnlyCollection<ProductViewModel>>(
                Error.InternalServerError);
        }
    }

    public async Task<Result<PaginatedList<ProductViewModel>>> GetPaginatedAsync(
        GetProductsPageViewModel request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _context.CreateConnection();

            var command = new CommandDefinition(
                "dbo.usp_Product_GetPaginated",
                new
                {
                    request.Name,
                    OrderBy = request.OrderBy ?? "id",
                    SortOrder = request.SortOrder ?? "asc",
                    request.PageNumber,
                    request.PageSize
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            using var multi = await connection.QueryMultipleAsync(command);

            var products = (await multi.ReadAsync<ProductViewModel>())
                .ToList();

            var totalCount = await multi.ReadSingleAsync<int>();

            var result = new PaginatedList<ProductViewModel>(
                products,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred while retrieving paginated products.");

            return Result.Failure<PaginatedList<ProductViewModel>>(
                Error.InternalServerError);
        }
    }
}