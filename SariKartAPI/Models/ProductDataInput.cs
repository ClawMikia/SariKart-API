/*
    This class model can be used as ProductDataInput
*/

namespace SariKartAPI.Models
{
    public class ProductDataInput
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public int CategoryId { get; set; }
        public string FileName { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public int Stock { get; set; }
    }
}
