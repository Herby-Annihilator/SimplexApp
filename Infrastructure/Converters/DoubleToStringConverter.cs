using SimplexApp.Infrastructure.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimplexApp.Infrastructure.Converters
{
	public class DoubleToStringConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is double)) return "0";
			return ((double)value).ToString();
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is string)) return 0;
			string str = (string)value;
			if (string.IsNullOrWhiteSpace(str)) return 0;
			if (!double.TryParse(str, out double number))
				return 0;
			else
				return number;
		}
	}
}
