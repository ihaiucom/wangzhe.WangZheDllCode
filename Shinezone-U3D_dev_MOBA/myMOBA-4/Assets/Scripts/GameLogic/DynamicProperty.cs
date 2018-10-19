using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class DynamicProperty : IComparer<object>
	{
		private class UpdatePropertyList
		{
			public uint deltaTime;

			public List<object> propertyList;
		}

		private uint dynamicPropertyConfig;

		private ulong lastSystemTime;

		private DictionaryView<uint, List<object>> _databin;

		private DictionaryView<uint, DynamicProperty.UpdatePropertyList> _updateDatabin;

		private ResBattleDynamicProperty _cprItem;

		public uint m_frameTimer
		{
			get;
			private set;
		}

		public void FightStart()
		{
			this._cprItem = new ResBattleDynamicProperty();
			this._databin = new DictionaryView<uint, List<object>>();
			this._updateDatabin = new DictionaryView<uint, DynamicProperty.UpdatePropertyList>();
			this.lastSystemTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			this.ResetTimer();
			GameDataMgr.battleDynamicPropertyDB.Accept(new Action<ResBattleDynamicProperty>(this.OnVisit));
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null)
			{
				this.dynamicPropertyConfig = curLvelContext.m_dynamicPropertyConfig;
			}
		}

		public void ResetTimer()
		{
			this.m_frameTimer = 0u;
		}

		private void OnVisit(ResBattleDynamicProperty InProperty)
		{
			uint dwID = InProperty.dwID;
			if (InProperty.bVarType == 1)
			{
				List<object> list;
				if (this._databin.ContainsKey(dwID))
				{
					list = this._databin[dwID];
				}
				else
				{
					list = new List<object>();
					this._databin.Add(dwID, list);
				}
				list.Add(InProperty);
			}
			else if (InProperty.bVarType == 2)
			{
				DynamicProperty.UpdatePropertyList updatePropertyList;
				if (this._updateDatabin.ContainsKey(dwID))
				{
					updatePropertyList = this._updateDatabin[dwID];
				}
				else
				{
					updatePropertyList = new DynamicProperty.UpdatePropertyList();
					updatePropertyList.deltaTime = 0u;
					updatePropertyList.propertyList = new List<object>();
					this._updateDatabin.Add(dwID, updatePropertyList);
				}
				updatePropertyList.propertyList.Add(InProperty);
			}
		}

		public void FightOver()
		{
			this._databin = null;
			this._cprItem = null;
		}

		public void UpdateActorProperty(ref ResBattleDynamicProperty _property)
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			for (int i = 0; i < heroActors.Count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = heroActors[i];
				if (_property.iSoulExp != 0 && ptr)
				{
					ptr.handle.ValueComponent.AddSoulExp((int)IncomeControl.GetCompensateExp(ptr.handle, (uint)_property.iSoulExp), false, AddSoulType.Dynamic);
					ptr.handle.ValueComponent.ChangeGoldCoinInBattle((int)_property.bGoldCoinInBattleAutoIncreaseValue, true, false, default(Vector3), false, default(PoolObjHandle<ActorRoot>));
				}
			}
		}

		public void UpdateLogic(int delta)
		{
			this.m_frameTimer += (uint)delta;
			if (this.dynamicPropertyConfig == 0u || this._updateDatabin == null || !this._updateDatabin.ContainsKey(this.dynamicPropertyConfig) || !Singleton<BattleLogic>.GetInstance().isFighting)
			{
				return;
			}
			DynamicProperty.UpdatePropertyList updatePropertyList = this._updateDatabin[this.dynamicPropertyConfig];
			for (int i = 0; i < updatePropertyList.propertyList.Count; i++)
			{
				ResBattleDynamicProperty resBattleDynamicProperty = (ResBattleDynamicProperty)updatePropertyList.propertyList[i];
				ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				if (logicFrameTick < (ulong)resBattleDynamicProperty.dwVarPara1 || i == updatePropertyList.propertyList.Count - 1)
				{
					updatePropertyList.deltaTime += (uint)(logicFrameTick - this.lastSystemTime);
					this.lastSystemTime = logicFrameTick;
					if (updatePropertyList.deltaTime >= resBattleDynamicProperty.dwVarPara2)
					{
						updatePropertyList.deltaTime -= resBattleDynamicProperty.dwVarPara2;
						this.UpdateActorProperty(ref resBattleDynamicProperty);
					}
					return;
				}
			}
		}

		public ResBattleDynamicProperty GetConfig(uint id, RES_BATTLE_DYNAMIC_PROPERTY_VAR dynVar)
		{
			if (id == 0u || this._databin == null || !this._databin.ContainsKey(id))
			{
				return null;
			}
			ResBattleDynamicProperty result = null;
			List<object> list = this._databin[id];
			if (dynVar == RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR)
			{
				this._cprItem.dwVarPara1 = this.m_frameTimer;
				int num = list.BinarySearch(this._cprItem, this);
				if (num < 0)
				{
					num = ~num;
					if (num > 0)
					{
						result = (ResBattleDynamicProperty)list[num - 1];
					}
					else if (num == 0)
					{
						result = (ResBattleDynamicProperty)list[num];
					}
				}
				else
				{
					result = (ResBattleDynamicProperty)list[num];
				}
			}
			return result;
		}

		public int GetDynamicReviveTime(uint id, int baseValue)
		{
			ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
			if (config != null)
			{
				baseValue = baseValue * config.iReviveTime / 10000;
			}
			return baseValue;
		}

		public uint GetDynamicSoulExp(uint id, int baseValue)
		{
			ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
			if (config != null)
			{
				baseValue = baseValue * config.iSoulExp / 10000;
			}
			return (uint)baseValue;
		}

		public uint GetDynamicGoldCoinInBattle(uint id, int baseValue, int floatRange)
		{
			ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
			if (config != null)
			{
				baseValue = baseValue * (int)config.wGoldCoinInBattleIncreaseRate / 10000;
			}
			int num = 0;
			if (floatRange > 0)
			{
				num = (int)FrameRandom.Random((uint)(floatRange * 2 + 1)) - floatRange;
			}
			int num2 = baseValue + num;
			if (num2 < 0)
			{
				num2 = 0;
			}
			return (uint)num2;
		}

		public int GetDynamicDamage(uint id, int baseValue)
		{
			ResBattleDynamicProperty config = this.GetConfig(id, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
			if (config != null)
			{
				baseValue = baseValue * config.iDamage / 10000;
			}
			return baseValue;
		}

		public int Compare(object l, object r)
		{
			return (int)(((ResBattleDynamicProperty)l).dwVarPara1 - ((ResBattleDynamicProperty)r).dwVarPara1);
		}

		public static int Adjustor(ValueDataInfo vdi)
		{
			int baseValue = vdi.baseValue;
			if (vdi.dynamicId < 1)
			{
				return baseValue;
			}
			ResBattleDynamicProperty config = Singleton<BattleLogic>.instance.dynamicProperty.GetConfig((uint)vdi.dynamicId, RES_BATTLE_DYNAMIC_PROPERTY_VAR.BATTLE_TIME_VAR);
			int num = 10000;
			if (config != null)
			{
				switch (vdi.Type)
				{
				case RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT:
					num = config.iAD;
					break;
				case RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT:
					num = config.iAP;
					break;
				case RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT:
					num = config.iDef;
					break;
				case RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT:
					num = config.iRes;
					break;
				case RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP:
					num = config.iHP;
					break;
				default:
					num = 10000;
					break;
				}
			}
			return baseValue * num / 10000;
		}
	}
}
