using System;
using Dxx.Util;

public class AchieveCondition1006 : AchieveConditionBase
{
	private int entityid;

	private int count;

	protected override void OnInit()
	{
		if (this.mArgs.Length != 2)
		{
			SdkManager.Bugly_Report(base.GetType().ToString(), Utils.FormatString("Args.Length:{0} != 2   !!!", new object[]
			{
				this.mArgs.Length
			}));
		}
		this.count = this.mData.maxcount;
		if (!int.TryParse(this.mArgs[1], out this.entityid))
		{
			SdkManager.Bugly_Report(base.GetType().ToString(), Utils.FormatString("Args[0] is not a int type", Array.Empty<object>()));
		}
	}

	protected override void OnExcute()
	{
		int killMonsters = GameLogic.Hold.BattleData.GetKillMonsters(this.entityid);
		LocalSave.Instance.Achieve_AddProgress(base.ID, killMonsters);
	}

	protected override string OnGetConditionString()
	{
		string languageByTID = GameLogic.Hold.Language.GetLanguageByTID(Utils.FormatString("monstername{0}", new object[]
		{
			this.entityid
		}), Array.Empty<object>());
		return GameLogic.Hold.Language.GetLanguageByTID(Utils.FormatString("成就_条件{0}", new object[]
		{
			this.mData.mData.CondType
		}), new object[]
		{
			languageByTID,
			this.count
		});
	}
}
