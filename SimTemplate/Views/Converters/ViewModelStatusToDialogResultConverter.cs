using SimTemplate.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimTemplate.Views.Converters
{
    public class ViewModelStatusToDialogResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? dialogResult = false;
            if (value.Equals(ViewModelStatus.Running))
            {
                dialogResult = null;
            }
            else if (value.Equals(ViewModelStatus.Complete))
            {
                dialogResult = true;
            }
            return dialogResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? dialogResult = value as bool?;

            ViewModelStatus status = ViewModelStatus.None;
            if (!dialogResult.HasValue)
            {
                status = ViewModelStatus.Running;
            }
            else if (!dialogResult.Value)
            {
                status = ViewModelStatus.NoChange;
            }
            return (status != ViewModelStatus.None) ? status : Binding.DoNothing;
        }
    }
}
