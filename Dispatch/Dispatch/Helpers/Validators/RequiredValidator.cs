using System.Globalization;
using System.Windows.Controls;

namespace Dispatch.Helpers.Validators
{
    public class RequiredValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value as string))
            {
                return new ValidationResult(false, "Field is required");
            }

            return ValidationResult.ValidResult;
        }
    }
}
