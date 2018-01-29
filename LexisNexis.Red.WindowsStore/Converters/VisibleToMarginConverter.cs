using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LexisNexis.Red.WindowsStore.Converters
{
    public class VisibleToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var bound = Window.Current.CoreWindow.Bounds;
            string param = parameter.ToString();
            if(param=="Content")
            {
                //bool visible = (bool)value;
                //if (bound.Height>bound.Width)
                //{
                    return new Thickness(20, 20, 20, 0);
                //}
                //else
                //{
                //    //return visible == false ? new Thickness(48, 20, 48, 0) : new Thickness(170, 20, 170, 0);
                //    return  new Thickness(48, 20, 48, 0);
                //}              
            }
            else if (param == "Annotation")
            {
                Visibility visible = (Visibility)value;
                if (bound.Height > bound.Width)
                {
                    return new Thickness(18, 0, 18, 0);
                }
                else
                {
                    return visible == Visibility.Visible ? new Thickness(18, 0, 18, 0) : new Thickness(170, 0, 170, 0);
                }   
              
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
