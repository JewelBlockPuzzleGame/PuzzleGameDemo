using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using UnityEngine;

namespace PureMVC.Patterns
{
	public class Mediator : Notifier, IMediator, INotifier
	{
		public const string NAME = "Mediator";

		protected string m_mediatorName;

		protected object m_viewComponent;

		public Mediator() : this("Mediator", null)
		{
		}

		public Mediator(string mediatorName) : this(mediatorName, null)
		{
		}

		public Mediator(string mediatorName, object viewComponent)
		{
			this.m_mediatorName = (mediatorName ?? "Mediator");
			this.m_viewComponent = viewComponent;
		}

		public virtual IEnumerable<string> ListNotificationInterests
		{
			get
			{
				return new List<string>();
			}
		}

		public virtual void HandleNotification(INotification notification)
		{
		}

		public virtual void OnRegister()
		{
		}

		public virtual void OnRemove()
		{
		}

		public virtual void PublicNotification(INotification notification)
		{
		}

		public virtual void Blur(bool blur)
		{
		}

		public virtual object GetEvent(string eventName)
		{
			return null;
		}

		public virtual string MediatorName
		{
			get
			{
				return this.m_mediatorName;
			}
		}

		public object ViewComponent
		{
			get
			{
				return this.m_viewComponent;
			}
			set
			{
				this.m_viewComponent = value;
			}
		}
	}
}
