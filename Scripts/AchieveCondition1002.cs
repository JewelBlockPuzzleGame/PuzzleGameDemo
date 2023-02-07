using System;
using Dxx.Util;

public class AchieveCondition1002 : AchieveConditionBase
{
	private int alltime;

	protected override void OnInit()
	{
		if (this.mArgs.Length != 2)
		{
			SdkManager.Bugly_Report(base.GetType().ToString(), Utils.FormatString("Args.Length:{0} != 2   !!!", new object[]
			{
				this.mArgs.Length
			}));
		}
		if (!int.TryParse(this.mArgs[1], out this.alltime))
		{
			SdkManager.Bugly_Report(base.GetType().ToString(), Utils.FormatString("Args[1] is not a int type", Array.Empty<object>()));
		}
	}

	protected override void OnExcute()
	{
		if (GameLogic.Hold.BattleData.Win && GameLogic.Hold.BattleData.GetGameTime() <= this.alltime)
		{
			LocalSave.Instance.Achieve_AddProgress(base.ID, 1);
		}
	}

	protected override bool OnIsFinish()
	{
		return GameLogic.Hold.BattleData.GetGameTime() <= this.alltime;
	}

	protected override string OnGetBattleMaxString()
	{
		return Utils.GetSecond2String(this.alltime);
	}

	protected override string OnGetConditionString()
	{
		int num = this.alltime / 60;
		int num2 = this.alltime % 60;
		string text = string.Empty;
		if (num2 != 0)
		{
			text = GameLogic.Hold.Language.GetLanguageByTID("成就_时间分秒", new object[]
			{
				num,
				num2
			});
		}
		else
		{
			text = GameLogic.Hold.Language.GetLanguageByTID("成就_时间分", new object[]
			{
				num
			});
		}
		return GameLogic.Hold.Language.GetLanguageByTID(Utils.FormatString("成就_条件{0}", new object[]
		{
			this.mData.mData.CondType
		}), new object[]
		{
			text
		});
	}
}
