using System;
using System.Collections.Generic;

namespace SariKartAPI.Entities;

public partial class OrderLine
{
    public int OrderLineId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual ShopOrder Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
