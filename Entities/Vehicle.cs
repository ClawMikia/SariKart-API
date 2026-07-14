using System.ComponentModel.DataAnnotations;

namespace SariKartAPIV2.Entities
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        public string? Type { get; set; }
    }
}
