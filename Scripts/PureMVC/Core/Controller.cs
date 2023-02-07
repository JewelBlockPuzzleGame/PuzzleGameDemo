using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace PureMVC.Core
{
	public class Controller : IController, IDisposable
	{
		protected string m_multitonKey;

		private IView m_view;

		private readonly IDictionary<string, object> m_commandMap;

		protected static readonly IDictionary<string, IController> m_instanceMap = new Dictionary<string, IController>();

		public const string DEFAULT_KEY = "PureMVC";

		protected const string MULTITON_MSG = "Controller instance for this Multiton key already constructed!";

		public Controller(string key)
		{
			this.m_multitonKey = key;
			this.m_commandMap = new Dictionary<string, object>();
			if (Controller.m_instanceMap.ContainsKey(key))
			{
				throw new Exception("Controller instance for this Multiton key already constructed!");
			}
			Controller.m_instanceMap[key] = this;
			this.InitializeController();
		}

		public Controller() : this("PureMVC")
		{
		}

		public void ExecuteCommand(INotification notification)
		{
			if (!this.m_commandMap.ContainsKey(notification.Name))
			{
				return;
			}
			object obj = this.m_commandMap[notification.Name];
			Type type = obj as Type;
			ICommand command;
			if (type != null)
			{
				object obj2 = Activator.CreateInstance(type);
				command = (obj2 as ICommand);
				if (command == null)
				{
					return;
				}
			}
			else
			{
				command = (obj as ICommand);
				if (command == null)
				{
					return;
				}
			}
			command.InitializeNotifier(this.m_multitonKey);
			command.Execute(notification);
		}

		public void RegisterCommand(string notificationName, Type commandType)
		{
			if (!this.m_commandMap.ContainsKey(notificationName))
			{
				this.m_view.RegisterObserver(notificationName, new Observer("executeCommand", this));
			}
			this.m_commandMap[notificationName] = commandType;
		}

		public void RegisterCommand(string notificationName, ICommand command)
		{
			if (!this.m_commandMap.ContainsKey(notificationName))
			{
				this.m_view.RegisterObserver(notificationName, new Observer("executeCommand", this));
			}
			command.InitializeNotifier(this.m_multitonKey);
			this.m_commandMap[notificationName] = command;
		}

		public bool HasCommand(string notificationName)
		{
			return this.m_commandMap.ContainsKey(notificationName);
		}

		public object RemoveCommand(string notificationName)
		{
			if (!this.m_commandMap.ContainsKey(notificationName))
			{
				return null;
			}
			this.m_view.RemoveObserver(notificationName, this);
			object result = this.m_commandMap[notificationName];
			this.m_commandMap.Remove(notificationName);
			return result;
		}

		public static IController Instance
		{
			get
			{
				return Controller.GetInstance("PureMVC");
			}
		}

		public static IController GetInstance()
		{
			return Controller.GetInstance("PureMVC");
		}

		public static IController GetInstance(string key)
		{
			IController controller;
			if (Controller.m_instanceMap.TryGetValue(key, out controller))
			{
				return controller;
			}
			controller = new Controller(key);
			Controller.m_instanceMap[key] = controller;
			return controller;
		}

		private void InitializeController()
		{
			this.m_view = View.GetInstance(this.m_multitonKey);
		}

		public IEnumerable<string> ListNotificationNames
		{
			get
			{
				return this.m_commandMap.Keys;
			}
		}

		public void Dispose()
		{
			Controller.RemoveController(this.m_multitonKey);
			this.m_commandMap.Clear();
		}

		public static void RemoveController(string key)
		{
			IController controller;
			if (!Controller.m_instanceMap.TryGetValue(key, out controller))
			{
				return;
			}
			Controller.m_instanceMap.Remove(key);
			controller.Dispose();
		}
	}
}
