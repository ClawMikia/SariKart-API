using SariKartAPIV2.Entities;
using SariKartAPIV2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

/*
    This controller performs different functions for order
 */

namespace SariKartAPIV2.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly SariKartContext _db;

        public OrderController(SariKartContext db)
        {
            _db = db;
        }

        [HttpGet("{userId:int}/{orderStatusId:int}")]
        public Result GetUserOrders(int userId, int orderStatusId)
        {
            var result = new Result();

            try
            {
                var orders = _db.ShopOrders.AsNoTracking()
                    .Where(o => o.UserId == userId && o.OrderStatusId == orderStatusId)
                    .OrderByDescending(o => o.CreateDate)
                    .SelectMany(o => o.OrderLines.Select(ol => new OrderListItemDataOutput
                    {
                        OrderId = o.Id,
                        FirstName = o.User.FirstName,
                        LastName = o.User.LastName,
                        CreateDate = o.CreateDate,
                        TotalAmount = o.TotalAmount,
                        AmountPaid = o.AmountPaid,
                        Change = o.Change,
                        ContactPerson = o.ContactPerson,
                        ContactNumber = o.ContactNumber,
                        ContactAddress = o.ContactAddress,
                        Products = ol.Product.Product1
                    }))
                    .ToList()
                    .GroupBy(x => new
                    {
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
                    .Select(g => new OrderListItemDataOutput
                    {
                        OrderId = g.Key.OrderId,
                        FirstName = g.Key.FirstName,
                        LastName = g.Key.LastName,
                        CreateDate = g.Key.CreateDate,
                        TotalAmount = g.Key.TotalAmount,
                        AmountPaid = g.Key.AmountPaid,
                        Change = g.Key.Change,
                        ContactPerson = g.Key.ContactPerson,
                        ContactNumber = g.Key.ContactNumber,
                        ContactAddress = g.Key.ContactAddress,
                        Products = string.Join(", ", g.Select(o => o.Products))
                    })
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();

                result.JsonResultObject = orders;
                result.Message = "You get all your orders";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all your orders";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("adminOrders")]
        public Result GetOrdersByFilter(SearchOrderFilterDataInput searchOrderFilterDataInput)
        {
            var result = new Result();

            try
            {
                var name = searchOrderFilterDataInput.customerName ?? string.Empty;

                var orders = _db.ShopOrders.AsNoTracking()
                    .Where(o => o.OrderStatusId == searchOrderFilterDataInput.orderStatusId)
                    .Where(o => o.User.FirstName.Contains(name) || o.User.LastName.Contains(name))
                    .SelectMany(o => o.OrderLines.Select(ol => new OrderListItemDataOutput
                    {
                        OrderId = o.Id,
                        FirstName = o.User.FirstName,
                        LastName = o.User.LastName,
                        CreateDate = o.CreateDate,
                        TotalAmount = o.TotalAmount,
                        AmountPaid = o.AmountPaid,
                        Change = o.Change,
                        ContactPerson = o.ContactPerson,
                        ContactNumber = o.ContactNumber,
                        ContactAddress = o.ContactAddress,
                        Products = ol.Product.Product1
                    }))
                    .ToList()
                    .GroupBy(x => new
                    {
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
                    .Select(g => new OrderListItemDataOutput
                    {
                        OrderId = g.Key.OrderId,
                        FirstName = g.Key.FirstName,
                        LastName = g.Key.LastName,
                        CreateDate = g.Key.CreateDate,
                        TotalAmount = g.Key.TotalAmount,
                        AmountPaid = g.Key.AmountPaid,
                        Change = g.Key.Change,
                        ContactPerson = g.Key.ContactPerson,
                        ContactNumber = g.Key.ContactNumber,
                        ContactAddress = g.Key.ContactAddress,
                        Products = string.Join(", ", g.Select(o => o.Products))
                    })
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();

                result.JsonResultObject = orders;
                result.Message = "You get all your orders";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all your orders";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("{orderId:int}")]
        public Result GetOrder(int orderId)
        {
            var result = new Result();

            try
            {
                var order = _db.ShopOrders.AsNoTracking()
                    .Include(x => x.OrderStatus)
                    .Include(x => x.OrderLines).ThenInclude(x => x.Product)
                    .FirstOrDefault(x => x.Id == orderId);

                result.JsonResultObject = order;
                result.Message = "You get all your current order";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get all your current order";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("orderStatus")]
        public Result GetOrderStatuses()
        {
            var result = new Result();

            try
            {
                var orderStatuses = _db.OrderStatuses.AsNoTracking().ToList();

                result.JsonResultObject = orderStatuses;
                result.Message = "You get order statuses";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get order statuses";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("add/{isCashOnHand}")]
        public Result PlaceOrder(bool isCashOnHand, [FromBody] List<OrderLineDataInput> orderLineDataInputs)
        {
            var result = new Result();

            if (orderLineDataInputs == null || orderLineDataInputs.Count == 0)
            {
                result.Message = "You have no order, please add items for order";
                result.IsSuccess = false;

                return result;
            }

            var userId = orderLineDataInputs.Select(X => X.userId).First();
            var totalAmount = orderLineDataInputs.Sum(x => x.TotalAmount);
            var contactPerson = orderLineDataInputs.Select(x => x.ContactPerson).FirstOrDefault() ?? string.Empty;
            var contactNumber = orderLineDataInputs.Select(x => x.ContactNumber).FirstOrDefault() ?? string.Empty;
            var contactAddress = orderLineDataInputs.Select(x => x.ContactAddress).FirstOrDefault() ?? string.Empty;

            try
            {
                var order = new ShopOrder
                {
                    UserId = userId,
                    TotalAmount = totalAmount,
                    ContactPerson = contactPerson,
                    ContactNumber = contactNumber,
                    ContactAddress = contactAddress
                };

                _db.ShopOrders.Add(order);

                foreach (var item in orderLineDataInputs)
                {
                    var product = _db.Products.FirstOrDefault(x => x.Id == item.ProductId);

                    if (product == null)
                    {
                        result.Message = "Product with id " + item.ProductId + " not found";
                        result.IsSuccess = false;

                        return result;
                    }

                    product.Stock -= item.Quantity;

                    _db.OrderLines.Add(new OrderLine
                    {
                        Order = order,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }

                _db.Deliveries.Add(new Delivery
                {
                    Order = order,
                    StoreId = 1,
                    RiderId = 1003,
                    VehicleId = 1,
                    Delivered = false,
                    CashOnHand = isCashOnHand,
                    DeliveryDate = DateTime.Now
                });

                _db.SaveChanges();

                result.Message = "You successfully placed an order";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to place your order";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPost("delivery/save")]
        public Result UpdateOrderDelivery([FromBody] OrderDeliveryDataInput orderDeliveryDataInput)
        {
            var result = new Result();

            try
            {
                if (orderDeliveryDataInput.StoreBranchId < 1 || orderDeliveryDataInput.RiderId < 1 || orderDeliveryDataInput.VehicleId < 1 ||
                    orderDeliveryDataInput.OrderStatusId < 1 || string.IsNullOrEmpty(orderDeliveryDataInput.DeliveryDate))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                if (orderDeliveryDataInput.AmountPaid <= 0)
                {
                    result.Message = "Amount paid should be entered";
                    result.IsSuccess = false;

                    return result;
                }

                var totalAmount = _db.ShopOrders
                    .Where(o => o.Id == orderDeliveryDataInput.OrderId)
                    .Select(x => x.TotalAmount)
                    .FirstOrDefault();

                if (totalAmount > orderDeliveryDataInput.AmountPaid)
                {
                    result.Message = "The amount paid should be higher than total amount";
                    result.IsSuccess = false;

                    return result;
                }

                var delivery = _db.Deliveries.FirstOrDefault(x => x.OrderId == orderDeliveryDataInput.OrderId);

                var deliveryDate = DateTime.ParseExact(orderDeliveryDataInput.DeliveryDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                if (delivery == null)
                {
                    _db.Deliveries.Add(new Delivery
                    {
                        OrderId = orderDeliveryDataInput.OrderId,
                        StoreId = orderDeliveryDataInput.StoreBranchId,
                        RiderId = orderDeliveryDataInput.RiderId,
                        VehicleId = orderDeliveryDataInput.VehicleId,
                        Delivered = orderDeliveryDataInput.IsDelivered,
                        DeliveryDate = deliveryDate
                    });
                }
                else
                {
                    delivery.StoreId = orderDeliveryDataInput.StoreBranchId;
                    delivery.RiderId = orderDeliveryDataInput.RiderId;
                    delivery.VehicleId = orderDeliveryDataInput.VehicleId;
                    delivery.Delivered = orderDeliveryDataInput.IsDelivered;
                    delivery.DeliveryDate = deliveryDate;
                }

                var order = _db.ShopOrders.FirstOrDefault(x => x.Id == orderDeliveryDataInput.OrderId);

                if (order != null)
                {
                    order.OrderStatusId = orderDeliveryDataInput.OrderStatusId;
                    order.AmountPaid = orderDeliveryDataInput.AmountPaid;
                    order.Change = orderDeliveryDataInput.Change;
                }

                _db.SaveChanges();

                result.Message = "You successfully updated a current delivery";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update a current delivery";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpGet("delivery/{orderId}")]
        public Result GetOrderDelivery(int orderId)
        {
            var result = new Result();

            try
            {
                var delivery = _db.Deliveries.AsNoTracking().FirstOrDefault(x => x.OrderId == orderId);

                result.JsonResultObject = delivery;
                result.Message = "You get current order delivery";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to get current delivery";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("followUp/{orderId}")]
        public Result FollowUpOrder(int orderId)
        {
            var result = new Result();

            try
            {
                var order = _db.ShopOrders.FirstOrDefault(x => x.Id == orderId);

                if (order == null)
                {
                    result.Message = "Order not found";
                    result.IsSuccess = false;

                    return result;
                }

                if (order.OrderStatusId == 4)
                {
                    result.Message = "This order is already followed-up";
                    result.IsSuccess = true;

                    return result;
                }

                order.OrderStatusId = 4;

                _db.SaveChanges();

                result.Message = "You order has been followed-up";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to follow-up your order";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("cancel/{orderId}")]
        public Result CancelOrder(int orderId)
        {
            var result = new Result();

            try
            {
                var order = _db.ShopOrders.FirstOrDefault(x => x.Id == orderId);

                if (order == null)
                {
                    result.Message = "Order not found";
                    result.IsSuccess = false;

                    return result;
                }

                if (order.OrderStatusId == 3)
                {
                    result.Message = "This order is already cancelled";
                    result.IsSuccess = true;

                    return result;
                }

                order.OrderStatusId = 3;

                _db.SaveChanges();

                result.Message = "You order has been cancelled";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to cancel your order";
                result.IsSuccess = false;
            }

            return result;
        }

        [HttpPut("editOrderContact")]
        public Result EditOrderContactInfo([FromBody] OrderContactDataInput orderContactDataInput)
        {
            var result = new Result();

            try
            {
                if (string.IsNullOrEmpty(orderContactDataInput.ContactPerson) || string.IsNullOrEmpty(orderContactDataInput.ContactNumber) || string.IsNullOrEmpty(orderContactDataInput.ContactAddress))
                {
                    result.Message = "Please enter the required fields";
                    result.IsSuccess = false;

                    return result;
                }

                var order = _db.ShopOrders.FirstOrDefault(x => x.Id == orderContactDataInput.OrderId);

                if (order == null)
                {
                    result.Message = "Order not found";
                    result.IsSuccess = false;

                    return result;
                }

                order.ContactPerson = orderContactDataInput.ContactPerson;
                order.ContactNumber = orderContactDataInput.ContactNumber;
                order.ContactAddress = orderContactDataInput.ContactAddress;

                _db.SaveChanges();

                result.Message = "You contact information for this order is successfully updated";
                result.IsSuccess = true;
            }
            catch
            {
                result.Message = "Unable to update the contact information of your current order";
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
