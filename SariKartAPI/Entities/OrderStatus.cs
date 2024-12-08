using System;
using System.Collections.Generic;

namespace SariKartAPI.Entities;

public partial class OrderStatus
{
    public int Id { get; set; }

    public string OrderStatus1 { get; set; } = null!;

    public virtual ICollection<ShopOrder> ShopOrders { get; set; } = new List<ShopOrder>();
}
