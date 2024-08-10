using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class OrderLine
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual ShopOrder Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
