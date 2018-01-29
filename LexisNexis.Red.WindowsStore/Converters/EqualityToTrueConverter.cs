using System;
using Windows.UI.Xaml.Data;

namespace LexisNexis.Red.WindowsStore.Converters
{
    public class EqualityToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool) value) ? parameter : null;
        }
    }
}
