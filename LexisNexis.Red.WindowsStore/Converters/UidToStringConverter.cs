using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LexisNexis.Red.WindowsStore.Converters
{
    public class UidToStringConverter : IValueConverter
    {

        private static readonly ResourceLoader RESOURCE_LOADER=new ResourceLoader();


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value!=null&& !string.IsNullOrEmpty(value.ToString()))
            {
                return RESOURCE_LOADER.GetString(value.ToString());
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
