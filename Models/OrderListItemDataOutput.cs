/*
    This class model can be used as OrderListItemDataOutput
*/

namespace SariKartAPI.Models
{
    public class OrderListItemDataOutput
    {
        public int OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public string ContactPerson { get; set; } = null!;

        public string ContactNumber { get; set; } = null!;

        public string ContactAddress { get; set; } = null!;
        public string Products { get; set; }
    }
}
