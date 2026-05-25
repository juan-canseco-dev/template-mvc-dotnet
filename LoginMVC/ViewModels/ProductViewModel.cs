namespace LoginMVC.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
}
