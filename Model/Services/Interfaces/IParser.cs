using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.Model.Services.Interfaces
{
	public interface IParser<TInput, TResult>
	{
		TResult Parse(TInput input);
	}
}
