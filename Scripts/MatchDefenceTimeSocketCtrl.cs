using System;
using System.Text;
using BestHTTP.WebSocket;
using Newtonsoft.Json;

public class MatchDefenceTimeSocketCtrl : Singleton<MatchDefenceTimeSocketCtrl>
{
	private const string url = "wss://";

	private WebSocket webSocket;

	private MatchDefenceTimeSocketCtrl.ConnectState mState = MatchDefenceTimeSocketCtrl.ConnectState.eClose;

	private string otheruserid = string.Empty;

	private string myname = string.Empty;

	public MatchDefenceTimeSocketCtrl.ConnectState State
	{
		get
		{
			return this.mState;
		}
	}

	public void init()
	{
		if (this.webSocket != null)
		{
			return;
		}
		this.webSocket = new WebSocket(new Uri("wss://api-archer.habby.mobi:4443/matching"));
		WebSocket webSocket = this.webSocket;
		webSocket.OnOpen = (OnWebSocketOpenDelegate)Delegate.Combine(webSocket.OnOpen, new OnWebSocketOpenDelegate(this.OnOpen));
		WebSocket webSocket2 = this.webSocket;
		webSocket2.OnMessage = (OnWebSocketMessageDelegate)Delegate.Combine(webSocket2.OnMessage, new OnWebSocketMessageDelegate(this.OnMessageReceived));
		WebSocket webSocket3 = this.webSocket;
		webSocket3.OnBinary = (OnWebSocketBinaryDelegate)Delegate.Combine(webSocket3.OnBinary, new OnWebSocketBinaryDelegate(this.OnBinaryReceived));
		WebSocket webSocket4 = this.webSocket;
		webSocket4.OnError = (OnWebSocketErrorDelegate)Delegate.Combine(webSocket4.OnError, new OnWebSocketErrorDelegate(this.OnError));
		WebSocket webSocket5 = this.webSocket;
		webSocket5.OnClosed = (OnWebSocketClosedDelegate)Delegate.Combine(webSocket5.OnClosed, new OnWebSocketClosedDelegate(this.OnClosed));
	}

	public void Deinit()
	{
		if (this.webSocket == null)
		{
			return;
		}
		this.webSocket.Close();
	}

	public void Connect()
	{
		if (this.webSocket != null)
		{
			return;
		}
		this.mState = MatchDefenceTimeSocketCtrl.ConnectState.eConnecting;
		this.init();
		this.otheruserid = string.Empty;
		this.webSocket.Open();
	}

	public void Close()
	{
		if (this.webSocket == null)
		{
			return;
		}
		this.Send(MatchMessageType.eUserLeave, 0);
		this.webSocket.OnOpen = null;
		this.webSocket.OnMessage = null;
		this.webSocket.OnError = null;
		this.webSocket.OnClosed = null;
		this.webSocket.Close();
		this.webSocket = null;
		this.mState = MatchDefenceTimeSocketCtrl.ConnectState.eClose;
	}

	public bool IsConnected
	{
		get
		{
			return this.mState == MatchDefenceTimeSocketCtrl.ConnectState.eConnected;
		}
	}

	private byte[] getBytes(string message)
	{
		return Encoding.Default.GetBytes(message);
	}

	public void Send(MatchMessageType msgtype, int arg = 0)
	{
		if (this.webSocket == null || !this.webSocket.IsOpen)
		{
			return;
		}
		MatchMessage matchMessage = new MatchMessage();
		this.myname = LocalSave.Instance.GetUserName();
		matchMessage.userid = LocalSave.Instance.GetServerUserID().ToString();
		if (this.myname == string.Empty)
		{
			this.myname = matchMessage.userid;
		}
		matchMessage.nickname = this.myname;
		matchMessage.msgtype = (short)msgtype;
		matchMessage.argint = arg;
		this.Send(JsonConvert.SerializeObject(matchMessage));
	}

	public void Send(string str)
	{
		this.webSocket.Send(str);
	}

	private void OnOpen(WebSocket ws)
	{
		Debugger.Log("connected");
		this.mState = MatchDefenceTimeSocketCtrl.ConnectState.eConnected;
		this.Send(MatchMessageType.eUserComeIn, 0);
	}

	private void OnMessageReceived(WebSocket ws, string message)
	{
		try
		{
			MatchMessage matchMessage = JsonConvert.DeserializeObject<MatchMessage>(message);
			if (!string.IsNullOrEmpty(matchMessage.userid))
			{
				if (string.IsNullOrEmpty(this.otheruserid))
				{
					this.otheruserid = matchMessage.userid;
				}
				else if (this.otheruserid != matchMessage.userid)
				{
					return;
				}
			}
			MatchMessageType msgtype = (MatchMessageType)matchMessage.msgtype;
			switch (msgtype)
			{
			case MatchMessageType.eLearnSkill:
				GameLogic.Hold.BattleData.Challenge_SendEvent("MatchDefenceTime_other_learn_skill", matchMessage.argint);
				break;
			case MatchMessageType.eDead:
				GameLogic.Hold.BattleData.Challenge_SendEvent("MatchDefenceTime_other_dead", null);
				break;
			case MatchMessageType.eReborn:
				GameLogic.Hold.BattleData.Challenge_SendEvent("MatchDefenceTime_other_reborn", null);
				break;
			case MatchMessageType.eScoreUpdate:
				GameLogic.Hold.BattleData.Challenge_SendEvent("MatchDefenceTime_other_updatescore", matchMessage.argint);
				break;
			default:
				if (msgtype != MatchMessageType.eGameStart)
				{
					if (msgtype != MatchMessageType.eGameEnd)
					{
					}
				}
				else
				{
					long num = (long)matchMessage.argint;
					GameLogic.Hold.BattleData.Challenge_UpdateMode(13101, BattleSource.eMatch);
					GameLogic.Hold.BattleData.Challenge_SendEvent("MatchDefenceTime_set_random_seed", num);
					WindowUI.ShowWindow(WindowID.WindowID_Battle);
					GameLogic.Hold.BattleData.Challenge_SendEvent("MatchDefenceTime_me_updatename", this.myname);
					GameLogic.Hold.BattleData.Challenge_SendEvent("MatchDefenceTime_other_updatename", matchMessage.nickname);
				}
				break;
			}
		}
		catch
		{
		}
	}

	private void OnBinaryReceived(WebSocket ws, byte[] bytes)
	{
		Debugger.Log(Encoding.ASCII.GetString(bytes));
	}

	private void OnClosed(WebSocket ws, ushort code, string message)
	{
		Debugger.Log("onclose " + message);
	}

	private new void OnDestroy()
	{
		if (this.webSocket != null && this.webSocket.IsOpen)
		{
			this.webSocket.Close();
			this.Deinit();
		}
	}

	private void OnError(WebSocket ws, Exception ex)
	{
		string str = string.Empty;
		if (ws.InternalRequest.Response != null)
		{
			str = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
		}
		Debugger.Log("OnError " + str);
	}

	public enum ConnectState
	{
		eConnecting,
		eConnected,
		eClose
	}
}
