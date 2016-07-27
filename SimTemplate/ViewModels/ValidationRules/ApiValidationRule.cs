using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SimTemplate.ViewModels.ValidationRules
{
    public class ApiValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string apiKey = value as string;

            bool isValid;
            if (apiKey != null)
            {
                Guid result;
                isValid = Guid.TryParse((string)value, out result);
            }
            else
            {
                isValid = false;
            }

            // Number is valid
            return new ValidationResult(isValid, null);
        }
    }
}
