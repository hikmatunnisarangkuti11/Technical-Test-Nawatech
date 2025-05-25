using System.ComponentModel.DataAnnotations;

namespace Project1.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name wajib diisi")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price wajib diisi")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price harus lebih dari 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category wajib dipilih")]
        public int ProductCategoryId { get; set; }

        public string? Picture { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ProductCategory? ProductCategory { get; set; }

        public int Stock { get; set; }

    }
}
