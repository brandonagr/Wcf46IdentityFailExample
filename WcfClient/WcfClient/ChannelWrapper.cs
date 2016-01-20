using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfClient
{
	public class ChannelWrapper<T> : IDisposable
			where T : class, IChannel
	{
		#region Declarations

		readonly T _channel;

		#endregion

		#region Properties

		public T Client
		{
			get { return _channel; }
		}

		#endregion

		#region Initialization

		public ChannelWrapper(T channel)
		{
			_channel = channel;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (_channel != null)
			{
				bool closedChannel = false;
				try
				{
					if (_channel.State != CommunicationState.Faulted)
					{
						_channel.Close();
						closedChannel = true;
					}
				}
				finally
				{
					if (!closedChannel)
						_channel.Abort();
				}
			}
		}

		#endregion
	}
}
