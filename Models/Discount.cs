using HomestayWeb.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomestayWeb.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public int HomstayId { get; set; }

    [Display(Name = "Discount")]
    [DisplayFormat(DataFormatString = "{0:F2}")]
    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
    public double Discount1 { get; set; }

    [Display(Name = "Start Date")]
    [DateNotLessThanCurrent]
    [DateComparison]
    public DateTime DateStart { get; set; }

    [Display(Name = "End Date")]
    public DateTime DateEnd { get; set; }

    public virtual Homestay? Homstay { get; set; } = null!;
}
