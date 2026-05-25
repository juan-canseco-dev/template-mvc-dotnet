using LoginMVC.Shared.Pagination;
using LoginMVC.Shared.Results;
using LoginMVC.ViewModels;

namespace LoginMVC.Services.Interfaces;

public interface IProductService
{
    Task<Result<int>> CreateAsync(
        CreateProductViewModel request, 
        CancellationToken cancellationToken = default
    );

    Task<Result> UpdateAsync(
        UpdateProductViewModel request,
        CancellationToken cancellationToken = default
    );

    Task<Result> DeleteAsync(
       int productId,
       CancellationToken cancellationToken = default
   );

    Task<Result<ProductViewModel>> GetByIdAsync(
        int productId, 
        CancellationToken cancellationToken = default
    );

    Task<Result<IReadOnlyCollection<ProductViewModel>>> GetAsync(
        GetProductsViewModel request, 
        CancellationToken cancellationToken = default
    );


    Task<Result<PaginatedList<ProductViewModel>>> GetPaginatedAsync(
        GetProductsPageViewModel request,
        CancellationToken cancellationToken = default
    );

}

