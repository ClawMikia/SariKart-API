namespace SariKartAPIV2.Models
{
    public class OrderLineDataInput
    {
        public int userId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactAddress { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
