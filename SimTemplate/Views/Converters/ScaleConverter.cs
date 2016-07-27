using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimTemplate.Views.Converters
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
            double trueHeight = (double)values[1];
            double scaledHeight = (double)values[2];
            return val * scaledHeight / trueHeight + offset;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        }
    }
}
