namespace LoginMVC.Domain.Entities;

public class Product : Entity<int>
{
    public string? Name { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public Product() { }
}
