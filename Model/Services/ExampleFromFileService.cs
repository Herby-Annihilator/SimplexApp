using SimplexApp.Model.Data;
using MyLibrary.Algorithms.Methods.Simplex.SimplexData;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SimplexApp.Model.Services
{
	public class ExampleFromFileService
	{
		protected const string INEQUALITIES_TAG = "[Inequalities]";
		protected const string TARGET_FUNCTION_TAG = "[TargetFunction]";
		protected const string FREE_COEFFICIENT_TAG = "[FreeCoefficient]";
		protected const string OPTIMALITY_CRITERION_TAG = "[OptimalityCriterion]";
		public Example GetExample(string fileName)
		{
			try
			{
				string buffer;
				List<Equation> inequalities = new List<Equation>();
				TargetFunction targetFunction;
				StreamReader reader = new StreamReader(fileName);
				while ((buffer = reader.ReadLine()) != null)
				{
					if (buffer == INEQUALITIES_TAG)
					{

					}
				}
				reader.Close();
			}
			catch(Exception)
			{
				throw new Exception($"Файл {fileName} не существует, либо в нем неверные данные");
			}
		}
		protected Equation TranslateStringToEquation(string pattern, int desiredID)
		{
			Data.Sign sign;
			double rightPart;
			string[] arr = pattern.Split(new string[] { "<=", ">=", "=" }, StringSplitOptions.RemoveEmptyEntries);
			double.TryParse(arr[2], out rightPart);
			if (arr[1] == "<=")
				sign = new LessThanOrEqual();
			else if (arr[1] == ">=")
				sign = new MoreThanOrEqual();
			else if (arr[1] == "=")
				sign = new EqualSign();
			else
				throw new Exception($"Не верный формат данных в файле");
			return new Equation(desiredID)
			{
				Coefficients = arr[0].Trim(),
				SelectedSign = sign,
				RightPart = rightPart
			};
		}
	}
}
