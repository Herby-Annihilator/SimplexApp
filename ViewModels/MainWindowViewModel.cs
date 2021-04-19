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
using System.Linq;

namespace SimplexApp.ViewModels
{
	[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
	public class MainWindowViewModel : ViewModel
	{
		public MainWindowViewModel()
		{
			BasisVariables = new ObservableCollection<Variable>();
			FreeVariables = new ObservableCollection<Variable>();
			Equations = new ObservableCollection<Equation>();

			AddNewEquationCommand = new LambdaCommand(OnAddNewEquationCommandExecuted, CanAddNewEquationCommandExecute);
			DeleteSelectedEquationCommand = new LambdaCommand(OnDeleteSelectedEquationCommandExecuted, CanDeleteSelectedEquationCommandExecute);
			SolveTaskCommand = new LambdaCommand(OnSolveTaskCommandExecuted, CanSolveTaskCommandExecute);
		}
		#region Properties
		private string _title = "Title";
		public string Title { get => _title; set => Set(ref _title, value); }

		public ObservableCollection<Variable> BasisVariables { get; set; }
		public ObservableCollection<Variable> FreeVariables { get; set; }
		public ObservableCollection<Equation> Equations { get; set; }

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

		private string _status;
		public string Status { get => _status; set => Set(ref _status, value); }
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
			SimplexTable table = PrepareFirstSimplexTable();
			SimplexMethod simplexMethod = new SimplexMethod(table, SelectedOptimalityCriterion);
			SimplexAnswer answer = simplexMethod.Solve();
			Status = answer.Status.ToString();
			FreeVariables.Clear();
			BasisVariables.Clear();
			if (answer.Status != AnswerStatus.NoSolutions)
			{
				TargetFunctionOptimalValue = answer.Solutions[0].OptimalValue;
				for (int i = 0; i < answer.Solutions[0].BasisVariablesIndexes.Length; i++)
				{
					if (answer.Solutions[0].BasisVariablesIndexes.Contains(i))
						BasisVariables.Add(new Variable($"X{i}", answer.Solutions[0].OptimalCoefficients[i]));
					else
						FreeVariables.Add(new Variable($"X{i}", answer.Solutions[0].OptimalCoefficients[answer.Solutions[0].BasisVariablesIndexes[i]]));
				}
			}
		}
		private bool CanSolveTaskCommandExecute(object p)
		{
			foreach (var equation in Equations)
			{
				if (equation.SelectedSign == null || string.IsNullOrWhiteSpace(equation.Coefficients))
					return false;
			}
			if (string.IsNullOrWhiteSpace(TargetFunctionCoefficients) || SelectedOptimalityCriterion == null)
				return false;
			return true;
		}

		#endregion

		private SimplexTable PrepareFirstSimplexTable()
		{
			var parser = App.Host.Services.GetRequiredService<StringToDoubleArrayParser>();
			int size = Equations.Count + parser.Parse(Equations[0].Coefficients).Length;
			double[][] matrix = new double[size][];
			double[] matrixStr;
			double[] freeMemebers = new double[size];
			int[] basis = new int[Equations.Count];
			int basisVariableIndex = size - Equations.Count;
			for (int i = 0; i < Equations.Count; i++)
			{
				matrixStr = new double[size];
				parser.Parse(Equations[i].Coefficients).CopyTo(matrixStr, 0);
				if (Equations[i].SelectedSign.ToString() == "<=")
					matrixStr[basisVariableIndex] = 1;
				else
					matrixStr[basisVariableIndex] = -1;
				basis[i] = basisVariableIndex;
				basisVariableIndex += 1;
				matrix[i] = matrixStr;
				freeMemebers[i] = Equations[i].RightPart;
			}
			for (int i = Equations.Count; i < size; i++)
			{
				matrix[i] = new double[size];
			}
			matrixStr = parser.Parse(TargetFunctionCoefficients).Map((element) => (-1) * element);
			SimplexTable table = new SimplexTable(matrix, freeMemebers, basis, matrixStr);
			return table;
		}
		
	}
}
