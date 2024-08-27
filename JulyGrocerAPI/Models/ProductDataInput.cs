namespace JulyGrocerAPI.Models
{
    public class ProductDataInput
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public byte[] Picture { get; set; }
        public string Unit { get; set; }
        public int Stock { get; set; }
    }
}
