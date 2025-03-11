using System.ComponentModel.DataAnnotations;

namespace HomestayWeb.Validations
{
    public class DateComparisonAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateStartProperty = validationContext.ObjectType.GetProperty("DateStart");
            var dateEndProperty = validationContext.ObjectType.GetProperty("DateEnd");

            if (dateStartProperty != null && dateEndProperty != null)
            {
                var dateStartValue = (DateTime)dateStartProperty.GetValue(validationContext.ObjectInstance, null);
                var dateEndValue = (DateTime)dateEndProperty.GetValue(validationContext.ObjectInstance, null);

                if (dateStartValue > dateEndValue)
                {
                    return new ValidationResult("DateStart cannot be later than DateEnd.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
