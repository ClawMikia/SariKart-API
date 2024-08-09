/*
 * This class contains results from the API
 */

namespace JulyGrocerAPI.Models
{
    public class Result
    {
        public object JsonResultObject { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
