using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserTypeId { get; set; }
        public string? EditableContactPerson { get; set; }
        public string? EditableContactNumber { get; set; }
        public string? EditableContactAddress { get; set; }

        [ForeignKey("UserTypeId")]
        public UserType? UserType { get; set; }

        public ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();
    }
}
