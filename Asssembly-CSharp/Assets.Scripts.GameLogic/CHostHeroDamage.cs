using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class CHostHeroDamage
	{
		private PoolObjHandle<ActorRoot> m_actorHero;

		private PoolObjHandle<ActorRoot> m_actorKiller;

		private int m_iDamageIntervalTimeMax = 10000;

		private int m_iDamageStatisticsTimeMax = 30000;

		private Dictionary<uint, DAMAGE_ACTOR_INFO> m_listDamageActorValue = new Dictionary<uint, DAMAGE_ACTOR_INFO>();

		private ulong m_ulHeroDeadTime;

		private uint[] m_arrHurtValue = new uint[4];

		public void Init(PoolObjHandle<ActorRoot> actorRoot)
		{
			this.m_actorHero = actorRoot;
			this.m_iDamageIntervalTimeMax = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_DEADINFO_ATTACK_INTERVALTIME_MAX);
			this.m_iDamageStatisticsTimeMax = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_DEADINFO_STATTIME_MAX);
		}

		public void UnInit()
		{
			if (this.m_actorHero)
			{
				this.m_actorHero.Release();
			}
			if (this.m_listDamageActorValue != null)
			{
				this.m_listDamageActorValue.Clear();
			}
		}

		public uint GetHostHeroObjId()
		{
			uint result = 0u;
			if (this.m_actorHero)
			{
				result = this.m_actorHero.handle.ObjID;
			}
			return result;
		}

		private void AddDamageValue(ref HurtEventResultInfo prm)
		{
			PoolObjHandle<ActorRoot> atker = prm.atker;
			SkillSlotType atkSlot = prm.hurtInfo.atkSlot;
			int hurtTotal = prm.hurtTotal;
			HurtTypeDef hurtType = prm.hurtInfo.hurtType;
			SKILL_USE_FROM_TYPE skillUseFrom = prm.hurtInfo.SkillUseFrom;
			if (this.m_listDamageActorValue != null && atkSlot <= SkillSlotType.SLOT_SKILL_VALID && atker)
			{
				uint objID = atker.handle.ObjID;
				this.DeleteTimeoutDamageValue(objID, 0uL);
				DAMAGE_ACTOR_INFO dAMAGE_ACTOR_INFO;
				if (!this.m_listDamageActorValue.TryGetValue(objID, ref dAMAGE_ACTOR_INFO))
				{
					dAMAGE_ACTOR_INFO = default(DAMAGE_ACTOR_INFO);
					dAMAGE_ACTOR_INFO.actorType = atker.handle.TheActorMeta.ActorType;
					dAMAGE_ACTOR_INFO.actorName = atker.handle.name;
					dAMAGE_ACTOR_INFO.ConfigId = atker.handle.TheActorMeta.ConfigId;
					if (dAMAGE_ACTOR_INFO.actorType == ActorTypeDef.Actor_Type_Monster)
					{
						MonsterWrapper monsterWrapper = atker.handle.AsMonster();
						dAMAGE_ACTOR_INFO.bMonsterType = monsterWrapper.GetActorSubType();
						dAMAGE_ACTOR_INFO.actorSubType = monsterWrapper.GetActorSubSoliderType();
					}
					Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(atker.handle.TheActorMeta.PlayerId);
					if (player != null)
					{
						dAMAGE_ACTOR_INFO.playerName = player.Name;
					}
					dAMAGE_ACTOR_INFO.listDamage = new SortedList<ulong, SKILL_SLOT_HURT_INFO[]>();
					this.m_listDamageActorValue.Add(objID, dAMAGE_ACTOR_INFO);
				}
				ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
				SKILL_SLOT_HURT_INFO[] array;
				if (!dAMAGE_ACTOR_INFO.listDamage.TryGetValue(logicFrameTick, ref array))
				{
					array = new SKILL_SLOT_HURT_INFO[12];
					dAMAGE_ACTOR_INFO.listDamage.Add(logicFrameTick, array);
				}
				if (array[(int)atkSlot].listHurtInfo == null)
				{
					array[(int)atkSlot].listHurtInfo = new List<HURT_INFO>();
				}
				if (array[(int)atkSlot].listHurtInfo != null)
				{
					string text = null;
					string strName = null;
					if (skillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL)
					{
						SkillSlot skillSlot;
						if (atkSlot < SkillSlotType.SLOT_SKILL_COUNT && atker.handle.SkillControl != null && (atker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero || atkSlot != SkillSlotType.SLOT_SKILL_0) && atker.handle.SkillControl.TryGetSkillSlot(atkSlot, out skillSlot) && skillSlot.SkillObj != null && skillSlot.SkillObj.cfgData != null)
						{
							if (!string.IsNullOrEmpty(skillSlot.SkillObj.IconName))
							{
								text = skillSlot.SkillObj.IconName;
							}
							if (!string.IsNullOrEmpty(skillSlot.SkillObj.cfgData.szSkillName))
							{
								strName = skillSlot.SkillObj.cfgData.szSkillName;
							}
						}
					}
					else if (skillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_EQUIP)
					{
						uint uiFromId = prm.hurtInfo.uiFromId;
						ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey(uiFromId);
						if (dataByKey != null)
						{
							text = dataByKey.szIcon;
							strName = dataByKey.szName;
						}
					}
					else if (skillUseFrom == SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_AREATRIGGER)
					{
						uint uiFromId2 = prm.hurtInfo.uiFromId;
						ResSkillCombineCfgInfo dataByKey2 = GameDataMgr.skillCombineDatabin.GetDataByKey(uiFromId2);
						if (dataByKey2 != null)
						{
							text = dataByKey2.szIconPath;
							strName = dataByKey2.szSkillCombineName;
						}
					}
					int count = array[(int)atkSlot].listHurtInfo.get_Count();
					int i;
					for (i = 0; i < count; i++)
					{
						if (array[(int)atkSlot].listHurtInfo.get_Item(i).strIconName == text)
						{
							HURT_INFO hURT_INFO;
							array[(int)atkSlot].listHurtInfo.get_Item(i).iValue = hURT_INFO.iValue + hurtTotal;
							break;
						}
					}
					if (i >= count)
					{
						HURT_INFO hURT_INFO2 = new HURT_INFO(hurtType, text, strName, hurtTotal, skillUseFrom);
						array[(int)atkSlot].listHurtInfo.Add(hURT_INFO2);
					}
				}
				dAMAGE_ACTOR_INFO.listDamage.set_Item(logicFrameTick, array);
				this.m_listDamageActorValue.set_Item(objID, dAMAGE_ACTOR_INFO);
			}
		}

		private void DeleteTimeoutDamageValue(uint uiObjId, ulong ulTime = 0uL)
		{
			if (this.m_listDamageActorValue != null)
			{
				ulong num;
				if (ulTime == 0uL)
				{
					num = Singleton<FrameSynchr>.instance.LogicFrameTick;
				}
				else
				{
					num = ulTime;
				}
				DAMAGE_ACTOR_INFO dAMAGE_ACTOR_INFO;
				if (this.m_listDamageActorValue.TryGetValue(uiObjId, ref dAMAGE_ACTOR_INFO))
				{
					int count = dAMAGE_ACTOR_INFO.listDamage.get_Count();
					if (count > 0)
					{
						ulong num2 = dAMAGE_ACTOR_INFO.listDamage.get_Keys().get_Item(count - 1);
						if (num - num2 > (ulong)((long)this.m_iDamageIntervalTimeMax))
						{
							dAMAGE_ACTOR_INFO.listDamage.Clear();
						}
						else
						{
							IEnumerator<KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]>> enumerator = dAMAGE_ACTOR_INFO.listDamage.GetEnumerator();
							while (enumerator.MoveNext())
							{
								KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]> current = enumerator.get_Current();
								ulong key = current.get_Key();
								if (num - key <= (ulong)((long)this.m_iDamageStatisticsTimeMax))
								{
									break;
								}
								dAMAGE_ACTOR_INFO.listDamage.Remove(key);
							}
						}
						this.m_listDamageActorValue.set_Item(uiObjId, dAMAGE_ACTOR_INFO);
					}
				}
			}
		}

		private void DeleteDamageValue(uint uiObjId)
		{
			if (this.m_listDamageActorValue != null && this.m_listDamageActorValue.ContainsKey(uiObjId))
			{
				this.m_listDamageActorValue.Remove(uiObjId);
			}
		}

		public void OnActorDamage(ref HurtEventResultInfo prm)
		{
			if (prm.src == this.m_actorHero && prm.hurtInfo.hurtType != HurtTypeDef.Therapic && prm.atker)
			{
				this.AddDamageValue(ref prm);
			}
		}

		public void OnActorDead(ref GameDeadEventParam prm)
		{
			if (prm.src == this.m_actorHero)
			{
				if (prm.orignalAtker && prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.m_actorKiller = prm.orignalAtker;
				}
				else if (prm.src.handle.ActorControl.IsKilledByHero())
				{
					this.m_actorKiller = prm.src.handle.ActorControl.LastHeroAtker;
				}
				else
				{
					this.m_actorKiller = prm.atker;
				}
				this.m_ulHeroDeadTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
			}
		}

		public void OnActorRevive(ref DefaultGameEventParam prm)
		{
			if (prm.src == this.m_actorHero)
			{
				if (this.m_listDamageActorValue != null)
				{
					this.m_listDamageActorValue.Clear();
				}
				if (this.m_actorKiller)
				{
					this.m_actorKiller.Release();
				}
				this.m_ulHeroDeadTime = 0uL;
			}
		}

		public bool GetActorDamage(uint uiObjId, ref HURT_INFO[] arrDamageInfo, ref int iActorTotalDamageValue)
		{
			iActorTotalDamageValue = 0;
			DAMAGE_ACTOR_INFO dAMAGE_ACTOR_INFO;
			if (this.m_listDamageActorValue == null || this.m_listDamageActorValue.get_Count() <= 0 || arrDamageInfo == null || !this.m_listDamageActorValue.TryGetValue(uiObjId, ref dAMAGE_ACTOR_INFO))
			{
				return false;
			}
			List<HURT_INFO> list = new List<HURT_INFO>();
			int count = dAMAGE_ACTOR_INFO.listDamage.get_Count();
			if (count <= 0)
			{
				return false;
			}
			if (this.m_ulHeroDeadTime - dAMAGE_ACTOR_INFO.listDamage.get_Keys().get_Item(count - 1) > (ulong)((long)this.m_iDamageIntervalTimeMax))
			{
				return false;
			}
			IEnumerator<KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]>> enumerator = dAMAGE_ACTOR_INFO.listDamage.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ulong ulHeroDeadTime = this.m_ulHeroDeadTime;
				KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]> current = enumerator.get_Current();
				if (ulHeroDeadTime - current.get_Key() <= (ulong)((long)this.m_iDamageStatisticsTimeMax))
				{
					KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]> current2 = enumerator.get_Current();
					SKILL_SLOT_HURT_INFO[] value = current2.get_Value();
					if (value != null)
					{
						for (int i = 0; i <= 11; i++)
						{
							if (value[i].listHurtInfo != null)
							{
								int count2 = value[i].listHurtInfo.get_Count();
								for (int j = 0; j < count2; j++)
								{
									int count3 = list.get_Count();
									int k;
									for (k = 0; k < count3; k++)
									{
										if (list.get_Item(k).strIconName == value[i].listHurtInfo.get_Item(j).strIconName && list.get_Item(k).strName == value[i].listHurtInfo.get_Item(j).strName)
										{
											HURT_INFO hURT_INFO = list.get_Item(k);
											hURT_INFO.iValue += value[i].listHurtInfo.get_Item(j).iValue;
											list.set_Item(k, hURT_INFO);
											break;
										}
									}
									if (k >= count3)
									{
										list.Add(value[i].listHurtInfo.get_Item(j));
									}
									iActorTotalDamageValue += value[i].listHurtInfo.get_Item(j).iValue;
								}
							}
						}
					}
				}
			}
			list.Sort(delegate(HURT_INFO a, HURT_INFO b)
			{
				if (a.iValue < b.iValue)
				{
					return 1;
				}
				if (a.iValue > b.iValue)
				{
					return -1;
				}
				return 0;
			});
			int count4 = list.get_Count();
			int num = arrDamageInfo.Length;
			int num2 = 0;
			while (num2 < count4 && num2 < num)
			{
				arrDamageInfo[num2] = list.get_Item(num2);
				num2++;
			}
			return true;
		}

		public int GetAllActorsTotalDamageAndTopActorId(ref uint[] arrObjId, int iTopCount, ref uint uiAllTotalDamage, ref uint[] arrDiffTypeHurtValue)
		{
			int num = 0;
			uiAllTotalDamage = 0u;
			if (this.m_listDamageActorValue != null && this.m_listDamageActorValue.get_Count() > 0)
			{
				int num2 = 0;
				DOUBLE_INT_INFO[] array = new DOUBLE_INT_INFO[this.m_listDamageActorValue.get_Count()];
				Dictionary<uint, DAMAGE_ACTOR_INFO>.Enumerator enumerator = this.m_listDamageActorValue.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int num3 = 0;
					KeyValuePair<uint, DAMAGE_ACTOR_INFO> current = enumerator.get_Current();
					int count = current.get_Value().listDamage.get_Count();
					if (count > 0)
					{
						ulong ulHeroDeadTime = this.m_ulHeroDeadTime;
						KeyValuePair<uint, DAMAGE_ACTOR_INFO> current2 = enumerator.get_Current();
						if (ulHeroDeadTime - current2.get_Value().listDamage.get_Keys().get_Item(count - 1) <= (ulong)((long)this.m_iDamageIntervalTimeMax))
						{
							KeyValuePair<uint, DAMAGE_ACTOR_INFO> current3 = enumerator.get_Current();
							IEnumerator<KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]>> enumerator2 = current3.get_Value().listDamage.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								ulong ulHeroDeadTime2 = this.m_ulHeroDeadTime;
								KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]> current4 = enumerator2.get_Current();
								if (ulHeroDeadTime2 - current4.get_Key() <= (ulong)((long)this.m_iDamageStatisticsTimeMax))
								{
									KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]> current5 = enumerator2.get_Current();
									SKILL_SLOT_HURT_INFO[] value = current5.get_Value();
									if (value != null)
									{
										for (int i = 0; i <= 11; i++)
										{
											if (value[i].listHurtInfo != null)
											{
												int count2 = value[i].listHurtInfo.get_Count();
												for (int j = 0; j < count2; j++)
												{
													num3 += value[i].listHurtInfo.get_Item(j).iValue;
													if (value[i].listHurtInfo.get_Item(j).hurtType >= HurtTypeDef.PhysHurt && value[i].listHurtInfo.get_Item(j).hurtType < HurtTypeDef.Max)
													{
														arrDiffTypeHurtValue[(int)value[i].listHurtInfo.get_Item(j).hurtType] += (uint)value[i].listHurtInfo.get_Item(j).iValue;
													}
												}
											}
										}
									}
								}
							}
							if (num3 > 0)
							{
								uiAllTotalDamage += (uint)num3;
								if (this.m_actorKiller)
								{
									uint objID = this.m_actorKiller.handle.ObjID;
									KeyValuePair<uint, DAMAGE_ACTOR_INFO> current6 = enumerator.get_Current();
									if (objID != current6.get_Key())
									{
										DOUBLE_INT_INFO[] array2 = array;
										int num4 = num2;
										KeyValuePair<uint, DAMAGE_ACTOR_INFO> current7 = enumerator.get_Current();
										array2[num4].iKey = (int)current7.get_Key();
										array[num2].iValue = num3;
										num2++;
									}
								}
							}
						}
					}
				}
				Array.Sort<DOUBLE_INT_INFO>(array, (DOUBLE_INT_INFO p1, DOUBLE_INT_INFO p2) => p2.iValue.CompareTo(p1.iValue));
				int num5 = 0;
				while (num5 < iTopCount && num5 < num2)
				{
					if (!this.m_actorKiller || (long)array[num5].iKey != (long)((ulong)this.m_actorKiller.handle.ObjID))
					{
						if (array[num5].iValue <= 0)
						{
							break;
						}
						arrObjId[num5] = (uint)array[num5].iKey;
						num++;
					}
					num5++;
				}
			}
			return num;
		}

		public HurtTypeDef GetSkillSlotHurtType(uint uiObjId, SkillSlotType slotType)
		{
			DAMAGE_ACTOR_INFO dAMAGE_ACTOR_INFO;
			if (slotType >= SkillSlotType.SLOT_SKILL_0 && slotType <= SkillSlotType.SLOT_SKILL_VALID && this.m_listDamageActorValue != null && this.m_listDamageActorValue.get_Count() > 0 && this.m_listDamageActorValue.TryGetValue(uiObjId, ref dAMAGE_ACTOR_INFO))
			{
				IEnumerator<KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]>> enumerator = dAMAGE_ACTOR_INFO.listDamage.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<ulong, SKILL_SLOT_HURT_INFO[]> current = enumerator.get_Current();
					SKILL_SLOT_HURT_INFO[] value = current.get_Value();
					if (value != null && value[(int)slotType].listHurtInfo.get_Item(0).iValue != 0)
					{
						return value[(int)slotType].listHurtInfo.get_Item(0).hurtType;
					}
				}
			}
			return HurtTypeDef.Max;
		}

		public bool GetKillerObjId(ref uint uiObjId, ref ActorTypeDef actorType)
		{
			if (this.m_actorKiller)
			{
				uiObjId = this.m_actorKiller.handle.ObjID;
				actorType = this.m_actorKiller.handle.TheActorMeta.ActorType;
				return true;
			}
			return false;
		}

		public ulong GetHostHeroDeadTime()
		{
			return this.m_ulHeroDeadTime;
		}

		public bool GetDamageActorInfo(uint uiObjId, ref string actorName, ref string playerName, ref ActorTypeDef actorType, ref int iConfigId, ref byte actorSubType, ref byte bMonsterType)
		{
			DAMAGE_ACTOR_INFO dAMAGE_ACTOR_INFO;
			if (this.m_listDamageActorValue != null && this.m_listDamageActorValue.get_Count() > 0 && this.m_listDamageActorValue.TryGetValue(uiObjId, ref dAMAGE_ACTOR_INFO))
			{
				actorType = dAMAGE_ACTOR_INFO.actorType;
				actorName = dAMAGE_ACTOR_INFO.actorName;
				playerName = dAMAGE_ACTOR_INFO.playerName;
				iConfigId = dAMAGE_ACTOR_INFO.ConfigId;
				actorSubType = dAMAGE_ACTOR_INFO.actorSubType;
				bMonsterType = dAMAGE_ACTOR_INFO.bMonsterType;
				return true;
			}
			return false;
		}
	}
}
