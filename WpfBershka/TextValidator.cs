using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfBershka
{
    public class TextValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int temp;
            if (value == null)
                return new ValidationResult(false, "Pole nie może być puste");
            else if (value.ToString().Length != 6)
                    return new ValidationResult(false, "Numer musi mieć 6 znaków");
            
            else if (!int.TryParse(value.ToString(), out temp))
            {
                return new ValidationResult(false, "Pole musi zawierać liczbę");
            }
            else if (temp < 100000)
            {
                return new ValidationResult(false, "Liczba musi być większa od 100000");
            }
            return ValidationResult.ValidResult;
        }
    }
}
