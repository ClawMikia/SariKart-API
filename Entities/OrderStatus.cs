using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }

        [Column("OrderStatus")]
        public string? OrderStatus1 { get; set; }
    }
}
