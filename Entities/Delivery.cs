using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class Delivery
    {
        [Key]
        public int Id { get; set; }

        public int StoreId { get; set; }
        public int OrderId { get; set; }
        public int RiderId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool Delivered { get; set; }
        public int VehicleId { get; set; }
        public bool CashOnHand { get; set; }

        [ForeignKey("OrderId")]
        public ShopOrder? Order { get; set; }
    }
}
