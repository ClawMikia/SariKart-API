using JulyGrocerAPI.Entities;
using JulyGrocerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

/*
    This controller performs different functions for order 
*/

namespace JulyGrocerAPI.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : Controller
    {
        [HttpGet("{userId}/{orderStatusId}")]
        public Result GetUserOrders(int userId, int orderStatusId) // This route gets user orders by search filter
        {
            var result = new Result();

            try
            {
                using (var db = new JulyGrocerContext())
                {
                    var orders = new List<OrderListItemDataOutput>();

                    var query = from o in db.ShopOrders
                                 join u in db.AppUsers on o.UserId equals u.Id
                                join ol in db.OrderLines on o.Id equals ol.OrderId
                                join p in db.Products on ol.ProductId equals p.Id
                                where o.UserId == userId
                                where o.OrderStatusId == orderStatusId
                                select new OrderListItemDataOutput
                                {
                                    OrderId = o.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    CreateDate = o.CreateDate,
                                    TotalAmount = o.TotalAmount,
                                    AmountPaid = o.AmountPaid,
                                    Change = o.Change,
                                    ContactPerson = o.ContactPerson,
                                    ContactNumber = o.ContactNumber,
                                    ContactAddress = o.ContactAddress,
                                    Products = p.Product1
                                };

                    foreach (var item in query)
                    {
                        orders.Add(item);
                    }

                    orders = orders
                        .GroupBy(x => new { 
                            x.OrderId, 
                            x.FirstName, 
                            x.LastName, 
                            x.CreateDate, 
                            x.TotalAmount, 
                            x.AmountPaid, 
                            x.Change,
                            x.ContactPerson,
                            x.ContactNumber,
                            x.ContactAddress
                        })
                        .Select(o => new OrderListItemDataOutput
                        {
                            OrderId = o.Key.OrderId,
                            FirstName = o.Key.FirstName,
                            LastName = o.Key.LastName,
                            CreateDate = o.Key.CreateDate,
                            TotalAmount = o.Key.TotalAmount,
                            AmountPaid = o.Key.AmountPaid,
                            Change = o.Key.Change,
                            ContactPerson = o.Key.ContactPerson,
                            ContactNumber = o.Key.ContactNumber,
                            ContactAddress = o.Key.ContactAddress,
                            Products = string.Join(", ", o.Select(o => o.Products))
                        }).ToList();

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
        public Result GetOrdersByFilter(SearchOrderFilterDataInput searchOrderFilterDataInput) // This route gets all orders by search filter
        {
            var result = new Result();

            try
            {

                using (var db = new JulyGrocerContext())
                {
                    var orders = new List<OrderListItemDataOutput>();

                    var query = from o in db.ShopOrders
                                join u in db.AppUsers on o.UserId equals u.Id
                                join ol in db.OrderLines on o.Id equals ol.OrderId
                                join p in db.Products on ol.ProductId equals p.Id
                                where o.OrderStatusId == searchOrderFilterDataInput.orderStatusId
                                where u.FirstName.Contains(searchOrderFilterDataInput.customerName) || u.LastName.Contains(searchOrderFilterDataInput.customerName)
                                select new OrderListItemDataOutput
                                {
                                    OrderId = o.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    CreateDate = o.CreateDate,
                                    TotalAmount = o.TotalAmount,
                                    AmountPaid = o.AmountPaid,
                                    Change = o.Change,
                                    ContactPerson = o.ContactPerson,
                                    ContactNumber = o.ContactNumber,
                                    ContactAddress = o.ContactAddress,
                                    Products = p.Product1
                                };

                    foreach (var item in query)
                    {
                        orders.Add(item);
                    }

                    orders = orders
                        .GroupBy(x => new {
                            x.OrderId,
                            x.FirstName,
                            x.LastName,
                            x.CreateDate,
                            x.TotalAmount,
                            x.AmountPaid,
                            x.Change,
                            x.ContactPerson,
                            x.ContactNumber,
                            x.ContactAddress
                        })
                        .Select(o => new OrderListItemDataOutput
                        {
                            OrderId = o.Key.OrderId,
                            FirstName = o.Key.FirstName,
                            LastName = o.Key.LastName,
                            CreateDate = o.Key.CreateDate,
                            TotalAmount = o.Key.TotalAmount,
                            AmountPaid = o.Key.AmountPaid,
                            Change = o.Key.Change,
                            ContactPerson = o.Key.ContactPerson,
                            ContactNumber = o.Key.ContactNumber,
                            ContactAddress = o.Key.ContactAddress,
                            Products = string.Join(", ", o.Select(o => o.Products))
                        }).ToList();

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
        public Result GetOrder(int orderId) // This route gets a specific order
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
        public Result GetOrderStatuses() // This route gets all order statuses
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

        [HttpPost("add/{isCashOnHand}")]
        public Result PlaceOrder(bool isCashOnHand, [FromBody] List<OrderLineDataInput> orderLineDataInputs) // This route updates current order delivery's cash on hand and creates an order
        {
            var result = new Result();

            // If the customer doesn't add product into cart, notify the customer
            if (orderLineDataInputs.Count == 0)
            {
                result.Message = "You have no order, please add items for order";
                result.IsSuccess = false;

                return result;
            }

            var userId = orderLineDataInputs.Select(X => X.userId).FirstOrDefault();
            var totalAmount = orderLineDataInputs.Sum(x => x.TotalAmount);

            var contactPerson = orderLineDataInputs.Select(X => X.ContactPerson).FirstOrDefault();
            var contactNumber = orderLineDataInputs.Select(X => X.ContactNumber).FirstOrDefault();
            var contactAddress = orderLineDataInputs.Select(X => X.ContactAddress).FirstOrDefault();

            try
            {
                using (var db = new JulyGrocerContext())
                {
                    var user = db.AppUsers.Where(x => x.Id == userId).FirstOrDefault();
                    
                    var order = new ShopOrder();

                    order.UserId = userId;
                    order.TotalAmount = totalAmount;
                    order.ContactPerson = contactPerson;
                    order.ContactNumber = contactNumber;
                    order.ContactAddress = contactAddress;

                    db.Add(order);
                    db.SaveChanges();

                    // Add every order line in the order
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

                    // Add new delivery
                    var delivery = new Delivery();

                    delivery.OrderId = order.Id;
                    delivery.StoreId = 1;
                    delivery.RiderId = 1003;
                    delivery.VehicleId = 1;
                    delivery.Delivered = false;
                    delivery.CashOnHand = isCashOnHand;
                    delivery.DeliveryDate = DateTime.Now;

                    db.Add(delivery);
                    db.SaveChanges();

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
        public Result UpdateOrderDelivery([FromBody] OrderDeliveryDataInput orderDeliveryDataInput) // This route updates current order delivery 
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

                // Get the order total amount
                using (var db = new JulyGrocerContext())
                {
                    var totalAmount = db.ShopOrders.Where(o => o.Id == orderDeliveryDataInput.OrderId).Select(
                            x => new
                            {
                                x.TotalAmount
                            }
                        ).FirstOrDefault();

                    // Check if the amount paid should be greater that total amount
                    if (totalAmount.TotalAmount > orderDeliveryDataInput.AmountPaid)
                    {
                        result.Message = "The amount paid should be higher than total amount";
                        result.IsSuccess = false;

                        return result;
                    }
                }

                //Validate if amount paid is entered
                if (orderDeliveryDataInput.AmountPaid <= 0)
                {
                    result.Message = "Amount paid should be entered";
                    result.IsSuccess = false;

                    return result;
                }

                // Update delivery
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
        public Result GetOrderDelivery(int orderId) // This route gets the current order delivery
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
        public Result FollowUpOrder(int orderId) // This route updates the order to follow-up
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
        public Result CancelOrder(int orderId) // This method cancels current order
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
        public Result EditOrderContactInfo([FromBody] OrderContactDataInput orderContactDataInput) // This route updates current order contact details
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
