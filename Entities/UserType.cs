using System;
using System.Collections.Generic;

namespace SariKartAPI.Entities;

public partial class UserType
{
    public int Id { get; set; }

    public string UserType1 { get; set; } = null!;

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
}
