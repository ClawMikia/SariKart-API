using System.ComponentModel.DataAnnotations;

namespace SariKartAPIV2.Entities
{
    public class StoreBranch
    {
        [Key]
        public int Id { get; set; }
        public string? Branch { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
    }
}
