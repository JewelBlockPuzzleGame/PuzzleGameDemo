using System;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
	public class Proxy : Notifier, IProxy, INotifier
	{
		public static string NAME = "Proxy";

		public Proxy() : this(Proxy.NAME, null)
		{
		}

		public Proxy(string proxyName) : this(proxyName, null)
		{
		}

		public Proxy(string proxyName, object data)
		{
			this.ProxyName = (proxyName ?? Proxy.NAME);
			if (data != null)
			{
				this.Data = data;
			}
		}

		public virtual void OnRegister()
		{
		}

		public virtual void OnRemove()
		{
		}

		public string ProxyName { get; protected set; }

		public object Data { get; set; }

		public Action Event_Para0 { get; set; }

		public Action<object> Event_Para1 { get; set; }
	}
}
