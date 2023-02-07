using System;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
	[Serializable]
	public class Observer : IObserver
	{
		public Observer(string notifyMethod, object notifyContext)
		{
			this.NotifyMethod = notifyMethod;
			this.NotifyContext = notifyContext;
		}

		public void NotifyObserver(INotification notification)
		{
			object notifyContext;
			lock (this)
			{
				notifyContext = this.NotifyContext;
			}
			if (notifyContext is IMediator)
			{
				IMediator mediator = notifyContext as IMediator;
				mediator.HandleNotification(notification);
			}
			else if (notifyContext is IController)
			{
				IController controller = notifyContext as IController;
				controller.ExecuteCommand(notification);
			}
		}

		public bool CompareNotifyContext(object obj)
		{
			bool result;
			lock (this)
			{
				result = this.NotifyContext.Equals(obj);
			}
			return result;
		}

		public string NotifyMethod { private get; set; }

		public object NotifyContext { private get; set; }
	}
}
