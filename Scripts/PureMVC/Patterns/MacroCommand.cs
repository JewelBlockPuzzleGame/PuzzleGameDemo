using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

namespace PureMVC.Patterns
{
	public class MacroCommand : Notifier, ICommand, INotifier
	{
		private readonly IList<object> m_subCommands;

		public MacroCommand()
		{
			this.m_subCommands = new List<object>();
			this.InitializeMacroCommand();
		}

		public MacroCommand(IEnumerable<Type> types)
		{
			this.m_subCommands = new List<object>(types);
			this.InitializeMacroCommand();
		}

		public MacroCommand(IEnumerable<ICommand> commands)
		{
			this.m_subCommands = new List<object>(commands);
			this.InitializeMacroCommand();
		}

		public MacroCommand(IEnumerable<object> commandCollection)
		{
			this.m_subCommands = new List<object>(commandCollection);
			this.InitializeMacroCommand();
		}

		public void Execute(INotification notification)
		{
			while (this.m_subCommands.Count > 0)
			{
				Type type = this.m_subCommands[0] as Type;
				if (type != null)
				{
					object obj = Activator.CreateInstance(type);
					if (obj is ICommand)
					{
						ICommand command = (ICommand)obj;
						command.InitializeNotifier(base.MultitonKey);
						command.Execute(notification);
					}
				}
				else
				{
					ICommand command2 = this.m_subCommands[0] as ICommand;
					if (command2 != null)
					{
						command2.InitializeNotifier(base.MultitonKey);
						command2.Execute(notification);
					}
				}
				this.m_subCommands.RemoveAt(0);
			}
		}

		protected virtual void InitializeMacroCommand()
		{
		}

		protected void AddSubCommand(Type commandType)
		{
			this.m_subCommands.Add(commandType);
		}

		protected void AddSubCommand(ICommand command)
		{
			this.m_subCommands.Add(command);
		}
	}
}
