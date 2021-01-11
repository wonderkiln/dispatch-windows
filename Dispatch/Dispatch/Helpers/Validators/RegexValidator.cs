using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Dispatch.Helpers.Validators
{
    public class RegexValidator : ValidationRule
    {
        public string Pattern { get; set; } = ".+";

        public string Message { get; set; } = "Field does not conform to pattern";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                var regex = new Regex(Pattern);

                if (regex.IsMatch((string)value))
                {
                    return ValidationResult.ValidResult;
                }
            }

            return new ValidationResult(false, Message);
        }
    }
}
