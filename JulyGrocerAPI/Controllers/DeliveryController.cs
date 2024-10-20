using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JulyGrocerAPI.Controllers
{
    [ApiController]
    [Route("api/delivery")]
    public class DeliveryController : Controller
    {
        [HttpGet("{orderId}")]
        public Result GetOrderDelivery(int orderId)
        {
            var result = new Result();

            try
            {
                var delivery = new Delivery();

                using (var db = new JulyGrocerContext())
                {
                    delivery = db.Deliveries.Where(x => x.OrderId == orderId).FirstOrDefault();

                    result.JsonResultObject = delivery;
                    result.Message = "You get current delivery";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get current delivery";
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
