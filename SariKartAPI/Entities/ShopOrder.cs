using System;
using System.Collections.Generic;

namespace SariKartAPI.Entities;

public partial class ShopOrder
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreateDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Change { get; set; }

    public int OrderStatusId { get; set; }

    public string ContactPerson { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string ContactAddress { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
