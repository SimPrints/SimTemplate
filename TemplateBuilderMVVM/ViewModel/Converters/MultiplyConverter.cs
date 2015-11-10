using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TemplateBuilderMVVM.Model;

namespace TemplateBuilderMVVM.ViewModel.Converters
{
    public class MultiplyConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] paramText = ((string)parameter).Split(';'); // TODO: handle formatting errors
            double factor = double.Parse(paramText[0]); // TODO: handle parse errors
            double offset = double.Parse(paramText[1]); // TODO: handle parse errors
            double val = (double)value;
            return val * factor + offset;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
