﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TemplateBuilder.Helpers;

namespace TemplateBuilder.ViewModel.Converters
{
    public class MultiplyConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double factor = ParseValue(parameter);
            double val = (double)value;

            return val * factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double factor = ParseValue(parameter);
            double val = (double)value;

            return val / factor;
        }

        private double ParseValue(object value)
        {
            IntegrityCheck.IsNotNull(value);
            double val;
            bool isValSuccessful = double.TryParse((string)value, out val);
            IntegrityCheck.IsTrue(isValSuccessful);
            return val;
        }
    }
}
