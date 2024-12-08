using System;
using System.Collections.Generic;

namespace SariKartAPI.Entities;

public partial class StoreBranch
{
    public int Id { get; set; }

    public string Branch { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Province { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
