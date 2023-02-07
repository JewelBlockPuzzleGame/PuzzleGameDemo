using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace PureMVC.Core
{
	public class View : IView, IDisposable
	{
		protected string m_multitonKey;

		protected IDictionary<string, IMediator> m_mediatorMap;

		protected IDictionary<string, IList<IObserver>> m_observerMap;

		protected static volatile IView m_instance;

		protected static readonly IDictionary<string, IView> m_instanceMap = new Dictionary<string, IView>();

		public const string DEFAULT_KEY = "PureMVC";

		protected const string MULTITON_MSG = "View instance for this Multiton key already constructed!";

		protected View(string key)
		{
			this.m_multitonKey = key;
			this.m_mediatorMap = new Dictionary<string, IMediator>();
			this.m_observerMap = new Dictionary<string, IList<IObserver>>();
			if (View.m_instanceMap.ContainsKey(key))
			{
				throw new Exception("View instance for this Multiton key already constructed!");
			}
			View.m_instanceMap[key] = this;
			this.InitializeView();
		}

		protected View() : this("PureMVC")
		{
		}

		public virtual void RegisterObserver(string notificationName, IObserver observer)
		{
			if (!this.m_observerMap.ContainsKey(notificationName))
			{
				this.m_observerMap[notificationName] = new List<IObserver>();
			}
			this.m_observerMap[notificationName].Add(observer);
		}

		public virtual void NotifyObservers(INotification notification)
		{
			IList<IObserver> list = null;
			if (this.m_observerMap.ContainsKey(notification.Name))
			{
				IList<IObserver> collection = this.m_observerMap[notification.Name];
				list = new List<IObserver>(collection);
			}
			if (list == null)
			{
				return;
			}
			foreach (IObserver observer in list)
			{
				observer.NotifyObserver(notification);
			}
		}

		public virtual void RemoveObserver(string notificationName, object notifyContext)
		{
			if (!this.m_observerMap.ContainsKey(notificationName))
			{
				return;
			}
			IList<IObserver> list = this.m_observerMap[notificationName];
			object obj = list;
			lock (obj)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].CompareNotifyContext(notifyContext))
					{
						list.RemoveAt(i);
						break;
					}
				}
				if (list.Count == 0)
				{
					this.m_observerMap.Remove(notificationName);
				}
			}
		}

		public virtual void RegisterMediator(IMediator mediator)
		{
			object mediatorMap = this.m_mediatorMap;
			lock (mediatorMap)
			{
				if (this.m_mediatorMap.ContainsKey(mediator.MediatorName))
				{
					return;
				}
				mediator.InitializeNotifier(this.m_multitonKey);
				this.m_mediatorMap[mediator.MediatorName] = mediator;
				IEnumerable<string> listNotificationInterests = mediator.ListNotificationInterests;
				IObserver observer = new Observer("HandleNotification", mediator);
				foreach (string notificationName in listNotificationInterests)
				{
					this.RegisterObserver(notificationName, observer);
				}
				IObserver observer2 = new Observer("PublicNotification", mediator);
				this.RegisterObserver("PUB_NOTIFICATION", observer2);
			}
			mediator.OnRegister();
		}

		public virtual IMediator RetrieveMediator(string mediatorName)
		{
			if (!this.m_mediatorMap.ContainsKey(mediatorName))
			{
				return null;
			}
			return this.m_mediatorMap[mediatorName];
		}

		public virtual IMediator RemoveMediator(string mediatorName)
		{
			object mediatorMap = this.m_mediatorMap;
			IMediator result;
			lock (mediatorMap)
			{
				if (!this.m_mediatorMap.ContainsKey(mediatorName))
				{
					result = null;
				}
				else
				{
					IMediator mediator = this.m_mediatorMap[mediatorName];
					IEnumerable<string> listNotificationInterests = mediator.ListNotificationInterests;
					foreach (string notificationName in listNotificationInterests)
					{
						this.RemoveObserver(notificationName, mediator);
					}
					this.RemoveObserver("PUB_NOTIFICATION", mediator);
					this.m_mediatorMap.Remove(mediatorName);
					mediator.OnRemove();
					result = mediator;
				}
			}
			return result;
		}

		public virtual bool HasMediator(string mediatorName)
		{
			return this.m_mediatorMap.ContainsKey(mediatorName);
		}

		public IEnumerable<string> ListMediatorNames
		{
			get
			{
				return this.m_mediatorMap.Keys;
			}
		}

		public static void RemoveView(string key)
		{
			IView view;
			if (!View.m_instanceMap.TryGetValue(key, out view))
			{
				return;
			}
			View.m_instanceMap.Remove(key);
			view.Dispose();
		}

		public void Dispose()
		{
			View.RemoveView(this.m_multitonKey);
			this.m_observerMap.Clear();
			this.m_mediatorMap.Clear();
		}

		public static IView Instance
		{
			get
			{
				return View.GetInstance("PureMVC");
			}
		}

		public static IView GetInstance()
		{
			return View.GetInstance("PureMVC");
		}

		public static IView GetInstance(string key)
		{
			IView view;
			if (View.m_instanceMap.TryGetValue(key, out view))
			{
				return view;
			}
			view = new View(key);
			View.m_instanceMap[key] = view;
			return view;
		}

		protected virtual void InitializeView()
		{
		}
	}
}
