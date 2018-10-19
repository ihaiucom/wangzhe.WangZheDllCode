using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class BuffClearRule
	{
		private BuffHolderComponent buffHolder;

		private List<int> CacheBufferList = new List<int>();

		private List<BuffSkill> TempBuffList = new List<BuffSkill>(4);

		public void Init(BuffHolderComponent _buffHolder)
		{
			this.buffHolder = _buffHolder;
		}

		private void CopyList(List<BuffSkill> Source, List<BuffSkill> Dest)
		{
			Dest.Clear();
			for (int i = 0; i < Source.Count; i++)
			{
				Dest.Add(Source[i]);
			}
		}

		public void CheckBuffClear(RES_SKILLFUNC_CLEAR_RULE _rule)
		{
			if (this.buffHolder.SpawnedBuffList.Count == 0)
			{
				return;
			}
			this.CopyList(this.buffHolder.SpawnedBuffList, this.TempBuffList);
			for (int i = 0; i < this.TempBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.TempBuffList[i];
				if (buffSkill.cfgData.bClearRule == (byte)_rule)
				{
					buffSkill.Stop();
				}
			}
			this.TempBuffList.Clear();
		}

		public void CheckBuffNoClear(RES_SKILLFUNC_CLEAR_RULE _rule)
		{
			if (this.buffHolder.SpawnedBuffList.Count == 0)
			{
				return;
			}
			this.CopyList(this.buffHolder.SpawnedBuffList, this.TempBuffList);
			for (int i = 0; i < this.TempBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.TempBuffList[i];
				if (buffSkill.cfgData.bClearRule != (byte)_rule && buffSkill.cfgData.bEffectType != 3)
				{
					buffSkill.Stop();
				}
			}
			this.TempBuffList.Clear();
		}

		public void CacheClearBuff(BuffSkill _buff, RES_SKILLFUNC_CLEAR_RULE _rule)
		{
			if (_buff.cfgData.bClearRule == (byte)_rule && _buff.cfgData.bEffectType == 3)
			{
				this.CacheBufferList.Add(_buff.cfgData.iCfgID);
			}
		}

		public void RecoverClearBuff()
		{
			for (int i = 0; i < this.CacheBufferList.Count; i++)
			{
				int inSkillCombineId = this.CacheBufferList[i];
				SkillUseParam skillUseParam = default(SkillUseParam);
				skillUseParam.SetOriginator(this.buffHolder.actorPtr);
				this.buffHolder.actor.SkillControl.SpawnBuff(this.buffHolder.actorPtr, ref skillUseParam, inSkillCombineId, false);
			}
			this.CacheBufferList.Clear();
		}
	}
}
