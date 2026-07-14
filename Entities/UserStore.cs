using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class UserStore
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public string? Address { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
    }
}
