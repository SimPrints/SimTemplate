using SimTemplate.DataTypes.Enums;
using SimTemplate.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SimTemplate.Views.Converters
{
    public class ActivityToStatusImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Activity currentActivity = (Activity)value;
            ResourceDictionary imageLookup = (ResourceDictionary)parameter;

            ImageSource image;
            switch (currentActivity)
            {
                case Activity.Fault:
                    image = (ImageSource)imageLookup["errorStatus"];
                    break;

                case Activity.Loading:
                case Activity.Transitioning:
                    image = (ImageSource)imageLookup["loadingStatus"];
                    break;

                case Activity.Templating:
                case Activity.Idle:
                    // TODO: uninitialised image?
                case Activity.Uninitialised:
                    image = null;
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(currentActivity);
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
