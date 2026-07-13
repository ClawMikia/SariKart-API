/*
    This class model can be used as OrderDeliveryDataInput
*/

namespace SariKartAPI.Models
{
    public class OrderDeliveryDataInput
    {
        public int OrderId { get; set; }
        public int StoreBranchId { get; set; }
        public int RiderId { get; set; }
        public int VehicleId { get; set; }
        public string DeliveryDate { get; set; }
        public bool IsDelivered { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public int OrderStatusId { get; set; }
    }
}
