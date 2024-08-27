using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Product1 { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public byte[] Picture { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public int Stock { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
