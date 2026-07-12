using SariKartAPI.Entities;
using SariKartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
    This controller performs different functions for delivery
 */

namespace SariKartAPI.Controllers
{
    [ApiController]
    [Route("api/delivery")]
    public class DeliveryController : Controller
    {
        private readonly SariKartContext _db;

        public DeliveryController(SariKartContext db)
        {
            _db = db;
        }

        [HttpGet("{orderId}")]
        public Result GetOrderDelivery(int orderId) // This route gets an order delivery by order
        {
            var result = new Result();

            try
            {
                var delivery = _db.Deliveries.AsNoTracking().FirstOrDefault(x => x.OrderId == orderId);

                result.JsonResultObject = delivery;
                result.Message = "You get current delivery";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get current delivery";
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
