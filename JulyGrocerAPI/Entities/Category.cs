using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class Category
{
    public int Id { get; set; }

    public string Category1 { get; set; } = null!;

    public byte[] Icon { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
