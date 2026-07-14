using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Column("Product")]
        public string? Product1 { get; set; }

        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
        public int Stock { get; set; }
        public string? Picture { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
