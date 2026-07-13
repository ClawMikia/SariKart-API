using System;
using System.Collections.Generic;

namespace SariKartAPI.Entities;

public partial class UserStore
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Address { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
