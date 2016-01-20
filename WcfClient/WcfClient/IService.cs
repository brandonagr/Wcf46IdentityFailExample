using System.ServiceModel;

namespace WcfSelfHostedServer
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
		string GetSomething();
	}
}
