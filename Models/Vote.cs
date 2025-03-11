using System;
using System.Collections.Generic;

namespace HomestayWeb.Models;

public partial class Vote
{
    public int UserId { get; set; }

    public int HomestayId { get; set; }

    public int Rating { get; set; }

    public string? Review { get; set; }

    public virtual Homestay Homestay { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
