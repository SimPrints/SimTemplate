using SimTemplate.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SimTemplate.Views.Converters
{
    public class BooleanToBorderColour : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Transparent : Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isValid = false;
            Brush borderBrush = value as Brush;
            if (borderBrush != null)
            {
                if (borderBrush == Brushes.Transparent)
                {
                    isValid = true;
                }
                else if (borderBrush == Brushes.Red)
                {
                    isValid = false;
                }
                else
                {
                    throw IntegrityCheck.Fail(
                        "BorderBrush unexpected value {0}",
                        borderBrush.ToString());
                }
            }
            else
            {
                throw IntegrityCheck.Fail("BorderBrush argument does not cast to a Brush");
            }
            return isValid;
        }
    }
}
