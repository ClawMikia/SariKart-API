using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class OrderLine
    {
        [Key]
        public int OrderLineId { get; set; }

        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public ShopOrder? Order { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
