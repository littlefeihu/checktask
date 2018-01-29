using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LexisNexis.Red.WindowsStore.Converters
{
    public class BoolToIndexStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null && value is bool && (bool)value ?(Style) Application.Current.Resources["IndexMenuHeaderTextBlockStyle"] :(Style) Application.Current.Resources["IndexMenuItemTextBlockStyle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
