using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TemplateBuilderMVVM.ViewModel.Converters
{
    public class OffsetConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double offset = double.Parse((string)parameter);
            double val = (double)value;
            return val + offset;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double offset = double.Parse((string)parameter);
            double val = (double)value;
            return val - offset;
        }
    }
}
