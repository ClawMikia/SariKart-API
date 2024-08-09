using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class Delivery
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public int OrderId { get; set; }

    public int RiderId { get; set; }

    public DateTime DeliveryDate { get; set; }

    public bool Delivered { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Rider Rider { get; set; } = null!;

    public virtual UserStore Store { get; set; } = null!;
}
