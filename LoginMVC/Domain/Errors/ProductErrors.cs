using LoginMVC.Shared.Results;

namespace LoginMVC.Domain.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "The product was not found.");
    public static readonly Error InvalidSalePrice = new("Product.InvalidSalePrice", "The sale price must be greater than the purchase price.");
}
