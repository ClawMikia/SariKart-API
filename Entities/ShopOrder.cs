using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class ShopOrder
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public int OrderStatusId { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactAddress { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        [ForeignKey("OrderStatusId")]
        public OrderStatus? OrderStatus { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}
