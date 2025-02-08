/*
    This class model can be used as CategoryDataInput
*/

namespace SariKartAPI.Models
{
    public class CategoryDataInput
    {
        public int? Id { get; set; }
        public string? Category { get; set; }
        public IFormFile? Icon { get; set; }

    }
}
