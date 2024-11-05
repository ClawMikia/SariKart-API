using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
                    orders = db.ShopOrders.Include(x => x.User).Include(x => x.OrderLines).ThenInclude(x => x.Product).Where(x => x.UserId == userId && x.OrderStatusId == orderStatusId).ToList();

                    if (orderStatusId == 4)
                    {
                        orders = orders.OrderBy(x => x.CreateDate).ToList();
                    
                    }

                    else
                    {
                        orders = orders.OrderByDescending(x => x.CreateDate).ToList();
                    }

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

        [HttpPost("adminOrders")]
        public Result GetOrdersByFilter(SearchOrderFilterDataInput searchOrderFilterDataInput)
        {
            var result = new Result();

            try
            {
                var orders = new List<ShopOrder>();

                using (var db = new JulyGrocerContext())
                {
                    orders = db.ShopOrders.Include(x => x.User).Include(x => x.OrderLines).ThenInclude(x => x.Product)
                        .Where(x => x.OrderStatusId == searchOrderFilterDataInput.orderStatusId && 
                        (x.User.FirstName.Contains(searchOrderFilterDataInput.customerName) || 
                        x.User.LastName.Contains(searchOrderFilterDataInput.customerName))).ToList();

                    if (searchOrderFilterDataInput.orderStatusId == 4)
                    {
                        orders = orders.OrderBy(x => x.CreateDate).ToList();
                    }

                    else
                    {
                        orders = orders.OrderByDescending(x => x.CreateDate).ToList();
                    }

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
                    var user = db.AppUsers.Where(x => x.Id == userId).FirstOrDefault();
                    var store = db.UserStores.Where(x => x.UserId == userId).FirstOrDefault();

                    var order = new ShopOrder();

                    order.UserId = userId;
                    order.TotalAmount = totalAmount;
                    order.ContactPerson = user.FirstName + " " + user.LastName;
                    order.ContactNumber = user.ContactNumber;
                    order.ContactAddress = store.Address;

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

        [HttpPost("delivery/save")]
        public Result UpdateOrderDelivery([FromBody] OrderDeliveryDataInput orderDeliveryDataInput)
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (orderDeliveryDataInput.StoreBranchId < 1 || orderDeliveryDataInput.RiderId < 1 || orderDeliveryDataInput.VehicleId < 1 ||
                    orderDeliveryDataInput.OrderStatusId < 1 || orderDeliveryDataInput.DeliveryDate.IsNullOrEmpty())
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                // Validate if amount paid is entered
                //if (orderDeliveryDataInput.AmountPaid <= 0)
                //{
                //    result.Message = "Amount paid should be entered";
                //    result.IsSuccess = false;

                //    return result;
                //}

                using (var db = new JulyGrocerContext())
                {
                    var delivery = db.Deliveries.Where(x => x.OrderId == orderDeliveryDataInput.OrderId).FirstOrDefault();

                    if (delivery == null)
                    {
                        delivery = new Delivery();

                        delivery.OrderId = orderDeliveryDataInput.OrderId;
                        delivery.StoreId = orderDeliveryDataInput.StoreBranchId;
                        delivery.RiderId = orderDeliveryDataInput.RiderId;
                        delivery.VehicleId = orderDeliveryDataInput.VehicleId;
                        delivery.Delivered = orderDeliveryDataInput.IsDelivered;
                        delivery.DeliveryDate = DateTime.ParseExact(orderDeliveryDataInput.DeliveryDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        db.Add(delivery);
                        db.SaveChanges();
                    }

                    else
                    {
                        delivery.OrderId = orderDeliveryDataInput.OrderId;
                        delivery.StoreId = orderDeliveryDataInput.StoreBranchId;
                        delivery.RiderId = orderDeliveryDataInput.RiderId;
                        delivery.VehicleId = orderDeliveryDataInput.VehicleId;
                        delivery.Delivered = orderDeliveryDataInput.IsDelivered;
                        delivery.DeliveryDate = DateTime.ParseExact(orderDeliveryDataInput.DeliveryDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        db.SaveChanges();
                    }

                    var order = db.ShopOrders.Where(x => x.Id == orderDeliveryDataInput.OrderId).FirstOrDefault();

                    order.OrderStatusId = orderDeliveryDataInput.OrderStatusId;
                    order.AmountPaid = orderDeliveryDataInput.AmountPaid;
                    order.Change = orderDeliveryDataInput.Change;

                    db.SaveChanges();

                    result.Message = "You successfully updated a current delivery";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to update a current delivery";
                result.IsSuccess = false;

                return result;
            }

            return null;
        }

        [HttpGet("delivery/{orderId}")]
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
                    result.Message = "You get current order delivery";
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

        [HttpPut("followUp/{orderId}")]
        public Result FollowUpOrder(int orderId)
        {
            var result = new Result();

            try
            {
                using (var db = new JulyGrocerContext())
                {
                    var order = db.ShopOrders.Where(x => x.Id == orderId).FirstOrDefault();

                    if (order.OrderStatusId == 4)
                    {
                        result.Message = "This order is already followed-up";
                        result.IsSuccess = true;

                        return result;
                    }

                    order.OrderStatusId = 4;

                    db.SaveChanges();

                    result.Message = "You order has been followed-up";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to follow-up your order";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("cancel/{orderId}")]
        public Result CancelOrder(int orderId)
        {
            var result = new Result();

            try
            {
                using (var db = new JulyGrocerContext())
                {
                    var order = db.ShopOrders.Where(x => x.Id == orderId).FirstOrDefault();

                    if (order.OrderStatusId == 3)
                    {
                        result.Message = "This order is already cancelled";
                        result.IsSuccess = true;

                        return result;
                    }

                    order.OrderStatusId = 3;

                    db.SaveChanges();

                    result.Message = "You order has been cancelled";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to cancel your order";
                result.IsSuccess = false;

                return result;
            }
        }

        [HttpPut("editOrderContact")]
        public Result EditOrderContactInfo([FromBody] OrderContactDataInput orderContactDataInput)
        {
            var result = new Result();

            try
            {
                // Validate if all inputs are entered
                if (orderContactDataInput.ContactPerson.IsNullOrEmpty() || orderContactDataInput.ContactNumber.IsNullOrEmpty() || orderContactDataInput.ContactAddress.IsNullOrEmpty())
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                using (var db = new JulyGrocerContext())
                {
                    var order = db.ShopOrders.Where(x => x.Id == orderContactDataInput.OrderId).FirstOrDefault();

                    order.ContactPerson = orderContactDataInput.ContactPerson;
                    order.ContactNumber = orderContactDataInput.ContactNumber;
                    order.ContactAddress = orderContactDataInput.ContactAddress;

                    db.SaveChanges();

                    result.Message = "You contact information for this order is successfully updated";
                    result.IsSuccess = true;

                    return result;
                }
            }

            catch
            {
                result.Message = "Unable to update the contact information of your current order";
                result.IsSuccess = false;

                return result;
            }
        }
    }
}
