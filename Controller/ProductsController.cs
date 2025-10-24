using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstoqueApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser um valor não-negativo.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
