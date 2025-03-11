using HomestayWeb.Validations;
using System.ComponentModel.DataAnnotations;

namespace HomestayWeb.Dtos
{
    public class CartItem
    {
        [Required(ErrorMessage = "Password is required")]
        public int HomeStayId { get; set; }

        [DateNotLessThanCurrent]
        [DateComparison]
        [Required(ErrorMessage = "Start date is required")]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime DateEnd { get; set; }
    }
}
