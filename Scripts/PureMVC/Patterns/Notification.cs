using System;
using System.Diagnostics;
using System.Text;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
	[Serializable]
	public class Notification : INotification
	{
		private StringBuilder strTemp = new StringBuilder();

		public Notification(string name) : this(name, null, null)
		{
		}

		public Notification(string name, object body) : this(name, body, null)
		{
		}

		public Notification(string name, object body, string type)
		{
			this.Name = name;
			this.Body = body;
			this.Type = type;
		}

		public void getDebugInfo()
		{
			StackTrace stackTrace = new StackTrace(true);
			StackFrame frame = stackTrace.GetFrame(5);
			if (frame != null)
			{
				this.FileName = frame.GetFileName();
				this.FuncName = frame.GetMethod().Name;
				this.LineNumber = frame.GetFileLineNumber();
			}
		}

		public override string ToString()
		{
			this.strTemp.Clear();
			this.strTemp.AppendFormat("Notification Name: {0}", this.Name);
			this.strTemp.AppendFormat("{0}Body:{1}", Environment.NewLine, (this.Body != null) ? this.Body.ToString() : "null");
			this.strTemp.AppendFormat("{0}Type:{1}", Environment.NewLine, this.Type ?? "null");
			return this.strTemp.ToString();
		}

		public string Name { get; private set; }

		public object Body { get; set; }

		public string Type { get; set; }

		public string FileName { get; private set; }

		public string FuncName { get; private set; }

		public int LineNumber { get; private set; }
	}
}
