using System.ComponentModel.DataAnnotations;

namespace EstoqueApi.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }
    }

    public class ProductUpdateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }
    }
}
