using SimplexApp.Infrastructure.Converters.Base;
using SimplexApp.Model.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimplexApp.Infrastructure.Converters
{
	public class SignConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Sign)) return "-*-";
			return (value as Sign).ToString();
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value as string) switch
			{
				">=" => new MoreThanOrEqual(),
				">" => new MoreThanSign(),
				"<" => new LessThanSign(),
				"<=" => new LessThanOrEqual(),
				"=" => new EqualSign(),
				_ => new EqualSign()
			};
		}
	}
}
