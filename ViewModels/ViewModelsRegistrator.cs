using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.ViewModels
{
	public static class ViewModelsRegistrator
	{
		public static void RegisterViewModels(this IServiceCollection services)
		{
			services.AddSingleton<MainWindowViewModel>();
			return;
		}
	}
}
