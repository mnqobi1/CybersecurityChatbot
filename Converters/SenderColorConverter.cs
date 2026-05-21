using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CybersecurityChatbot.Converters 
{
    public class SenderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sender = value as string;
            return sender == "User" ? Brushes.LightGreen : Brushes.Turquoise;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}