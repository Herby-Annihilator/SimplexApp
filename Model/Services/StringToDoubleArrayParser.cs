using SimplexApp.Model.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.Model.Services
{
	public class StringToDoubleArrayParser : IParser<string, double[]>
	{
		public double[] Parse(string input)
		{
			double[] numbers = null;
			string[] numbersStr = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (numbersStr != null)
			{
				for (int i = 0; i < numbersStr.Length; i++)
				{
					try
					{
						numbers[i] = Convert.ToDouble(numbersStr[i]);
					}
					catch(Exception)
					{
						numbers[i] = default;
					}
				}
			}
			return numbers;
		}
	}
}
