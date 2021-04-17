using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimplexApp.Model.Data
{
	public class Equation : INotifyPropertyChanged
	{
		public int ID { get; set; }
		public string Coefficients { get; set; }
		public double RightPart { get; set; }
		public List<Sign> Signs { get; private set; }
		public Sign SelectedSign { get; set; }

		public Equation(int id)
		{
			Signs = new List<Sign>()
			{
				new EqualSign(),
				new MoreThanOrEqual(),
				new MoreThanSign(),
				new LessThanOrEqual(),
				new LessThanSign()
			};
			SelectedSign = new EqualSign();
			ID = id;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public abstract class Sign
	{
	}

	public class EqualSign : Sign
	{
		public override string ToString() => "=";
	}

	public class MoreThanSign : Sign
	{
		public override string ToString() => ">";
	}

	public class LessThanSign : Sign
	{
		public override string ToString() => "<";
	}

	public class MoreThanOrEqual : Sign
	{
		public override string ToString() => ">=";
	}

	public class LessThanOrEqual : Sign
	{
		public override string ToString() => "<=";
	}
}
