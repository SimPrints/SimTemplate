using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace TemplateBuilderMVVM.ViewModel.Converters
{
    public abstract class BaseConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        abstract public object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        abstract public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
