using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace LexisNexis.Red.WindowsStore.Converters
{
    public class IntEqualityToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? parameter : null;
        }
    }
}
