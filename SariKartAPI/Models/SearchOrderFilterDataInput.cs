/*
    This class model can be used as SearchOrderFilterDataInput
*/

namespace SariKartAPI.Models
{
    public class SearchOrderFilterDataInput
    {
        public string? customerName { get; set; }
        public int orderStatusId { get; set; }
    }
}
