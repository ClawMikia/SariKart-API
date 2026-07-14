namespace SariKartAPIV2.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? JsonResultObject { get; set; }
    }
}
