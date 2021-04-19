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

		private string _targetFunctionCoefficients;
		public string TargetFunctionCoefficients { get => _targetFunctionCoefficients; set => Set(ref _targetFunctionCoefficients, value); }

		private double _targetFunctionOptimalValue;
		public double TargetFunctionOptimalValue { get => _targetFunctionOptimalValue; set => Set(ref _targetFunctionOptimalValue, value); }

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
			var stringToDoubleArrayParser = App.Host.Services.GetRequiredService<StringToDoubleArrayParser>();
			
		}
		private bool CanSolveTaskCommandExecute(object p)
		{
			foreach (var equation in Equations)
			{
				if (equation.SelectedSign == null || string.IsNullOrWhiteSpace(equation.Coefficients))
					return false;
			}
			return true;
		}
		#endregion
	}
}
