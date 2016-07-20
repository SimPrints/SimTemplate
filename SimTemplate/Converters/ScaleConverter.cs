using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimTemplate.Converters
{
    public class ScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double offset = 0;
            if (parameter != null)
            {
                double parameterVal;
                bool isSuccessful = double.TryParse((string)parameter, out parameterVal);
                if (isSuccessful)
                {
                    offset = parameterVal;
                }
            }
            double val = (double)values[0];
            double scale = (double)values[1];
            return val * scale + offset;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
