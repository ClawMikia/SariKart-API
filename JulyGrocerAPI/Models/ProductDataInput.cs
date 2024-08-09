namespace JulyGrocerAPI.Models
{
    public class ProductDataInput
    {
        public string Product { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public byte[] Picture { get; set; }
        public string Unit { get; set; }
    }
}
