using System;
using System.Collections.Generic;
using PureMVC.Core;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
	public class Facade : Notifier, IFacade, INotifier, IDisposable
	{
		protected IController m_controller;

		protected IModel m_model;

		protected IView m_view;

		protected static readonly IDictionary<string, IFacade> m_instanceMap = new Dictionary<string, IFacade>();

		public const string DEFAULT_KEY = "PureMVC";

		protected new const string MULTITON_MSG = "Facade instance for this Multiton key already constructed!";

		public Facade(string key)
		{
			base.InitializeNotifier(key);
			PureMVC.Patterns.Facade.m_instanceMap[key] = this;
			this.InitializeFacade();
		}

		public Facade() : this("PureMVC")
		{
		}

		public void RegisterProxy(IProxy proxy)
		{
			this.m_model.RegisterProxy(proxy);
		}

		public IProxy RetrieveProxy(string proxyName)
		{
			return this.m_model.RetrieveProxy(proxyName);
		}

		public IProxy RemoveProxy(string proxyName)
		{
			return this.m_model.RemoveProxy(proxyName);
		}

		public bool HasProxy(string proxyName)
		{
			return this.m_model.HasProxy(proxyName);
		}

		public void RegisterCommand(string notificationName, Type commandType)
		{
			this.m_controller.RegisterCommand(notificationName, commandType);
		}

		public void RegisterCommand(string notificationName, ICommand command)
		{
			this.m_controller.RegisterCommand(notificationName, command);
		}

		public object RemoveCommand(string notificationName)
		{
			return this.m_controller.RemoveCommand(notificationName);
		}

		public bool HasCommand(string notificationName)
		{
			return this.m_controller.HasCommand(notificationName);
		}

		public void RegisterMediator(IMediator mediator)
		{
			this.m_view.RegisterMediator(mediator);
		}

		public IMediator RetrieveMediator(string mediatorName)
		{
			return this.m_view.RetrieveMediator(mediatorName);
		}

		public IMediator RemoveMediator(string mediatorName)
		{
			return this.m_view.RemoveMediator(mediatorName);
		}

		public bool HasMediator(string mediatorName)
		{
			return this.m_view.HasMediator(mediatorName);
		}

		public void NotifyObservers(INotification notification)
		{
			this.m_view.NotifyObservers(notification);
		}

		public override void SendNotification(string notificationName)
		{
			this.NotifyObservers(new Notification(notificationName));
		}

		public override void SendNotification(string notificationName, object body)
		{
			this.NotifyObservers(new Notification(notificationName, body));
		}

		public override void SendNotification(string notificationName, object body, string type)
		{
			this.NotifyObservers(new Notification(notificationName, body, type));
		}

		public static IFacade Instance
		{
			get
			{
				return PureMVC.Patterns.Facade.GetInstance("PureMVC");
			}
		}

		public static IFacade GetInstance()
		{
			return PureMVC.Patterns.Facade.GetInstance("PureMVC");
		}

		public static IFacade GetInstance(string key)
		{
			IFacade facade;
			if (PureMVC.Patterns.Facade.m_instanceMap.TryGetValue(key, out facade))
			{
				return facade;
			}
			facade = new Facade(key);
			PureMVC.Patterns.Facade.m_instanceMap[key] = facade;
			return facade;
		}

		public static bool HasCore(string key)
		{
			return PureMVC.Patterns.Facade.m_instanceMap.ContainsKey(key);
		}

		public static IEnumerable<string> ListCore
		{
			get
			{
				return PureMVC.Patterns.Facade.m_instanceMap.Keys;
			}
		}

		public static void RemoveCore(string key)
		{
			IFacade facade;
			if (!PureMVC.Patterns.Facade.m_instanceMap.TryGetValue(key, out facade))
			{
				return;
			}
			PureMVC.Patterns.Facade.m_instanceMap.Remove(key);
			facade.Dispose();
			Model.RemoveModel(key);
			Controller.RemoveController(key);
			View.RemoveView(key);
		}

		public void Dispose()
		{
			this.m_view = null;
			this.m_model = null;
			this.m_controller = null;
			PureMVC.Patterns.Facade.m_instanceMap.Remove(base.MultitonKey);
		}

		public static void BroadcastNotification(INotification notification)
		{
			foreach (KeyValuePair<string, IFacade> keyValuePair in PureMVC.Patterns.Facade.m_instanceMap)
			{
				keyValuePair.Value.NotifyObservers(notification);
			}
		}

		public static void BroadcastNotification(string notificationName)
		{
			PureMVC.Patterns.Facade.BroadcastNotification(new Notification(notificationName));
		}

		public static void BroadcastNotification(string notificationName, object body)
		{
			PureMVC.Patterns.Facade.BroadcastNotification(new Notification(notificationName, body));
		}

		public static void BroadcastNotification(string notificationName, object body, string type)
		{
			PureMVC.Patterns.Facade.BroadcastNotification(new Notification(notificationName, body, type));
		}

		protected virtual void InitializeFacade()
		{
			this.InitializeModel();
			this.InitializeController();
			this.InitializeView();
		}

		protected virtual void InitializeController()
		{
			if (this.m_controller != null)
			{
				return;
			}
			this.m_controller = Controller.GetInstance(base.MultitonKey);
		}

		protected virtual void InitializeModel()
		{
			if (this.m_model != null)
			{
				return;
			}
			this.m_model = Model.GetInstance(base.MultitonKey);
		}

		protected virtual void InitializeView()
		{
			if (this.m_view != null)
			{
				return;
			}
			this.m_view = View.GetInstance(base.MultitonKey);
		}
	}
}
