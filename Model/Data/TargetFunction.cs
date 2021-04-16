using System;
using System.Collections.Generic;
using System.Text;
using MyLibrary.Algorithms.Methods.Simplex;

namespace SimplexApp.Model.Data
{
	public class TargetFunction
	{
		public double[] Coefficients { get; set; } = new double[0];
		public IOptimalityCriterion OptimalityCriterion { get; set; } = new MaxOptimalityCriterion();
	}
}
