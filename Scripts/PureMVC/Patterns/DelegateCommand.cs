using System;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
	public class DelegateCommand : Notifier, ICommand, INotifier
	{
		private readonly Action<INotification> m_action;

		public DelegateCommand(Action<INotification> action)
		{
			this.m_action = action;
		}

		public virtual void Execute(INotification notification)
		{
			this.m_action(notification);
		}
	}
}
