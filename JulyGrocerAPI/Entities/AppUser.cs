using System;
using System.Collections.Generic;

namespace JulyGrocerAPI.Entities;

public partial class AppUser
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int UserTypeId { get; set; }

    public string EditableContactPerson { get; set; } = null!;

    public string EditableContactNumber { get; set; } = null!;

    public string EditableContactAddress { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual ICollection<ShopOrder> ShopOrders { get; set; } = new List<ShopOrder>();

    public virtual ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();

    public virtual UserType UserType { get; set; } = null!;
}
