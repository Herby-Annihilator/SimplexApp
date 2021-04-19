using MyLibrary.Algorithms.Methods.Simplex;
using SimplexApp.Infrastructure.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace SimplexApp.Infrastructure.Converters
{
	public class OptimalityCriterionToComboBoxItemConverter : Converter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is IOptimalityCriterion))
				return new ComboBoxItem()
				{
					Content = "None"
				};
			return new ComboBoxItem()
			{
				Content = ((IOptimalityCriterion)value).ToString()
			};
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is ComboBoxItem)) return new MaxOptimalityCriterion();
			if ((string)((ComboBoxItem)value).Content == "Max") return new MaxOptimalityCriterion();
			else
				return new MinOptimalityCriterion();
		}
	}
}
