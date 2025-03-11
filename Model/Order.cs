using System;
using System.Collections.Generic;

namespace HomestayWeb.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int HomestayId { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Homestay Homestay { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
