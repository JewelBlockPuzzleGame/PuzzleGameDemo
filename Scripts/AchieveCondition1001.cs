using System;
using Dxx.Util;

public class AchieveCondition1001 : AchieveConditionBase
{
	private int maxlevel;

	private int hittedcount;

	protected override void OnInit()
	{
		if (this.mArgs.Length != 3)
		{
			SdkManager.Bugly_Report("AchieveCondition1001", Utils.FormatString("Args.Length:{0} != 3   !!!", new object[]
			{
				this.mArgs.Length
			}));
		}
		if (!int.TryParse(this.mArgs[1], out this.maxlevel))
		{
			SdkManager.Bugly_Report("AchieveCondition1001", Utils.FormatString("Args[1] is not a int type", Array.Empty<object>()));
		}
		if (!int.TryParse(this.mArgs[2], out this.hittedcount))
		{
			SdkManager.Bugly_Report("AchieveCondition1001", Utils.FormatString("Args[2] is not a int type", Array.Empty<object>()));
		}
	}

	protected override void OnExcute()
	{
		if (GameLogic.Release.Mode.RoomGenerate.GetCurrentRoomID() >= this.maxlevel && GameLogic.Hold.BattleData.GetHittedCount(this.maxlevel) <= this.hittedcount)
		{
			LocalSave.Instance.Achieve_AddProgress(base.ID, 1);
		}
	}

	protected override bool OnIsFinish()
	{
		return GameLogic.Hold.BattleData.GetHittedCount() <= this.hittedcount;
	}

	protected override string OnGetBattleMaxString()
	{
		return this.hittedcount.ToString();
	}

	protected override string OnGetConditionString()
	{
		return GameLogic.Hold.Language.GetLanguageByTID(Utils.FormatString("成就_条件{0}", new object[]
		{
			this.mData.mData.CondType
		}), new object[]
		{
			this.maxlevel.ToString(),
			this.hittedcount.ToString()
		});
	}
}
