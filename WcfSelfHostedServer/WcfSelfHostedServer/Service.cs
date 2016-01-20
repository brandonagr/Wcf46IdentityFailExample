using System;
using System.ServiceModel;

namespace WcfSelfHostedServer
{
	/// <summary>
	/// Just a dummy service to do something
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class Service : IService
	{
		public string GetSomething()
		{
			var something = DateTime.Now.ToString("H:m:s.fff");
			Console.WriteLine("Got a call, " + something);
			return something;
		}
	}
}
