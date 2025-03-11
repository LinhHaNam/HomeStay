using System;
using System.Collections.Generic;

namespace HomestayWeb.Model;

public partial class Discount
{
    public int DiscountId { get; set; }

    public int HomstayId { get; set; }

    public double Discount1 { get; set; }

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public virtual Homestay Homstay { get; set; } = null!;
}
