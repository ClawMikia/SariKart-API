using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SariKartAPIV2.Entities
{
    public class UserType
    {
        [Key]
        public int Id { get; set; }

        [Column("UserType")]
        public string? UserType1 { get; set; }
    }
}
