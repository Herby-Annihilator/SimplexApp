using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexApp.Model.Services
{
	public class ServiceLocator
	{
		public ExampleFromFileService ExampleFromFileService => App.Host.Services.GetRequiredService<ExampleFromFileService>();
	}
}
