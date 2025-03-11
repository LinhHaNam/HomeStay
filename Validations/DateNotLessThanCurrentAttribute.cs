using System.ComponentModel.DataAnnotations;

namespace HomestayWeb.Validations
{
    public class DateNotLessThanCurrentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateStartProperty = validationContext.ObjectType.GetProperty("DateStart");
            var dateEndProperty = validationContext.ObjectType.GetProperty("DateEnd");

            if (dateStartProperty != null && dateEndProperty != null)
            {
                var dateStartValue = (DateTime)dateStartProperty.GetValue(validationContext.ObjectInstance, null);
                var dateEndValue = (DateTime)dateEndProperty.GetValue(validationContext.ObjectInstance, null);
                var currentDate = DateTime.Now;

                if (dateStartValue < currentDate || dateEndValue < currentDate)
                {
                    return new ValidationResult("Dates cannot be less than the current date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
