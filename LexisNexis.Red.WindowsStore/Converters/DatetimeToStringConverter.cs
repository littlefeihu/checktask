using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LexisNexis.Red.WindowsStore.Converters
{
    public class DatetimeToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;
            string format = (string)parameter;
            ResourceLoader rl = new ResourceLoader();
            string dateFormat = rl.GetString(format);            
            return dt.ToString(dateFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
