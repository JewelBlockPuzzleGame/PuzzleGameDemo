using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

namespace PureMVC.Core
{
	public class Model : IModel, IDisposable
	{
		protected string m_multitonKey;

		protected IDictionary<string, IProxy> m_proxyMap;

		protected static volatile IModel m_instance;

		protected static readonly IDictionary<string, IModel> m_instanceMap = new Dictionary<string, IModel>();

		public const string DEFAULT_KEY = "PureMVC";

		protected const string MULTITON_MSG = "Model instance for this Multiton key already constructed!";

		public Model(string key)
		{
			this.m_multitonKey = key;
			this.m_proxyMap = new Dictionary<string, IProxy>();
			if (Model.m_instanceMap.ContainsKey(key))
			{
				throw new Exception("Model instance for this Multiton key already constructed!");
			}
			Model.m_instanceMap[key] = this;
			this.InitializeModel();
		}

		public Model() : this("PureMVC")
		{
		}

		public virtual void RegisterProxy(IProxy proxy)
		{
			proxy.InitializeNotifier(this.m_multitonKey);
			this.m_proxyMap[proxy.ProxyName] = proxy;
			proxy.OnRegister();
		}

		public virtual IProxy RetrieveProxy(string proxyName)
		{
			if (!this.m_proxyMap.ContainsKey(proxyName))
			{
				return null;
			}
			return this.m_proxyMap[proxyName];
		}

		public virtual bool HasProxy(string proxyName)
		{
			return this.m_proxyMap.ContainsKey(proxyName);
		}

		public IEnumerable<string> ListProxyNames
		{
			get
			{
				return this.m_proxyMap.Keys;
			}
		}

		public virtual IProxy RemoveProxy(string proxyName)
		{
			IProxy proxy = null;
			if (this.m_proxyMap.ContainsKey(proxyName))
			{
				proxy = this.RetrieveProxy(proxyName);
				this.m_proxyMap.Remove(proxyName);
			}
			if (proxy != null)
			{
				proxy.OnRemove();
			}
			return proxy;
		}

		public static IModel Instance
		{
			get
			{
				return Model.GetInstance("PureMVC");
			}
		}

		public static IModel GetInstance(string key)
		{
			IModel model;
			if (Model.m_instanceMap.TryGetValue(key, out model))
			{
				return model;
			}
			model = new Model(key);
			Model.m_instanceMap[key] = model;
			return model;
		}

		protected virtual void InitializeModel()
		{
		}

		public static void RemoveModel(string key)
		{
			IModel model;
			if (!Model.m_instanceMap.TryGetValue(key, out model))
			{
				return;
			}
			Model.m_instanceMap.Remove(key);
			model.Dispose();
		}

		public void Dispose()
		{
			Model.RemoveModel(this.m_multitonKey);
			this.m_proxyMap.Clear();
		}
	}
}
