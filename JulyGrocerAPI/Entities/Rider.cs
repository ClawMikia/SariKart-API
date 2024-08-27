using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class Rider
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int VehicleId { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual Vehicle Vehicle { get; set; } = null!;
}
