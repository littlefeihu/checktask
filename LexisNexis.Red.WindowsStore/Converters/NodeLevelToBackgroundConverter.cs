using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace LexisNexis.Red.WindowsStore.Converters
{
    public class NodeLevelToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int nodeLevel = (int)value;
            int rgb = 160 - nodeLevel * 20;
            if (rgb < 0)
                rgb = 0;
            return new SolidColorBrush(Color.FromArgb(255, 255, (byte)rgb, (byte)rgb));
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
