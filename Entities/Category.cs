using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Column("Category")]
        public string? Category1 { get; set; }

        public string? Icon { get; set; }
    }
}
