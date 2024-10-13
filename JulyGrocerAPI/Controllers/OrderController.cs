using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JulyGrocerAPI.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : Controller
    {
        [HttpGet("{userId}/{orderStatusId}")]
        public Result GetUserOrders(int userId, int orderStatusId)
        {
            var result = new Result();

            try
            {
                var orders = new List<ShopOrder>();

                using (var db = new JulyGrocerContext())
                {
                    orders = db.ShopOrders.Where(x => x.UserId == userId && x.OrderStatusId == orderStatusId).ToList();
                    
                    result.JsonResultObject = orders;
                    result.Message = "You get all your orders";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all your orders";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("orderStatus/{orderStatusId}")]
        public Result GetOrdersByStatus(int orderStatusId)
        {
            var result = new Result();

            try
            {
                var orders = new List<ShopOrder>();

                using (var db = new JulyGrocerContext())
                {
                    orders = db.ShopOrders.Where(x => x.OrderStatusId == orderStatusId).ToList();

                    result.JsonResultObject = orders;
                    result.Message = "You get all your orders";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all your orders";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("{orderId}")]
        public Result GetOrder(int orderId)
        {
            var result = new Result();

            try
            {
                var order = new ShopOrder();

                using (var db = new JulyGrocerContext())
                {
                    order = db.ShopOrders.Where(x => x.Id == orderId).Include(x => x.OrderStatus).Include(x => x.OrderLines).ThenInclude(x => x.Product).FirstOrDefault();

                    result.JsonResultObject = order;
                    result.Message = "You get all your current order";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get all your current order";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpGet("orderStatus")]
        public Result GetOrderStatuses()
        {
            var result = new Result();

            try
            {
                var orderStatuses = new List<OrderStatus>();

                using (var db = new JulyGrocerContext())
                {
                    orderStatuses = db.OrderStatuses.ToList();

                    result.JsonResultObject = orderStatuses;
                    result.Message = "You get order statuses";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to get order statuses";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPost("add")]
        public Result PlaceOrder([FromBody] List<OrderLineDataInput> orderLineDataInputs)
        {
            var result = new Result();
            var userId = orderLineDataInputs.Select(X => X.userId).FirstOrDefault();
            var totalAmount = orderLineDataInputs.Sum(x => x.TotalAmount);

            try
            {
                using (var db = new JulyGrocerContext())
                {
                    var order = new ShopOrder();

                    order.UserId = userId;
                    order.TotalAmount = totalAmount;

                    db.Add(order);
                    db.SaveChanges();

                    foreach (var item in orderLineDataInputs)
                    {
                        var product = db.Products.Where(x => x.Id == item.ProductId).FirstOrDefault();

                        product.Stock = product.Stock - item.Quantity;

                        db.SaveChanges();

                        var orderLine = new OrderLine();

                        orderLine.OrderId = order.Id;
                        orderLine.ProductId = item.ProductId;
                        orderLine.Quantity = item.Quantity;

                        db.Add(orderLine);
                        db.SaveChanges();
                    }

                    result.Message = "You successfully placed an order";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to place your order";
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
