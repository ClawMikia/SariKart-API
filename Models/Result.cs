/*
 * This class contains results from the API
 */

namespace SariKartAPI.Models
{
    public class Result
    {
        public object JsonResultObject { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
