using System;
using System.Collections.Generic;
using PureMVC.Interfaces;

public class MaskModuleMediator : WindowMediator, IMediator, INotifier
{
	public MaskModuleMediator() : base("MaskUIPanel")
	{
	}

	protected override void OnRegisterOnce()
	{
	}

	protected override void OnRegisterEvery()
	{
		this._MonoView.transform.SetAsLastSibling();
	}

	protected override void OnRemoveAfter()
	{
	}

	public override List<string> OnListNotificationInterests
	{
		get
		{
			return new List<string>();
		}
	}

	public override void OnHandleNotification(INotification notification)
	{
		string name = notification.Name;
		object body = notification.Body;
		if (name != null)
		{
		}
	}

	protected override void OnLanguageChange()
	{
	}
}
