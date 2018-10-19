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
            string str = !bSrcAllies ? "Enemy_" : "Self_";
            if (Type != KillDetailInfoType.Info_Type_Soldier_Boosted)
            {
                if ((actorType == ActorTypeDef.Actor_Type_Monster) || (actorType == ActorTypeDef.Actor_Type_Organ))
                {
                    if (Type == KillDetailInfoType.Info_Type_DestroyTower)
                    {
                        return (str + "TowerDie");
                    }
                    return "Executed";
                }
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
                        if (!bSelfKillORKilled)
                        {
                            return "Self_OneKill";
                        }
                        return "Self_YouKill";

                    case KillDetailInfoType.Info_Type_DoubleKill:
                        return (str + "DoubleKill");

                    case KillDetailInfoType.Info_Type_TripleKill:
                        return (str + "TripleKill");

                    case KillDetailInfoType.Info_Type_QuataryKill:
                        return (str + "QuadraKill");

                    case KillDetailInfoType.Info_Type_PentaKill:
                        return (str + "PentaKill");

                    case KillDetailInfoType.Info_Type_MonsterKill:
                        return (str + "KillingSpree1");

                    case KillDetailInfoType.Info_Type_DominateBattle:
                        return (str + "KillingSpree2");

                    case KillDetailInfoType.Info_Type_Legendary:
                        return (str + "KillingSpree3");

                    case KillDetailInfoType.Info_Type_TotalAnnihilat:
                        return (str + "KillingSpree4");

                    case KillDetailInfoType.Info_Type_Odyssey:
                        return (str + "KillingSpree5");

                    case KillDetailInfoType.Info_Type_DestroyTower:
                        return (str + "TowerDie");

                    case KillDetailInfoType.Info_Type_Game_Start_Wel:
                        return "Play_5V5_sys_1_01";

                    case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3:
                        return "Play_5V5_sys_2";

                    case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5:
                        return "Play_5V5_sys_3";

                    case KillDetailInfoType.Info_Type_Soldier_Activate:
                        return "Play_5V5_war_1";

                    case KillDetailInfoType.Info_Type_Kill_3V3_Dragon:
                        return (str + "BaoJunSkill");

                    case KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon:
                        return (str + "BaoJunSkill");

                    case KillDetailInfoType.Info_Type_Kill_5V5_BigDragon:
                        return (str + "BaoJunSkill");

                    case KillDetailInfoType.Info_Type_AllDead:
                        return "Common_Ace";

                    case KillDetailInfoType.Info_Type_StopMultiKill:
                        return "ShutDown";

                    case KillDetailInfoType.Info_Type_5V5SmallDragon_Suicide:
                        return "Self_BaoJunSkill";
                }
            }
            return string.Empty;
        }


		public static List<string> GetAllAnimations()
		{
			List<string> list = new List<string>();
			Array values = Enum.GetValues(typeof(KillDetailInfoType));
			for (int i = 0; i < values.Length; i++)
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
            string str = !bSrc ? "_B" : "_A";
            string str2 = string.Empty;
            bool flag = false;
            KillDetailInfoType type = Type;
            switch (type)
            {
                case KillDetailInfoType.Info_Type_First_Kill:
                    str2 = "FirstBlood";
                    break;

                case KillDetailInfoType.Info_Type_Kill:
                    str2 = "NormalKill";
                    break;

                case KillDetailInfoType.Info_Type_DoubleKill:
                    str2 = "DoubleKill";
                    break;

                case KillDetailInfoType.Info_Type_TripleKill:
                    str2 = "TrebleKill";
                    break;

                case KillDetailInfoType.Info_Type_QuataryKill:
                    str2 = "QuataryKill";
                    break;

                case KillDetailInfoType.Info_Type_PentaKill:
                    str2 = "PentaKill";
                    break;

                case KillDetailInfoType.Info_Type_MonsterKill:
                    str2 = "DaShaTeSha";
                    break;

                case KillDetailInfoType.Info_Type_DominateBattle:
                    str2 = "ShaRenRuMa";
                    break;

                case KillDetailInfoType.Info_Type_Legendary:
                    str2 = "WuRenNenDang";
                    break;

                case KillDetailInfoType.Info_Type_TotalAnnihilat:
                    str2 = "HengSaoQianJun";
                    break;

                case KillDetailInfoType.Info_Type_Odyssey:
                    str2 = "TianXiaWuShuang";
                    break;

                case KillDetailInfoType.Info_Type_DestroyTower:
                    str2 = "BreakTower";
                    break;

                default:
                    switch (type)
                    {
                        case KillDetailInfoType.Info_Type_Cannon_Spawned:
                            str2 = "GongChengCheJiaRu";
                            break;

                        case KillDetailInfoType.Info_Type_Soldier_Boosted:
                            str2 = "XiaoBingZengQiang";
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
                            str2 = "ZhuZaiChuXian";
                            break;

                        case KillDetailInfoType.Info_Type_Destroy_All_Tower:
                            str2 = "TowerAllDead";
                            break;

                        default:
                            switch (type)
                            {
                                case KillDetailInfoType.Info_Type_Kill_3V3_Dragon:
                                    str2 = "DragonKill";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon:
                                    str2 = "KillJuLong";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_Kill_5V5_BigDragon:
                                    str2 = "KillZhuZai";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_RunningMan:
                                    str2 = "ExitGame";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_Reconnect:
                                    str2 = "ChongLian";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_Disconnect:
                                    str2 = "ExitGame";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_FireHole_first:
                                    str2 = "YouShi";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_FireHole_second:
                                    str2 = "JiJiangShengLi";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_FireHole_third:
                                    str2 = "ShengLi";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_5V5SmallDragon_Suicide:
                                    return "BaoJunDisappear";

                                case KillDetailInfoType.Info_Type_5V5SmallDragon_Enter:
                                    return "TaiGuBaoJunAppear";

                                case KillDetailInfoType.Info_Type_AllDead:
                                    str2 = "Ace";
                                    goto Label_027E;

                                case KillDetailInfoType.Info_Type_StopMultiKill:
                                    str2 = "EndKill";
                                    goto Label_027E;
                            }
                            flag = true;
                            break;
                    }
                    break;
            }
        Label_027E:
            if (flag)
            {
                return string.Empty;
            }
            return (str2 + str);
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
			result.actorType = ((!killer) ? ActorTypeDef.Invalid : killer.handle.TheActorMeta.ActorType);
			result.bSrcAllies = detail.bSelfCamp;
			result.bSuicide = detail.bSuicide;
			result.assistImgSrc = new string[4];
			result.killerObjID = ((!killer) ? 0u : killer.handle.ObjID);
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
				for (int i = 0; i < detail.assistList.Count; i++)
				{
					uint num = detail.assistList[i];
					for (int j = 0; j < Singleton<GameObjMgr>.instance.HeroActors.Count; j++)
					{
						if (num == Singleton<GameObjMgr>.instance.HeroActors[j].handle.ObjID)
						{
							result.assistImgSrc[i] = KillNotifyUT.GetHero_Icon(Singleton<GameObjMgr>.instance.HeroActors[j], false);
						}
					}
				}
			}
			if (detail.Type == KillDetailInfoType.Info_Type_Soldier_Boosted)
			{
				result.KillerImgSrc = ((!detail.bSelfCamp) ? KillNotify.red_soldier_icon : KillNotify.blue_soldier_icon);
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
			return string.Format("{0}{1}", (!bSmall) ? CUIUtility.s_Sprite_Dynamic_BustCircle_Dir : CUIUtility.s_Sprite_Dynamic_BustCircleSmall_Dir, heroSkinPic);
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
