using System;
using PureMVC.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class MatchDefenceTimeUICtrl : MediatorCtrlBase
{
	public ButtonCtrl Button_Match;

	public Text Text_Match;

	public GameObject match_obj;

	protected override void OnInit()
	{
		this.Button_Match.onClick = delegate()
		{
			MatchDefenceTimeSocketCtrl.ConnectState state = Singleton<MatchDefenceTimeSocketCtrl>.Instance.State;
			if (state != MatchDefenceTimeSocketCtrl.ConnectState.eConnected)
			{
				if (state == MatchDefenceTimeSocketCtrl.ConnectState.eClose)
				{
					this.StartMatch();
					Singleton<MatchDefenceTimeSocketCtrl>.Instance.Connect();
				}
			}
			else
			{
				this.StopMatch();
				Singleton<MatchDefenceTimeSocketCtrl>.Instance.Close();
			}
		};
		RectTransform rectTransform = this.Button_Match.transform.parent as RectTransform;
		rectTransform.anchoredPosition = new Vector2(0f, (float)GameLogic.Height * 0.23f);
	}

	protected override void OnOpen()
	{
		this.StopMatch();
		this.InitUI();
	}

	private void InitUI()
	{
	}

	private void StartMatch()
	{
		this.match_obj.SetActive(true);
		this.Text_Match.text = "取消匹配";
	}

	private void StopMatch()
	{
		this.match_obj.SetActive(false);
		this.Text_Match.text = "匹配";
	}

	protected override void OnClose()
	{
	}

	public override object OnGetEvent(string eventName)
	{
		return null;
	}

	public override void OnHandleNotification(INotification notification)
	{
	}

	public override void OnLanguageChange()
	{
	}
}
