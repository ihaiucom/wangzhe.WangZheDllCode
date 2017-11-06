using System;
using UnityEngine;

public class NewbieGuideSctiptFactory
{
	public static NewbieGuideBaseScript AddScript(NewbieGuideScriptType type, GameObject gameObject)
	{
		switch (type)
		{
		case NewbieGuideScriptType.clickTrialBtn:
			return gameObject.AddComponent<NewbieGuideClickTrial>();
		case NewbieGuideScriptType.clickZhuangziBtn:
			return gameObject.AddComponent<NewbieGuideClickZhuangzi>();
		case NewbieGuideScriptType.clickBurningBtn:
			return gameObject.AddComponent<NewbieGuideClickBurning>();
		case NewbieGuideScriptType.clickEliteBtn:
			return gameObject.AddComponent<NewbieGuideClickElitePvE>();
		case NewbieGuideScriptType.clickGuildBtn:
			return gameObject.AddComponent<NewbieGuideClickHallGuild>();
		case (NewbieGuideScriptType)57:
		case (NewbieGuideScriptType)58:
		case (NewbieGuideScriptType)59:
		case (NewbieGuideScriptType)60:
		case (NewbieGuideScriptType)61:
		case (NewbieGuideScriptType)62:
		case (NewbieGuideScriptType)63:
		case (NewbieGuideScriptType)64:
		case (NewbieGuideScriptType)66:
		case (NewbieGuideScriptType)67:
		case (NewbieGuideScriptType)68:
		case (NewbieGuideScriptType)69:
		case (NewbieGuideScriptType)70:
			IL_A5:
			switch (type)
			{
			case NewbieGuideScriptType.clickPvpHumanAi:
				return gameObject.AddComponent<NewbieGuideClickPvpHumanAi>();
			case NewbieGuideScriptType.clickPvpHumanAiSingle33:
				return gameObject.AddComponent<NewbieGuideClickPvpHumanAiSingle33>();
			case NewbieGuideScriptType.clickPvpHuman:
				return gameObject.AddComponent<NewbieGuideClickPvpHuman>();
			case NewbieGuideScriptType.clickPvpHumanSingle33:
				return gameObject.AddComponent<NewbieGuideClickPvpHumanSingle33>();
			case NewbieGuideScriptType.clickMatching:
				return gameObject.AddComponent<NewbieGuideClickMatching>();
			case NewbieGuideScriptType.clickPvpHumanAiSingle33Difficulty:
				return gameObject.AddComponent<NewbieGuideClickPvpHumanAiSingle33Difficulty>();
			case NewbieGuideScriptType.guide5v5Confirm:
				return gameObject.AddComponent<NewbieGuide5v5GuideConfirm>();
			case NewbieGuideScriptType.clickPvpHuman11:
				return gameObject.AddComponent<NewbieGuideClickPvPHuman11>();
			case NewbieGuideScriptType.clickTrain:
				return gameObject.AddComponent<NewbieGuideClickTrain>();
			case NewbieGuideScriptType.clickTrain55:
				return gameObject.AddComponent<NewbieGuideClickTrain55>();
			case NewbieGuideScriptType.clickTrainWheelDisc:
				return gameObject.AddComponent<NewbieGuideClickWheelDisc>();
			case NewbieGuideScriptType.guide3v3Confirm:
				return gameObject.AddComponent<NewbieGuide3v3Confirm>();
			case NewbieGuideScriptType.clickTrain33:
				return gameObject.AddComponent<NewbieGuideClickTrain33>();
			case NewbieGuideScriptType.oldPlayerGuide5v5Confirm:
				return gameObject.AddComponent<NewbieGuideOldPlayer55Confirm>();
			case NewbieGuideScriptType.clickTrainLevelBack:
				return gameObject.AddComponent<NewbieGuideTrainLevelClickBack>();
			case NewbieGuideScriptType.clickPvpHuman55:
				return gameObject.AddComponent<NewbieGuideClickPvPHuman55>();
			case NewbieGuideScriptType.clickMatchingConfirm:
				return gameObject.AddComponent<NewbieGuideClickMatchingConfirm>();
			case NewbieGuideScriptType.introfireMatch:
				return gameObject.AddComponent<NewbieGuideIntroFireMatch>();
			}
			switch (type)
			{
			case NewbieGuideScriptType.clickHallSymbolBtn:
				return gameObject.AddComponent<NewbieGuideClickHallSymbolBtn>();
			case NewbieGuideScriptType.clickSymbolSlot:
				return gameObject.AddComponent<NewbieGuideClickSymbolSlot>();
			case NewbieGuideScriptType.clickSymbol:
				return gameObject.AddComponent<NewbieGuideClickSymbol>();
			case NewbieGuideScriptType.clickPvEBackToHall:
				return gameObject.AddComponent<NewbieGuideClickAdvToLobbyScript>();
			case NewbieGuideScriptType.clickGetSymbol:
				return gameObject.AddComponent<NewbieGuideClickGetSymbol>();
			case NewbieGuideScriptType.clickSymbolRewardBack:
				return gameObject.AddComponent<NewbieGuideClickSymbolRewardBack>();
			case NewbieGuideScriptType.clickCloseSymbolIntro:
				return gameObject.AddComponent<NewbieGuideClickCloseSymbolIntro>();
			case NewbieGuideScriptType.clickSymbolManufacture:
				return gameObject.AddComponent<NewbieGuideClickSymbolManufacture>();
			case NewbieGuideScriptType.clickSymbolEquip:
				return gameObject.AddComponent<NewbieGuideClickSymbolEquip>();
			case NewbieGuideScriptType.clickSymbolSpecific:
				return gameObject.AddComponent<NewbieGuideClickSymbolSpecific>();
			case NewbieGuideScriptType.clickHeroSymbolPage:
				return gameObject.AddComponent<NewbieGuideClickHeroSymbolPage>();
			case NewbieGuideScriptType.pickSymbolManufacture:
				return gameObject.AddComponent<NewbieGuidePickSymbolManufacture>();
			case NewbieGuideScriptType.clickSymbolManufactureConfirm:
				return gameObject.AddComponent<NewbieGuideClickSymbolManufactureConfirm>();
			case NewbieGuideScriptType.clickSymbolLottery:
				return gameObject.AddComponent<NewbieGuideClickSymbolLottery>();
			case NewbieGuideScriptType.clickSymbolLotteryBack:
				return gameObject.AddComponent<NewbieGuideClickSymbolLotteryBack>();
			case NewbieGuideScriptType.clickBattleHeroAddedSkillSwitch:
				return gameObject.AddComponent<NewbieGuideClickBattleHeroAddedSkillSwitch>();
			case NewbieGuideScriptType.clickAddedSkillForBattle:
				return gameObject.AddComponent<NewbieGuideClickAddedSkillForBattle>();
			case NewbieGuideScriptType.clickAddedSkillArrow:
				return gameObject.AddComponent<NewbieGuideClickAddedSkillArrow>();
			case NewbieGuideScriptType.clickAddedSkillConfirm:
				return gameObject.AddComponent<NewbieGuideClickAddedSkillConfirm>();
			case NewbieGuideScriptType.clickHeroSkill:
				return gameObject.AddComponent<NewbieGuideClickHeroSkill>();
			default:
				switch (type)
				{
				case NewbieGuideScriptType.clickChangeTeam:
					return gameObject.AddComponent<NewbieGuideClickChangeTeam>();
				case NewbieGuideScriptType.clickBackToHall:
					return gameObject.AddComponent<NewbieGuideClickBackToHall>();
				case NewbieGuideScriptType.clickBattleHero:
					return gameObject.AddComponent<NewbieGuideClickBattleHero>();
				case NewbieGuideScriptType.clickBattleHeroConfirm:
					return gameObject.AddComponent<NewbieGuideClickBattleHeroConfirm>();
				case NewbieGuideScriptType.clickFullStarAward:
					return gameObject.AddComponent<NewbieGuideClickFullStarAward>();
				case NewbieGuideScriptType.clickFullStartAwardConfirm:
					return gameObject.AddComponent<NewbieGuideClickFullStartAwardConfirm>();
				case NewbieGuideScriptType.clickPvpBackToHall:
					return gameObject.AddComponent<NewbieGuideClickPvpBackToHall>();
				case NewbieGuideScriptType.clickChallenge:
					return gameObject.AddComponent<NewbieGuideClickChallenge>();
				case NewbieGuideScriptType.clickChallengeNext:
					return gameObject.AddComponent<NewbieGuideClickChallengeNext>();
				case NewbieGuideScriptType.clickMoreHero:
					return gameObject.AddComponent<NewbieGuideClickMoreHero>();
				case NewbieGuideScriptType.clickTaskFinish:
					return gameObject.AddComponent<NewbieGuideClickTaskFinish>();
				case NewbieGuideScriptType.clickTournamentDefense:
					return gameObject.AddComponent<NewbieGuideClickTournamentDefense>();
				case NewbieGuideScriptType.closeMoreHeroFolder:
					return gameObject.AddComponent<NewbieGuideCloseMoreHeroFolder>();
				case NewbieGuideScriptType.clickPvPEntryToHall:
					return gameObject.AddComponent<NewbieGuideClickPvPEntryToHall>();
				case NewbieGuideScriptType.clickAdvSelectToHall:
					return gameObject.AddComponent<NewbieGuideClickAdvToLobbyScript>();
				case NewbieGuideScriptType.clickPvPProfit:
					return gameObject.AddComponent<NewbieGuideClickPvPProfit>();
				default:
					switch (type)
					{
					case NewbieGuideScriptType.clickHallNewbie:
						return gameObject.AddComponent<NewbieGuideClickHallTask>();
					case NewbieGuideScriptType.clickMainTask:
						return gameObject.AddComponent<NewbieGuideClickMainTask>();
					case NewbieGuideScriptType.clickConfirmReward:
						return gameObject.AddComponent<NewbieGuideClickConfirmReward>();
					case NewbieGuideScriptType.clickRewardToHall:
						return gameObject.AddComponent<NewbieGuideClickRewardToHall>();
					case NewbieGuideScriptType.HighlightLevelList:
						return gameObject.AddComponent<NewbieGuideHighlightLevelList>();
					case NewbieGuideScriptType.HighlightUnlockNode:
						return gameObject.AddComponent<NewbieGuideHighlightUnlockNode>();
					case NewbieGuideScriptType.clickGetRewardBtn:
						return gameObject.AddComponent<NewbieGuideClickGetRewardBtn>();
					case NewbieGuideScriptType.HighlightLevelTaskNode:
						return gameObject.AddComponent<NewbieGuideHighlightLevelTaskNode>();
					case NewbieGuideScriptType.oldPlayerGrowGuide:
						return gameObject.AddComponent<NewbieGuideOldPlayerGrow>();
					}
					switch (type)
					{
					case NewbieGuideScriptType.clickSettingMenu:
						return gameObject.AddComponent<NewbieGuideClickSettingMenu>();
					case NewbieGuideScriptType.clickSettingOp:
						return gameObject.AddComponent<NewbieGuideClickSettingOp>();
					case NewbieGuideScriptType.clickAutoCasting:
						return gameObject.AddComponent<NewbieGuideClickAutoCasting>();
					case NewbieGuideScriptType.clickCadCasting:
						return gameObject.AddComponent<NewbieGuideClickCadCasting>();
					case NewbieGuideScriptType.clickAttackNearest:
						return gameObject.AddComponent<NewbieGuideClickAttackNearest>();
					case NewbieGuideScriptType.clickAttackWeakest:
						return gameObject.AddComponent<NewbieGuideClickAttackWeakest>();
					case NewbieGuideScriptType.clickCloseSettingMenu:
						return gameObject.AddComponent<NewbieGuideClickCloseSettingMenu>();
					case NewbieGuideScriptType.clickCaptainButton:
						return gameObject.AddComponent<NewbieGuideClickCaptainButton>();
					case NewbieGuideScriptType.clickCommonAttackType1:
						return gameObject.AddComponent<NewbieGuideCommonAttackType1>();
					case NewbieGuideScriptType.clickCommonAttackType2:
						return gameObject.AddComponent<NewbieGuideCommonAttackType2>();
					default:
						switch (type)
						{
						case NewbieGuideScriptType.clickHallMall:
							return gameObject.AddComponent<NewbieGuideClickHallMall>();
						case NewbieGuideScriptType.clickBuyOne:
							return gameObject.AddComponent<NewbieGuideClickBuyOne>();
						case NewbieGuideScriptType.clickHeroPackage:
							return gameObject.AddComponent<NewbieGuideClickHeroPackage>();
						case NewbieGuideScriptType.clickMallMenu:
							return gameObject.AddComponent<NewbieGuideClickMallMenu>();
						case NewbieGuideScriptType.highLightHeroListArea:
							return gameObject.AddComponent<NewbieGuideHighLightHeroListArea>();
						default:
							switch (type)
							{
							case NewbieGuideScriptType.clickCombat3v3:
								return gameObject.AddComponent<NewbieGuideClickSingleCombat>();
							case NewbieGuideScriptType.clickSymbolmenu:
								return gameObject.AddComponent<NewbieGuideClickSymbolMenu>();
							case NewbieGuideScriptType.clickSymbolBuy:
								return gameObject.AddComponent<NewbieGuideClickSymbolBuy>();
							case NewbieGuideScriptType.SymbolBagClickSymbol:
								return gameObject.AddComponent<NewbieGuideSymbolBagClickSymbol>();
							case NewbieGuideScriptType.clickSymbolBuyClose:
								return gameObject.AddComponent<NewbieGuideClickSymbolBuyClose>();
							default:
								switch (type)
								{
								case NewbieGuideScriptType.clickPvpEntry:
									return gameObject.AddComponent<NewbieGuideClickPvpEntry>();
								case NewbieGuideScriptType.clickPveEntry:
									return gameObject.AddComponent<NewbieGuideClickPveEntry>();
								case NewbieGuideScriptType.clickExploring:
									return gameObject.AddComponent<NewbieGuideClickExploring>();
								default:
									if (type == NewbieGuideScriptType.clickWaiting)
									{
										return gameObject.AddComponent<NewbieGuideWaitSomeTime>();
									}
									if (type == NewbieGuideScriptType.openForm)
									{
										return gameObject.AddComponent<NewbieGuideOpenForm>();
									}
									if (type == NewbieGuideScriptType.clickTournament)
									{
										return gameObject.AddComponent<NewbieGuideClickTournament>();
									}
									if (type == NewbieGuideScriptType.clickChallenging)
									{
										return gameObject.AddComponent<NewbieGuideClickChallenging>();
									}
									if (type == NewbieGuideScriptType.clickAnyWhereScreen)
									{
										return gameObject.AddComponent<NewbieGuideClickAnyWhereScreen>();
									}
									if (type == NewbieGuideScriptType.clickHonorMall)
									{
										return gameObject.AddComponent<NewbieGuideClickHonorMall>();
									}
									if (type == NewbieGuideScriptType.clickMsgBox)
									{
										return gameObject.AddComponent<NewbieGuideClickMsgBox>();
									}
									if (type == NewbieGuideScriptType.clickPlayerSkillBtn)
									{
										return gameObject.AddComponent<NewbieGuideClickPlayerSkillBtn>();
									}
									if (type == NewbieGuideScriptType.introGloryPoints)
									{
										return gameObject.AddComponent<NewbieGuideIntroGloryPoints>();
									}
									if (type != NewbieGuideScriptType.showImageGuide)
									{
										return null;
									}
									return gameObject.AddComponent<NewbieGuideShowImageGuide>();
								}
								break;
							}
							break;
						}
						break;
					}
					break;
				}
				break;
			}
			break;
		case NewbieGuideScriptType.clickNewbieDragonIcon:
			return gameObject.AddComponent<NewbieGuideClickDragonIcon>();
		case NewbieGuideScriptType.clickBuyEquip:
			return gameObject.AddComponent<NewbieGuideClickBuyEquip>();
		case NewbieGuideScriptType.clickFocus:
			return gameObject.AddComponent<NewbieGuideClickFocus>();
		case NewbieGuideScriptType.clickAttack:
			return gameObject.AddComponent<NewbieGuideClickAttack>();
		case NewbieGuideScriptType.clickDefense:
			return gameObject.AddComponent<NewbieGuideClickDefense>();
		case NewbieGuideScriptType.clickGather:
			return gameObject.AddComponent<NewbieGuideClickGather>();
		case NewbieGuideScriptType.clickVoice:
			return gameObject.AddComponent<NewbieGuideClickVoice>();
		case NewbieGuideScriptType.clickMap:
			return gameObject.AddComponent<NewbieGuideClickMap>();
		case NewbieGuideScriptType.clickEquipBuyBtn:
			return gameObject.AddComponent<NewbieGuideClickEquipBuyBtn>();
		case NewbieGuideScriptType.clickEquipPanel:
			return gameObject.AddComponent<NewbieGuideClickEquipPanel>();
		case NewbieGuideScriptType.clickJungleSword:
			return gameObject.AddComponent<NewbieGuideClickJungleSword>();
		case NewbieGuideScriptType.clickJungleEquipPanel:
			return gameObject.AddComponent<NewbieGuideClickJungleEquipPanel>();
		case NewbieGuideScriptType.clickEuipPanelClose:
			return gameObject.AddComponent<NewbieGuideClickEquipPanelClose>();
		case NewbieGuideScriptType.autoCloseEquipForm:
			return gameObject.AddComponent<NewbieGuideCloseEquipForm>();
		case NewbieGuideScriptType.bigMapSignGuide:
			return gameObject.AddComponent<NewbieGuideBigMapGign>();
		case NewbieGuideScriptType.selectModeGuide:
			return gameObject.AddComponent<NewbieGuideShowSelectModeGuide>();
		case NewbieGuideScriptType.prefabeTextGuide:
			return gameObject.AddComponent<NewbieGuideShowHighlightPrefabeText>();
		case NewbieGuideScriptType.cameraMoveGuide:
			return gameObject.AddComponent<NewbieGuideCameraMoveGuide>();
		case NewbieGuideScriptType.eyeGuide:
			return gameObject.AddComponent<NewbieGuideEyeGuide>();
		case NewbieGuideScriptType.clickBattleInfoBtn:
			return gameObject.AddComponent<NewbieGuideClickBattleInfoBtn>();
		}
		goto IL_A5;
	}
}
