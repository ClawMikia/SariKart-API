using Microsoft.AspNetCore.Http;

namespace SariKartAPIV2.Models
{
    public class CategoryDataInput
    {
        public int? Id { get; set; }
        public string? Category { get; set; }
        public IFormFile? Icon { get; set; }
    }
}
