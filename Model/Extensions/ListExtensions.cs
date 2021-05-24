using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.Model.Extensions
{
	public static class ListExtensions
	{
		public static string GetEqualString<T>(this List<T> lst)
		{
			string result = "";
			foreach (T item in lst)
			{
				result += item.ToString() + " ";
			}
			return result;
		}
	}
}
