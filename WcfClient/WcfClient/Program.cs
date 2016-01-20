using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using WcfSelfHostedServer;

namespace WcfClient
{
	class Program
	{
		static void Main(string[] args)
		{
			// Run server (after installing the cert)
			// Then run this client, note it is targetting 4.6.1 in project properties
			// It will fail with the following exception: "The Identity check failed for the outgoing message. The remote endpoint did not provide a domain name system (DNS) claim and therefore did not satisfied DNS identity 'IdentityFail.local'. This may be caused by lack of DNS or CN name in the remote endpoint X.509 certificate's distinguished name."
			//
			// Now switch this project to target framework 4.5, it runs as expected

			using (var wcf = GetClient("localhost:30000"))
			{
				Console.WriteLine(wcf.Client.GetSomething());
			}

			Console.WriteLine("Success!");
			Console.ReadLine();
		}

		static ChannelWrapper<IServiceChannel> GetClient(string host)
		{
			var url = $"net.tcp://{host}/Service.svc";

			var channelFactory = ChannelFactory<IServiceChannel>(url);

			return new ChannelWrapper<IServiceChannel>(channelFactory.CreateChannel());
		}

		interface IServiceChannel : IService, IChannel
		{ }

		private static ChannelFactory<T> ChannelFactory<T>(string url)
		{
			int maxReceivedSizeBytes = 100 * 1024 * 1024;

			var binding = new CustomBinding(new BindingElement[] {
						new TransactionFlowBindingElement(),
						new SslStreamSecurityBindingElement(),
						new BinaryMessageEncodingBindingElement() {
							ReaderQuotas = { MaxDepth = maxReceivedSizeBytes, MaxStringContentLength = maxReceivedSizeBytes, MaxArrayLength = maxReceivedSizeBytes, MaxBytesPerRead = maxReceivedSizeBytes, MaxNameTableCharCount = maxReceivedSizeBytes },
						},
						new TcpTransportBindingElement {
							TransferMode = TransferMode.StreamedResponse,
							MaxReceivedMessageSize = maxReceivedSizeBytes,
						},
					}) {
				SendTimeout = TimeSpan.FromSeconds(30),
			};

			var channelFactory = new ChannelFactory<T>(binding, new EndpointAddress(new Uri(url), EndpointIdentity.CreateDnsIdentity("IdentityFail.local")));

			// ignore root CA validation for test cert
			foreach (var behavior in channelFactory.Endpoint.EndpointBehaviors)
			{
				var cc = behavior as ClientCredentials;
				if (cc != null)
				{
					cc.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
					cc.ServiceCertificate.Authentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
				}
			}

			return channelFactory;
		}
	}
}
