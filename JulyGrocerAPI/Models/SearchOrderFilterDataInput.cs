/*
    This class model can be used as SearchOrderFilterDataInput
*/

namespace JulyGrocerAPI.Models
{
    public class SearchOrderFilterDataInput
    {
        public string? customerName { get; set; }
        public int orderStatusId { get; set; }
    }
}
