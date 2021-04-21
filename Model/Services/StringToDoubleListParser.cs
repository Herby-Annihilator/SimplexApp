using System;
using System.Collections.Generic;
using System.Text;
using SimplexApp.Model.Services.Interfaces;

namespace SimplexApp.Model.Services
{
	public class StringToDoubleListParser : IParser<string, List<double>>
	{
		public List<double> Parse(string input)
		{
			List<double> toReturn = new List<double>();
			string[] numbersStr = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (numbersStr != null)
			{
				for (int i = 0; i < numbersStr.Length; i++)
				{
					if (double.TryParse(numbersStr[i], out double number))
					{
						toReturn.Add(number);
					}
					else
					{
						toReturn.Add(default);
					}
				}
			}
			return toReturn;
		}
	}
}
