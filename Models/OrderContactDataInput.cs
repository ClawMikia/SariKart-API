/*
    This class model can be used as OrderContactDataInput
*/


namespace SariKartAPI.Models
{
    public class OrderContactDataInput
    {
        public int OrderId { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string ContactAddress { get; set; }
    }
}
