using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;

namespace TemplateBuilder.ViewModel.Converters
{
    public class TypeToImageConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MinutiaType type = (MinutiaType)value;

            Uri imageUri = null;
            switch (type)
            {
                case MinutiaType.Termination:
                    imageUri = new Uri("Resources/termination.png", UriKind.Relative);
                    break;

                case MinutiaType.Bifurication:
                    imageUri = new Uri("Resources/bifurication.png", UriKind.Relative);
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(type);
            }
            IntegrityCheck.IsNotNull(imageUri);
            BitmapImage image = new BitmapImage(imageUri);
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
