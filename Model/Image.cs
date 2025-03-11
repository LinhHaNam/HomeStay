using System;
using System.Collections.Generic;

namespace HomestayWeb.Model;

public partial class Image
{
    public int ImageId { get; set; }

    public int HomstayId { get; set; }

    public string Link { get; set; } = null!;

    public virtual Homestay Homstay { get; set; } = null!;
}
