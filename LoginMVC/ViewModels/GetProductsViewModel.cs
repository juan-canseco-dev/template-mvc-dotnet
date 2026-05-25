using System.ComponentModel.DataAnnotations;

namespace LoginMVC.ViewModels;

public class GetProductsViewModel : IValidatableObject
{
    private static readonly HashSet<string> AllowedOrderByFields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "id",
            "name",
            "purchaseprice",
            "saleprice",
            "quantity"
        };

    private static readonly HashSet<string> AllowedSortOrders =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "asc",
            "desc"
        };

    public string? Name { get; set; }

    public string? OrderBy { get; set; }

    public string? SortOrder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(OrderBy) &&
            !AllowedOrderByFields.Contains(OrderBy))
        {
            yield return new ValidationResult(
                $"OrderBy must be one of: {string.Join(", ", AllowedOrderByFields)}.",
                new[] { nameof(OrderBy) });
        }

        if (!string.IsNullOrWhiteSpace(SortOrder) &&
            !AllowedSortOrders.Contains(SortOrder))
        {
            yield return new ValidationResult(
                "SortOrder must be either 'asc' or 'desc'.",
                new[] { nameof(SortOrder) });
        }
    }
}