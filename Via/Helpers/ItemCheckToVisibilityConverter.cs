using System;
using Xamarin.Forms;

namespace Via.Helpers
{ 
        public class ItemCheckToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
            return (string)value == "Reports" ? SeparatorVisibility.Default : SeparatorVisibility.None;
            }

            public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new System.NotImplementedException();
            }
        }
    
}
