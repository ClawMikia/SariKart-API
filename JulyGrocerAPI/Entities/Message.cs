using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class Message
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RiderId { get; set; }

    public string Message1 { get; set; } = null!;

    public bool IsSentByUser { get; set; }

    public bool IsSentByRider { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public virtual Rider Rider { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
