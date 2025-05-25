namespace Project1.Models
{
    public class ProductCategoryViewModel
    {
        public ProductCategory NewCategory { get; set; } = new ProductCategory();
        public List<ProductCategory> ExistingCategories { get; set; } = new List<ProductCategory>();
    }
}
