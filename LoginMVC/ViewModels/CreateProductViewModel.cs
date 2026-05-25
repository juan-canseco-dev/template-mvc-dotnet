using System.ComponentModel.DataAnnotations;

namespace LoginMVC.ViewModels;

public class CreateProductViewModel : IValidatableObject
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string? Name { get; set; }

    [Range(typeof(decimal), "0.01", "999999999.99")]
    public decimal SalePrice { get; set; }

    [Range(typeof(decimal), "0.01", "999999999.99")]
    public decimal PurchasePrice { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SalePrice < PurchasePrice)
        {
            yield return new ValidationResult(
                "Sale price cannot be less than purchase price.",
                [nameof(SalePrice)]);
        }
    }
}