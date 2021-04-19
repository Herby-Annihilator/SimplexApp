﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.Model.Data
{
	public class Variable
	{
		public string Name { get; set; }
		public double Value { get; set; }
		public Variable(string name, double value)
		{
			Value = value;
			Name = name;
		}
	}
}
