using SimTemplate.DataTypes.Enums;
using SimTemplate.Utilities;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SimTemplate.Views.Converters
{
    public class ActivityToLoadIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Activity currentActivity = (Activity)value;
            ResourceDictionary iconLookup = (ResourceDictionary)parameter;

            ImageSource image;
            switch (currentActivity)
            {
                case Activity.Templating:
                case Activity.Idle:
                case Activity.Uninitialised:
                case Activity.Transitioning:
                case Activity.Fault:
                    image = (ImageSource)iconLookup["loadIcon"];
                    break;

                case Activity.Loading:
                    image = (ImageSource)iconLookup["cancelIcon"];
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
