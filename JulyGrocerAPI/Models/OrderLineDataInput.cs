namespace JulyGrocerAPI.Models
{
    public class OrderLineDataInput
    {
        public int ProductId { get; set; }
        public string Product { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int userId { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string ContactAddress { get; set; }
    }
}
