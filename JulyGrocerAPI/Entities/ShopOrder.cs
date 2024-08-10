using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class ShopOrder
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreateDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Change { get; set; }

    public bool Paid { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual AppUser User { get; set; } = null!;
}
