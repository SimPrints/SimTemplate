using SimTemplate.DataTypes.Enums;
using SimTemplate.Utilities;
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
            ViewModelStatus status = (ViewModelStatus)value;

            bool? dialogResult = false;
            switch (status)
            {
                case ViewModelStatus.NoChange:
                    dialogResult = false;
                    break;

                case ViewModelStatus.Running:
                    dialogResult = null;
                    break;

                case ViewModelStatus.Complete:
                    dialogResult = true;
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(status);
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
