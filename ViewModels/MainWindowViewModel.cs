using SimplexApp.Infrastructure.Commands;
using SimplexApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Markup;
using SimplexApp.Model.Data;
using SimplexApp.Model.Services;
using System.Collections.ObjectModel;
using SimplexApp.Model.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MyLibrary.Algorithms.Methods.Simplex;
using MyLibrary.Extensions.ArrayExtensions;
using MyLibrary.Algorithms.Methods.Simplex.SimplexData;
using System.Linq;
using System.IO;
using SimplexApp.Model.Extensions;

namespace SimplexApp.ViewModels
{
	[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
	public class MainWindowViewModel : ViewModel
	{
		private ServiceLocator _serviceLocator;
		public MainWindowViewModel()
		{
			_serviceLocator = new ServiceLocator();
			BasisVariables = new ObservableCollection<Variable>();
			StartVariables = new ObservableCollection<Variable>();
			Equations = new ObservableCollection<Equation>();
			CommonVariables = new ObservableCollection<CommonVariableValue>();

			AddNewEquationCommand = new LambdaCommand(OnAddNewEquationCommandExecuted, CanAddNewEquationCommandExecute);
			DeleteSelectedEquationCommand = new LambdaCommand(OnDeleteSelectedEquationCommandExecuted, CanDeleteSelectedEquationCommandExecute);
			SolveTaskCommand = new LambdaCommand(OnSolveTaskCommandExecuted, CanSolveTaskCommandExecute);

			ExampleFromFileCommand = new LambdaCommand(OnExampleFromFileCommandExecuted, CanExampleFromFileCommandExecute);
		}
		#region Properties
		private string _title = "Title";
		public string Title { get => _title; set => Set(ref _title, value); }

		public ObservableCollection<Variable> BasisVariables { get; set; }
		public ObservableCollection<Variable> StartVariables { get; set; }
		public ObservableCollection<Equation> Equations { get; set; }
		public ObservableCollection<CommonVariableValue> CommonVariables { get; set; }

		private Equation _selectedEquation;
		public Equation SelectedEquation { get => _selectedEquation; set => Set(ref _selectedEquation, value); }

		private IOptimalityCriterion _selectedOptimalityCtriterion;
		public IOptimalityCriterion SelectedOptimalityCriterion { get => _selectedOptimalityCtriterion; set => Set(ref _selectedOptimalityCtriterion, value); }

		public ObservableCollection<IOptimalityCriterion> OptimalityCriterions { get; set; } = new ObservableCollection<IOptimalityCriterion>()
		{
			new MaxOptimalityCriterion(),
			new MinOptimalityCriterion()
		};

		private string _targetFunctionCoefficients;
		public string TargetFunctionCoefficients { get => _targetFunctionCoefficients; set => Set(ref _targetFunctionCoefficients, value); }

		private double _targetFunctionOptimalValue;
		public double TargetFunctionOptimalValue { get => _targetFunctionOptimalValue; set => Set(ref _targetFunctionOptimalValue, value); }

		private string _status = "Решаю все";
		public string Status { get => _status; set => Set(ref _status, value); }

		private double heightOfTheRowWithAlternativeSolutions;
		public double HeightOfTheRowWithAlternativeSolutions { get => heightOfTheRowWithAlternativeSolutions; set => Set(ref heightOfTheRowWithAlternativeSolutions, value); }

		private string _targetFunctionFreeCoef;
		public string TargetFunctionFreeCoef { get => _targetFunctionFreeCoef; set => Set(ref _targetFunctionFreeCoef, value); }

		private string _fileName = "example.txt";
		public string FileName { get => _fileName; set => Set(ref _fileName, value); }
		#endregion

		#region Commands
		public ICommand AddNewEquationCommand { get; }
		private void OnAddNewEquationCommandExecuted(object p)
		{
			Equations.Add(new Equation(Equations.Count + 1));
		}
		private bool CanAddNewEquationCommandExecute(object p) => true;

		public ICommand DeleteSelectedEquationCommand { get; }
		private void OnDeleteSelectedEquationCommandExecuted(object p)
		{
			Equations.Remove(SelectedEquation);
			for (int i = 0; i < Equations.Count; i++)
			{
				Equations[i].ID = i + 1;
				Equations[i].OnPropertyChanged("ID");
			}
		}
		private bool CanDeleteSelectedEquationCommandExecute(object p) => !(SelectedEquation is null);

		public ICommand SolveTaskCommand { get; }
		private void OnSolveTaskCommandExecuted(object p)
		{
			try
			{
				StartVariables.Clear();
				BasisVariables.Clear();
				CommonVariables.Clear();
				TargetFunctionOptimalValue = 0;
				SimplexTable table = PrepareFirstSimplexTable();
				
				SimplexMethod simplexMethod = new SimplexMethod(table, SelectedOptimalityCriterion);
				SimplexAnswer answer = simplexMethod.Solve();
				Status = answer.Status.ToString();				
				HeightOfTheRowWithAlternativeSolutions = 0;
				if (answer.Status == AnswerStatus.TargetFunctionUnlimited)
					Status = "Функция не ограничена";
				else if (answer.Status == AnswerStatus.OneSolution || answer.Status == AnswerStatus.SeveralSolutions)
				{
					if (answer.Status == AnswerStatus.SeveralSolutions)
					{
						Status = "Найдены альтернативные оптимумы";
						HeightOfTheRowWithAlternativeSolutions = 200;
						foreach (var commonVariable in answer.CommonVariableValues)
						{
							CommonVariables.Add(commonVariable);
						}
					}
					for (int i = 0; i < answer.Solutions[0].OptimalCoefficients.Length; i++)
					{
						if (answer.Solutions[0].BasisIndexes.Contains(i))
						{
							BasisVariables.Add(new Variable($"X{i + 1}", Math.Round(answer.Solutions[0].OptimalCoefficients[i], 5)));
						}
						if (answer.Solutions[0].StartIndexes.Contains(i))
							StartVariables.Add(new Variable($"X{i + 1}", Math.Round(answer.Solutions[0].OptimalCoefficients[i], 5)));
					}
					TargetFunctionOptimalValue = Math.Round(answer.Solutions[0].OptimalValue, 5);
				}
				else
				{
					Status = "Решений нет";
				}
			}
			catch (Exception e)
			{
				Status = "Не удалось выполнить команду. Причина - " + e.Message;
			}		
		}
		private bool CanSolveTaskCommandExecute(object p)
		{
			if (Equations.Count == 0)
				return false;
			foreach (var equation in Equations)
			{
				if (equation.SelectedSign == null || string.IsNullOrWhiteSpace(equation.Coefficients))
					return false;
			}
			if (string.IsNullOrWhiteSpace(TargetFunctionCoefficients) || SelectedOptimalityCriterion == null)
				return false;
			if (!double.TryParse(TargetFunctionFreeCoef, out double number))
				return false;
			return true;
		}


		public ICommand ExampleFromFileCommand { get; }
		private void OnExampleFromFileCommandExecuted(object p)
		{
			try
			{
				ExampleFromFileService service = _serviceLocator.ExampleFromFileService;
				Example example = service.GetExample(FileName);
				Equations.Clear();
				SelectedEquation = null;
				SelectedOptimalityCriterion = example.TargetFunction.OptimalityCriterion;
				TargetFunctionCoefficients = example.TargetFunction.Coefficients.GetEqualString();
				TargetFunctionFreeCoef = example.TargetFunction.FreeCoefficient.ToString();
				for (int i = 0; i < example.Equations.Count; i++)
				{
					Equations.Add(example.Equations[i]);
					Equations[i].OnPropertyChanged("SelectedSign");
				}
			}
			catch(Exception e)
			{
				Status = $"Операция не выполнена. Причина {e.Message}";
			}
		}
		private bool CanExampleFromFileCommandExecute(object p) => File.Exists(FileName);
		#endregion

		private SimplexTable PrepareFirstSimplexTable()
		{
			var parser = App.Host.Services.GetRequiredService<StringToDoubleListParser>();
			List<Inequality> inequalities = new List<Inequality>();
			Inequality inequality;
			MyLibrary.Algorithms.Methods.Simplex.SimplexData.Sign currentSign;
			for (int i = 0; i < Equations.Count; i++)
			{
				if (Equations[i].SelectedSign.ToString() == "≤")
				{
					currentSign = MyLibrary.Algorithms.Methods.Simplex.SimplexData.Sign.LessThanOrEqualSign;
				}
				else if (Equations[i].SelectedSign.ToString() == "≥")
				{
					currentSign = MyLibrary.Algorithms.Methods.Simplex.SimplexData.Sign.MoreThanOrEqualSign;
				}
				else
				{
					currentSign = MyLibrary.Algorithms.Methods.Simplex.SimplexData.Sign.EqualSign;
				}
				inequality = new Inequality(parser.Parse(Equations[i].Coefficients), Equations[i].RightPart, currentSign);
				inequalities.Add(inequality);
			}
			TargetFunction targetFunction = new TargetFunction(parser.Parse(TargetFunctionCoefficients), SelectedOptimalityCriterion, Convert.ToDouble(TargetFunctionFreeCoef));
			return SimplexMethod.PrepareFirstSimplexTable(inequalities, targetFunction);
		}
		
	}
}
