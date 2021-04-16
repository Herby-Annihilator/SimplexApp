using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.Model.Data
{
	public class Equation
	{
		public int ID { get; set; }
		public string Coefficients { get; set; }
		public double RightPart { get; set; }
		public Sign Sign { get; set; }
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
