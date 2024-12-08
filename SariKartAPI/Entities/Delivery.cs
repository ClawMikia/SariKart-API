using System;
using System.Collections.Generic;

namespace SariKartAPI.Entities;

public partial class Delivery
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public int OrderId { get; set; }

    public int RiderId { get; set; }

    public DateTime DeliveryDate { get; set; }

    public bool Delivered { get; set; }

    public int VehicleId { get; set; }

    public bool CashOnHand { get; set; }

    public virtual ShopOrder Order { get; set; } = null!;

    public virtual AppUser Rider { get; set; } = null!;

    public virtual StoreBranch Store { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
