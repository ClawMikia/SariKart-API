/*
    This class model can be used as UserStoreDataInput
*/

namespace JulyGrocerAPI.Models
{
    public class UserStoreDataInput
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Address { get; set; }
    }
}
