using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace D2MTranslator.Converters
{
    public class WidthMinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
            {
                double deduction = 0;
                if (parameter is string paramString && double.TryParse(paramString, out deduction)) { }
                //Debug.WriteLine("deduction!");
                return width - deduction;
            }
            //Debug.WriteLine("wtf");

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
