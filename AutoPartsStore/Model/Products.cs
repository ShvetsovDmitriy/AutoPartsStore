
namespace AutoPartsStore.Model
{
    public class Products
    {
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public List<OrderItems>? OrderItems { get; set; }

        public ProductTypes? ProductType { get; set; } 

    }
}
