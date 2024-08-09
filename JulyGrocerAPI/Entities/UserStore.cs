using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class UserStore
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Address { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual AppUser User { get; set; } = null!;
}
