using System;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
	public class Notifier : INotifier
	{
		protected const string MULTITON_MSG = "Multiton key for this Notifier not yet initialized!";

		public virtual void SendNotification(string notificationName)
		{
			this.Facade.SendNotification(notificationName);
		}

		public virtual void SendNotification(string notificationName, object body)
		{
			this.Facade.SendNotification(notificationName, body);
		}

		public virtual void SendNotification(string notificationName, object body, string type)
		{
			this.Facade.SendNotification(notificationName, body, type);
		}

		public void InitializeNotifier(string key)
		{
			this.MultitonKey = key;
		}

		public string MultitonKey { get; protected set; }

		protected IFacade Facade
		{
			get
			{
				if (this.MultitonKey == null)
				{
					throw new Exception("Multiton key for this Notifier not yet initialized!");
				}
				return PureMVC.Patterns.Facade.GetInstance(this.MultitonKey);
			}
		}
	}
}
