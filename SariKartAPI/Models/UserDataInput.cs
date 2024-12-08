/*
    This class model can be used as UserDataInput
*/

namespace SariKartAPI.Models
{
    public class UserDataInput
    {
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }
        public string? ContactNumber { get; set; }
        public string? EditableContactPerson { get; set; }
        public string? EditableContactNumber { get; set; }
        public string? EditableContactAddress { get; set; }
        public UserStoreDataInput? UserStore { get; set; }
    }
}
