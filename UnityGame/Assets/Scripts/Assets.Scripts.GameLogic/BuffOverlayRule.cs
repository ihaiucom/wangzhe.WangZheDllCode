using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BuffOverlayRule
	{
		private BuffHolderComponent buffHolder;

		public void Init(BuffHolderComponent _buffHolder)
		{
			this.buffHolder = _buffHolder;
		}

		public bool CheckTriggerRate(BuffSkill _buffSkill)
		{
			if (_buffSkill.cfgData.iTriggerRate > 0)
			{
				int num = (int)FrameRandom.Random(10000u);
				return num < _buffSkill.cfgData.iTriggerRate;
			}
			return true;
		}

		public bool CheckDependCondition(BuffSkill _buffSkill)
		{
			if (_buffSkill.cfgData.iDependCfgID != 0)
			{
				if (this.buffHolder.FindBuff(_buffSkill.cfgData.iDependCfgID) == null)
				{
					return false;
				}
				this.buffHolder.RemoveBuff(_buffSkill.cfgData.iDependCfgID);
			}
			return true;
		}

		public bool CheckMutexCondition(BuffSkill _buffSkill)
		{
			return (_buffSkill.cfgData.iMutexCfgID1 == 0 || this.buffHolder.FindBuff(_buffSkill.cfgData.iMutexCfgID1) == null) && (_buffSkill.cfgData.iMutexCfgID2 == 0 || this.buffHolder.FindBuff(_buffSkill.cfgData.iMutexCfgID2) == null) && (_buffSkill.cfgData.iMutexCfgID3 == 0 || this.buffHolder.FindBuff(_buffSkill.cfgData.iMutexCfgID3) == null);
		}

		private void SetOverlayCount(int inSkillCombineId, int count)
		{
			if (this.buffHolder.SpawnedBuffList != null)
			{
				for (int i = 0; i < this.buffHolder.SpawnedBuffList.get_Count(); i++)
				{
					BuffSkill buffSkill = this.buffHolder.SpawnedBuffList.get_Item(i);
					if (buffSkill != null && buffSkill.SkillID == inSkillCombineId)
					{
						buffSkill.SetOverlayCount(count);
					}
				}
			}
		}

		public bool CheckOverlay(BuffSkill _buffSkill)
		{
			if (_buffSkill.cfgData == null)
			{
				return false;
			}
			if (!this.CheckTriggerRate(_buffSkill))
			{
				return false;
			}
			if (!this.CheckMutexCondition(_buffSkill))
			{
				return false;
			}
			if (!this.CheckDependCondition(_buffSkill))
			{
				return false;
			}
			if (_buffSkill.cfgData.bOverlayMax < 1)
			{
				_buffSkill.SetOverlayCount(1);
				if (this.buffHolder.FindBuffCount(_buffSkill.SkillID) == 0)
				{
					_buffSkill.bFirstEffect = true;
				}
				return true;
			}
			BuffSkill buffSkill = this.buffHolder.FindBuff(_buffSkill.SkillID);
			if (buffSkill == null || buffSkill.GetOverlayCount() < (int)_buffSkill.cfgData.bOverlayMax)
			{
				int num = 0;
				if (buffSkill != null)
				{
					num = buffSkill.GetOverlayCount();
					this.SetOverlayCount(buffSkill.SkillID, num + 1);
					this.buffHolder.RemoveBuff(_buffSkill.SkillID);
				}
				_buffSkill.SetOverlayCount(num + 1);
				return true;
			}
			if (_buffSkill.cfgData.bOverlayRule == 1)
			{
				return false;
			}
			if (_buffSkill.cfgData.bOverlayRule == 2)
			{
				this.buffHolder.RemoveBuff(_buffSkill.SkillID);
				return false;
			}
			if (_buffSkill.cfgData.bOverlayRule == 3)
			{
				this.buffHolder.RemoveBuff(_buffSkill.SkillID);
				_buffSkill.SetOverlayCount(1);
				return true;
			}
			if (_buffSkill.cfgData.bOverlayRule == 4)
			{
				int overlayCount = buffSkill.GetOverlayCount();
				this.buffHolder.RemoveBuff(_buffSkill.SkillID);
				_buffSkill.SetOverlayCount(overlayCount);
				return true;
			}
			return false;
		}
	}
}
