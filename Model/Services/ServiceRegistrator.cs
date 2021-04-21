using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.Model.Services
{
	public static class ServiceRegistrator
	{
		public static void RegisterServices(this IServiceCollection services)
		{
			services.AddTransient<StringToDoubleArrayParser>();
			services.AddTransient<StringToDoubleListParser>();
			return;
		}
	}
}
