using SimplexApp.Model.Data;
using MyLibrary.Algorithms.Methods.Simplex.SimplexData;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MyLibrary.Algorithms.Methods.Simplex;

namespace SimplexApp.Model.Services
{
	public class ExampleFromFileService
	{
		protected const string INEQUALITIES_TAG = "[Inequalities]";
		protected const string END_INEQUALITIES_TAG = "[End_Inequalities]";

		protected const string TARGET_FUNCTION_COEFS_TAG = "[TargetFunctionCoefs]";
		protected const string END_TARGET_FUNCTION_COEFS_TAG = "[End_TargetFunctionCoefs]";

		protected const string FREE_COEFFICIENT_TAG = "[FreeCoefficient]";
		protected const string END_FREE_COEFFICIENT_TAG = "[End_FreeCoefficient]";

		protected const string OPTIMALITY_CRITERION_TAG = "[OptimalityCriterion]";
		protected const string END_OPTIMALITY_CRITERION_TAG = "[End_OptimalityCriterion]";
		public Example GetExample(string fileName)
		{
			try
			{
				string buffer;
				string optCriterion;
				int inequalityID = 1;
				List<Equation> inequalities = new List<Equation>();
				List<double> targetFunctionCoefficients = null;
				IOptimalityCriterion optimalityCriterion = null;
				double freeCoef = 0;
				TargetFunction targetFunction;
				StreamReader reader = new StreamReader(fileName);
				while ((buffer = reader.ReadLine()) != null)
				{
					if (buffer == INEQUALITIES_TAG)
					{
						while ((buffer = reader.ReadLine()) != END_INEQUALITIES_TAG)
						{
							if (!string.IsNullOrWhiteSpace(buffer))
							{
								inequalities.Add(TranslateStringToEquation(buffer, inequalityID));
								inequalityID++;
							}
						}
					}
					if (buffer == TARGET_FUNCTION_COEFS_TAG)
					{
						while ((buffer = reader.ReadLine()) != END_TARGET_FUNCTION_COEFS_TAG)
						{
							if (!string.IsNullOrWhiteSpace(buffer))
							{
								targetFunctionCoefficients = TranslateStringToTargetFunctionCoefficients(buffer);
							}
						}
					}
					if (buffer == FREE_COEFFICIENT_TAG)
					{
						while ((buffer = reader.ReadLine()) != END_FREE_COEFFICIENT_TAG)
						{
							if (!string.IsNullOrWhiteSpace(buffer))
								freeCoef = Convert.ToDouble(buffer);
						}
					}
					if (buffer == OPTIMALITY_CRITERION_TAG)
					{
						while ((buffer = reader.ReadLine()) != END_OPTIMALITY_CRITERION_TAG)
						{
							if (!string.IsNullOrWhiteSpace(buffer))
							{
								optCriterion = buffer.ToLower();
								if (optCriterion == "max")
									optimalityCriterion = new MaxOptimalityCriterion();
								else if (optCriterion == "min")
									optimalityCriterion = new MinOptimalityCriterion();
							}
						}
					}
				}
				reader.Close();
				if (targetFunctionCoefficients == null || optimalityCriterion == null)
				{
					throw new Exception("Отсутствуют необходимые данные и критерии оптимальности или о коэф-ах целевой функции");
				}
				targetFunction = new TargetFunction(targetFunctionCoefficients, optimalityCriterion, freeCoef);
				return new Example(inequalities, targetFunction);
			}
			catch(Exception e)
			{
				throw new Exception($"Файл {fileName} не существует, либо в нем неверные данные. Конкретная причина - {e.Message}");
			}
		}
		protected Equation TranslateStringToEquation(string pattern, int desiredID)
		{
			Data.Sign sign = null;
			double rightPart;
			string[] arr = pattern.Split(new string[] { "<=", ">=", "=" }, StringSplitOptions.RemoveEmptyEntries);
			double.TryParse(arr[1], out rightPart);
			string[] instances = pattern.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < instances.Length; i++)
			{
				if (instances[i] == "<=")
				{
					sign = new LessThanOrEqual();
					break;
				}					
				else if (instances[i] == ">=")
				{
					sign = new MoreThanOrEqual();
					break;
				}					
				else if (instances[i] == "=")
				{
					sign = new EqualSign();
					break;
				}				
			}
			if (sign == null)
			{
				throw new Exception("Знак неравенства не был найден");
			}
			return new Equation(desiredID)
			{
				Coefficients = arr[0].Trim(),
				SelectedSign = sign,
				RightPart = rightPart
			};
		}

		protected List<double> TranslateStringToTargetFunctionCoefficients(string pattern)
		{
			List<double> coefs = new List<double>();
			string[] coefficients = pattern.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < coefficients.Length; i++)
			{
				coefs.Add(Convert.ToDouble(coefficients[i]));
			}
			return coefs;
		}
	}
}
