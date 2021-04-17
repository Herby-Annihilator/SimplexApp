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
		#endregion
	}
}
