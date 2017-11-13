using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class KillNotifyUT
	{
		public static string GetSoundEvent(KillDetailInfoType Type, bool bSrcAllies, bool bSelfKillORKilled, ActorTypeDef actorType)
		{
			string text = bSrcAllies ? "Self_" : "Enemy_";
			if (Type != KillDetailInfoType.Info_Type_Soldier_Boosted)
			{
				if (actorType != ActorTypeDef.Actor_Type_Monster && actorType != ActorTypeDef.Actor_Type_Organ)
				{
					switch (Type)
					{
					case KillDetailInfoType.Info_Type_First_Kill:
						return "First_Blood";
					case KillDetailInfoType.Info_Type_Kill:
						if (!bSrcAllies)
						{
							if (bSelfKillORKilled)
							{
								return "Self_OneDie";
							}
							return "Self_TeamDie";
						}
						else
						{
							if (!bSelfKillORKilled)
							{
								return "Self_OneKill";
							}
							return "Self_YouKill";
						}
						break;
					case KillDetailInfoType.Info_Type_DoubleKill:
						return text + "DoubleKill";
					case KillDetailInfoType.Info_Type_TripleKill:
						return text + "TripleKill";
					case KillDetailInfoType.Info_Type_QuataryKill:
						return text + "QuadraKill";
					case KillDetailInfoType.Info_Type_PentaKill:
						return text + "PentaKill";
					case (KillDetailInfoType)7:
					case (KillDetailInfoType)8:
					case (KillDetailInfoType)9:
					case (KillDetailInfoType)10:
					case (KillDetailInfoType)16:
					case (KillDetailInfoType)17:
					case (KillDetailInfoType)18:
					case (KillDetailInfoType)19:
					case (KillDetailInfoType)20:
						IL_A7:
						switch (Type)
						{
						case KillDetailInfoType.Info_Type_Game_Start_Wel:
							return "Play_5V5_sys_1_01";
						case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3:
							return "Play_5V5_sys_2";
						case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5:
							return "Play_5V5_sys_3";
						case KillDetailInfoType.Info_Type_Soldier_Activate:
							return "Play_5V5_war_1";
						default:
							switch (Type)
							{
							case KillDetailInfoType.Info_Type_Kill_3V3_Dragon:
								return text + "BaoJunSkill";
							case KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon:
								return text + "BaoJunSkill";
							case KillDetailInfoType.Info_Type_Kill_5V5_BigDragon:
								return text + "BaoJunSkill";
							default:
								if (Type == KillDetailInfoType.Info_Type_AllDead)
								{
									return "Common_Ace";
								}
								if (Type == KillDetailInfoType.Info_Type_StopMultiKill)
								{
									return "ShutDown";
								}
								if (Type != KillDetailInfoType.Info_Type_5V5SmallDragon_Suicide)
								{
									goto IL_1F4;
								}
								return "Self_BaoJunSkill";
							}
							break;
						}
						break;
					case KillDetailInfoType.Info_Type_MonsterKill:
						return text + "KillingSpree1";
					case KillDetailInfoType.Info_Type_DominateBattle:
						return text + "KillingSpree2";
					case KillDetailInfoType.Info_Type_Legendary:
						return text + "KillingSpree3";
					case KillDetailInfoType.Info_Type_TotalAnnihilat:
						return text + "KillingSpree4";
					case KillDetailInfoType.Info_Type_Odyssey:
						return text + "KillingSpree5";
					case KillDetailInfoType.Info_Type_DestroyTower:
						return text + "TowerDie";
					}
					goto IL_A7;
				}
				if (Type == KillDetailInfoType.Info_Type_DestroyTower)
				{
					return text + "TowerDie";
				}
				return "Executed";
			}
			IL_1F4:
			return string.Empty;
		}

		public static List<string> GetAllAnimations()
		{
			List<string> list = new List<string>();
			Array values = Enum.GetValues(typeof(KillDetailInfoType));
			for (int i = 0; i < values.get_Length(); i++)
			{
				KillDetailInfoType type = (KillDetailInfoType)((int)values.GetValue(i));
				string animation = KillNotifyUT.GetAnimation(type, true);
				if (!string.IsNullOrEmpty(animation))
				{
					list.Add(animation);
				}
				animation = KillNotifyUT.GetAnimation(type, false);
				if (!string.IsNullOrEmpty(animation))
				{
					list.Add(animation);
				}
			}
			return list;
		}

		public static string GetAnimation(KillDetailInfoType Type, bool bSrc)
		{
			string text = bSrc ? "_A" : "_B";
			string text2 = string.Empty;
			bool flag = false;
			switch (Type)
			{
			case KillDetailInfoType.Info_Type_First_Kill:
				text2 = "FirstBlood";
				goto IL_293;
			case KillDetailInfoType.Info_Type_Kill:
				text2 = "NormalKill";
				goto IL_293;
			case KillDetailInfoType.Info_Type_DoubleKill:
				text2 = "DoubleKill";
				goto IL_293;
			case KillDetailInfoType.Info_Type_TripleKill:
				text2 = "TrebleKill";
				goto IL_293;
			case KillDetailInfoType.Info_Type_QuataryKill:
				text2 = "QuataryKill";
				goto IL_293;
			case KillDetailInfoType.Info_Type_PentaKill:
				text2 = "PentaKill";
				goto IL_293;
			case KillDetailInfoType.Info_Type_MonsterKill:
				text2 = "DaShaTeSha";
				goto IL_293;
			case KillDetailInfoType.Info_Type_DominateBattle:
				text2 = "ShaRenRuMa";
				goto IL_293;
			case KillDetailInfoType.Info_Type_Legendary:
				text2 = "WuRenNenDang";
				goto IL_293;
			case KillDetailInfoType.Info_Type_TotalAnnihilat:
				text2 = "HengSaoQianJun";
				goto IL_293;
			case KillDetailInfoType.Info_Type_Odyssey:
				text2 = "TianXiaWuShuang";
				goto IL_293;
			case KillDetailInfoType.Info_Type_DestroyTower:
				text2 = "BreakTower";
				goto IL_293;
			}
			switch (Type)
			{
			case KillDetailInfoType.Info_Type_Cannon_Spawned:
				text2 = "GongChengCheJiaRu";
				break;
			case KillDetailInfoType.Info_Type_Soldier_Boosted:
				text2 = "XiaoBingZengQiang";
				break;
			case KillDetailInfoType.Info_Type_Game_Start_Wel:
				return "Welcome";
			case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3:
				return "ThreeSecond";
			case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5:
				return "FiveSecond";
			case KillDetailInfoType.Info_Type_Soldier_Activate:
				return "Battle";
			case KillDetailInfoType.Info_Type_Soldier_BigDragon:
				text2 = "ZhuZaiChuXian";
				break;
			case KillDetailInfoType.Info_Type_Destroy_All_Tower:
				text2 = "TowerAllDead";
				break;
			default:
				switch (Type)
				{
				case KillDetailInfoType.Info_Type_Kill_3V3_Dragon:
					text2 = "DragonKill";
					break;
				case KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon:
					text2 = "KillJuLong";
					break;
				case KillDetailInfoType.Info_Type_Kill_5V5_BigDragon:
					text2 = "KillZhuZai";
					break;
				default:
					switch (Type)
					{
					case KillDetailInfoType.Info_Type_RunningMan:
						text2 = "ExitGame";
						break;
					case KillDetailInfoType.Info_Type_Reconnect:
						text2 = "ChongLian";
						break;
					case KillDetailInfoType.Info_Type_Disconnect:
						text2 = "ExitGame";
						break;
					default:
						switch (Type)
						{
						case KillDetailInfoType.Info_Type_FireHole_first:
							text2 = "YouShi";
							break;
						case KillDetailInfoType.Info_Type_FireHole_second:
							text2 = "JiJiangShengLi";
							break;
						case KillDetailInfoType.Info_Type_FireHole_third:
							text2 = "ShengLi";
							break;
						default:
							if (Type == KillDetailInfoType.Info_Type_5V5SmallDragon_Suicide)
							{
								return "BaoJunDisappear";
							}
							if (Type == KillDetailInfoType.Info_Type_5V5SmallDragon_Enter)
							{
								return "TaiGuBaoJunAppear";
							}
							if (Type != KillDetailInfoType.Info_Type_AllDead)
							{
								if (Type != KillDetailInfoType.Info_Type_StopMultiKill)
								{
									flag = true;
								}
								else
								{
									text2 = "EndKill";
								}
							}
							else
							{
								text2 = "Ace";
							}
							break;
						}
						break;
					}
					break;
				}
				break;
			}
			IL_293:
			if (flag)
			{
				return string.Empty;
			}
			return text2 + text;
		}

		public static void SetImageSprite(Image img, string spt)
		{
			if (img != null && !string.IsNullOrEmpty(spt))
			{
				img.SetSprite(spt, Singleton<CBattleSystem>.GetInstance().FormScript, true, true, false, false);
			}
		}

		public static KillInfo Convert_DetailInfo_KillInfo(KillDetailInfo detail)
		{
			KillDetailInfoType killDetailInfoType = KillDetailInfoType.Info_Type_None;
			PoolObjHandle<ActorRoot> killer = detail.Killer;
			PoolObjHandle<ActorRoot> victim = detail.Victim;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (killer)
			{
				flag = (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ);
				flag2 = (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster);
				flag3 = (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero);
			}
			KillInfo result = default(KillInfo);
			result.KillerImgSrc = (result.VictimImgSrc = string.Empty);
			result.MsgType = detail.Type;
			result.bPlayerSelf_KillOrKilled = detail.bPlayerSelf_KillOrKilled;
			result.actorType = (killer ? killer.handle.TheActorMeta.ActorType : ActorTypeDef.Invalid);
			result.bSrcAllies = detail.bSelfCamp;
			result.bSuicide = detail.bSuicide;
			result.assistImgSrc = new string[4];
			result.killerObjID = (killer ? killer.handle.ObjID : 0u);
			if (flag2)
			{
				result.KillerImgSrc = KillNotify.monster_icon;
			}
			if (flag)
			{
				if (killer.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
				{
					result.KillerImgSrc = KillNotify.base_icon;
				}
				else
				{
					result.KillerImgSrc = KillNotify.building_icon;
				}
			}
			if (flag3)
			{
				result.KillerImgSrc = KillNotifyUT.GetHero_Icon(detail.Killer, false);
			}
			if (killer)
			{
				if (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					if (killer.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
					{
						result.KillerImgSrc = KillNotify.base_icon;
					}
					else
					{
						result.KillerImgSrc = KillNotify.building_icon;
					}
				}
				if (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					MonsterWrapper monsterWrapper = killer.handle.AsMonster();
					if (killer.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId)
					{
						result.KillerImgSrc = KillNotify.dragon_icon;
					}
					else if (monsterWrapper.cfgInfo != null && monsterWrapper.cfgInfo.bMonsterType == 2)
					{
						result.KillerImgSrc = KillNotify.yeguai_icon;
					}
					else
					{
						result.KillerImgSrc = KillNotify.monster_icon;
					}
				}
				if (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					result.KillerImgSrc = KillNotifyUT.GetHero_Icon(killer, false);
				}
			}
			if (victim)
			{
				if (victim.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					result.VictimImgSrc = KillNotifyUT.GetHero_Icon(victim, false);
				}
				if (victim.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					result.VictimImgSrc = KillNotify.building_icon;
				}
				if (victim.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && victim.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId)
				{
					result.VictimImgSrc = KillNotify.dragon_icon;
				}
			}
			if (detail.assistList != null && result.assistImgSrc != null)
			{
				for (int i = 0; i < detail.assistList.get_Count(); i++)
				{
					uint num = detail.assistList.get_Item(i);
					for (int j = 0; j < Singleton<GameObjMgr>.instance.HeroActors.get_Count(); j++)
					{
						if (num == Singleton<GameObjMgr>.instance.HeroActors.get_Item(j).handle.ObjID)
						{
							result.assistImgSrc[i] = KillNotifyUT.GetHero_Icon(Singleton<GameObjMgr>.instance.HeroActors.get_Item(j), false);
						}
					}
				}
			}
			if (detail.Type == KillDetailInfoType.Info_Type_Soldier_Boosted)
			{
				result.KillerImgSrc = (detail.bSelfCamp ? KillNotify.blue_soldier_icon : KillNotify.red_soldier_icon);
				return result;
			}
			if (detail.Type == KillDetailInfoType.Info_Type_Reconnect || detail.Type == KillDetailInfoType.Info_Type_RunningMan)
			{
				result.VictimImgSrc = string.Empty;
				return result;
			}
			if (detail.HeroMultiKillType != killDetailInfoType)
			{
				result.MsgType = detail.HeroMultiKillType;
				return result;
			}
			if (detail.Type == KillDetailInfoType.Info_Type_StopMultiKill)
			{
				return result;
			}
			if (detail.Type == KillDetailInfoType.Info_Type_First_Kill)
			{
				return result;
			}
			if (detail.Type == KillDetailInfoType.Info_Type_DestroyTower)
			{
				return result;
			}
			if (detail.Type == KillDetailInfoType.Info_Type_Kill_3V3_Dragon || detail.Type == KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon || detail.Type == KillDetailInfoType.Info_Type_Kill_5V5_BigDragon)
			{
				if (flag2)
				{
					result.KillerImgSrc = KillNotify.monster_icon;
				}
				if (flag)
				{
					result.KillerImgSrc = KillNotify.building_icon;
				}
				if (flag3)
				{
					result.KillerImgSrc = KillNotifyUT.GetHero_Icon(detail.Killer, false);
				}
				result.bSrcAllies = detail.bSelfCamp;
				return result;
			}
			if (detail.bAllDead)
			{
				result.MsgType = KillDetailInfoType.Info_Type_AllDead;
				return result;
			}
			if (detail.HeroContiKillType != killDetailInfoType)
			{
				result.MsgType = detail.HeroContiKillType;
				return result;
			}
			if (detail.Type == KillDetailInfoType.Info_Type_Kill)
			{
				return result;
			}
			return result;
		}

		public static string GetHero_Icon(ref ActorMeta actorMeta, bool bSmall = false)
		{
			return KillNotifyUT.GetHero_Icon((uint)actorMeta.ConfigId, 0u, bSmall);
		}

		public static string GetHero_Icon(PoolObjHandle<ActorRoot> actor, bool bSmall = false)
		{
			if (!actor)
			{
				return string.Empty;
			}
			return KillNotifyUT.GetHero_Icon(ref actor.handle.TheActorMeta, bSmall);
		}

		public static string GetHero_Icon(uint ConfigID, uint SkinID = 0u, bool bSmall = false)
		{
			string heroSkinPic = CSkinInfo.GetHeroSkinPic(ConfigID, SkinID);
			return string.Format("{0}{1}", bSmall ? CUIUtility.s_Sprite_Dynamic_BustCircleSmall_Dir : CUIUtility.s_Sprite_Dynamic_BustCircle_Dir, heroSkinPic);
		}

		public static string GetHero_Icon(ActorRoot actor, bool bSmall)
		{
			string result = string.Empty;
			if (actor != null)
			{
				result = KillNotifyUT.GetHero_Icon(ref actor.TheActorMeta, bSmall);
			}
			return result;
		}
	}
}
