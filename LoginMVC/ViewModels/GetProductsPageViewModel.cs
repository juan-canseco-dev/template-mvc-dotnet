using System.ComponentModel.DataAnnotations;

namespace LoginMVC.ViewModels;

public class GetProductsPageViewModel : GetProductsViewModel
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}
