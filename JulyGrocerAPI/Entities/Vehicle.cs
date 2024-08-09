using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class Vehicle
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Rider> Riders { get; set; } = new List<Rider>();
}
