using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class StoreBranch
{
    public int Id { get; set; }

    public string Branch { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Province { get; set; } = null!;
}
