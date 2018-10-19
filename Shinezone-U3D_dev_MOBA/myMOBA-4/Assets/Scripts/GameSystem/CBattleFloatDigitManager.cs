using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CBattleFloatDigitManager
	{
		private const string FLOAT_TEXT_PREFAB = "Text/FloatText/FloatText.prefab";

		private const uint DEFAULT_FONT_SIZE = 6u;

		private const uint HP_RECOVER_SHOW_THRESHOLD = 50u;

		private static string[][] s_battleFloatDigitAnimatorStates = new string[][]
		{
			new string[]
			{
				string.Empty
			},
			new string[]
			{
				"Physics_Right",
				"Physics_Left"
			},
			new string[]
			{
				"Physics_RightCrit",
				"Physics_LeftCrit"
			},
			new string[]
			{
				"Magic_Right",
				"Magic_Left"
			},
			new string[]
			{
				"Magic_RightCrit",
				"Magic_LeftCrit"
			},
			new string[]
			{
				"ZhenShi_Right",
				"ZhenShi_Left"
			},
			new string[]
			{
				"ZhenShi_RightCrit",
				"ZhenShi_LeftCrit"
			},
			new string[]
			{
				"SufferPhysicalDamage"
			},
			new string[]
			{
				"SufferMagicDamage"
			},
			new string[]
			{
				"SufferRealDamage"
			},
			new string[]
			{
				"ReviveHp"
			},
			new string[]
			{
				"Exp"
			},
			new string[]
			{
				"Gold"
			},
			new string[]
			{
				"LastHitGold"
			}
		};

		private static string[] s_restrictTextKeys = new string[]
		{
			"Restrict_None",
			"Restrict_Dizzy",
			"Restrict_SlowDown",
			"Restrict_Taunt",
			"Restrict_Fear",
			"Restrict_Frozen",
			"Restrict_Floating",
			"Restrict_Slient",
			"Restrict_Stone",
			"SkillBuff_Custom_Type_1",
			"SkillBuff_Custom_Type_2",
			"SkillBuff_Custom_Type_3",
			"SkillBuff_Custom_Type_4",
			"SkillBuff_Custom_Type_5",
			"SkillBuff_Custom_Type_6",
			"SkillBuff_Custom_Type_7",
			"SkillBuff_Custom_Type_8",
			"SkillBuff_Custom_Type_9",
			"SkillBuff_Custom_Type_10",
			"SkillBuff_Custom_Type_11",
			"SkillBuff_Custom_Type_12",
			"SkillBuff_Custom_Type_13",
			"SkillBuff_Custom_Type_14",
			"SkillBuff_Custom_Type_15",
			"SkillBuff_Custom_Type_16",
			"SkillBuff_Custom_Type_17",
			"SkillBuff_Custom_Type_18",
			"SkillBuff_Custom_Type_19",
			"SkillBuff_Custom_Type_20",
			"SkillBuff_Custom_Type_21",
			"SkillBuff_Custom_Type_22",
			"SkillBuff_Custom_Type_23",
			"SkillBuff_Custom_Type_24",
			"SkillBuff_Custom_Type_25",
			"SkillBuff_Custom_Type_26",
			"SkillBuff_Custom_Type_27",
			"SkillBuff_Custom_Type_28",
			"SkillBuff_Custom_Type_29",
			"SkillBuff_Custom_Type_30",
			"SkillBuff_Custom_Type_31",
			"SkillBuff_Custom_Type_32",
			"SkillBuff_Custom_Type_33",
			"SkillBuff_Custom_Type_34",
			"SkillBuff_Custom_Type_35",
			"SkillBuff_Custom_Type_36",
			"SkillBuff_Custom_Type_37",
			"SkillBuff_Custom_Type_38",
			"SkillBuff_Custom_Type_39",
			"SkillBuff_Custom_Type_40"
		};

		private static string s_restrictTextAnimatorState = "RestrictText_Anim";

		private static string[] s_otherFloatTextKeys = new string[]
		{
			"Accept_Task",
			"Complete_Task",
			"Level_Up",
			"Talent_Open",
			"Talent_Learn",
			"DragonBuff_Get1",
			"DragonBuff_Get2",
			"DragonBuff_Get3",
			"Battle_Absorb",
			"Battle_ShieldDisappear",
			"Battle_Immunity",
			"Battle_InCooldown",
			"Battle_NoTarget",
			"Battle_MagicShortage",
			"Battle_Blindess",
			"Battle_MadnessShortage",
			"Battle_EnergyShortage",
			"Battle_FuryShortage",
			"Battle_BeanShortage"
		};

		private static string[] s_otherFloatTextAnimatorStates = new string[]
		{
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim",
			"Other_Anim"
		};

		private List<FloatDigitInfo> m_floatDigitInfoList;

		private bool CanMergeToCritText(ref DIGIT_TYPE type1, DIGIT_TYPE type2)
		{
			if ((type1 == DIGIT_TYPE.PhysicalAttackNormal && type2 == DIGIT_TYPE.PhysicalAttackCrit) || (type1 == DIGIT_TYPE.PhysicalAttackCrit && type2 == DIGIT_TYPE.PhysicalAttackNormal) || (type1 == DIGIT_TYPE.MagicAttackNormal && type2 == DIGIT_TYPE.MagicAttackCrit) || (type1 == DIGIT_TYPE.MagicAttackCrit && type2 == DIGIT_TYPE.MagicAttackNormal) || (type1 == DIGIT_TYPE.RealAttackNormal && type2 == DIGIT_TYPE.RealAttackCrit) || (type1 == DIGIT_TYPE.RealAttackCrit && type2 == DIGIT_TYPE.RealAttackNormal))
			{
				if (type1 < type2)
				{
					type1 = type2;
				}
				return true;
			}
			return false;
		}

		public void CollectFloatDigitInSingleFrame(PoolObjHandle<ActorRoot> attacker, PoolObjHandle<ActorRoot> target, DIGIT_TYPE digitType, int value)
		{
			if (MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				return;
			}
			if (this.m_floatDigitInfoList == null)
			{
				this.m_floatDigitInfoList = new List<FloatDigitInfo>();
			}
			FloatDigitInfo floatDigitInfo;
			for (int i = 0; i < this.m_floatDigitInfoList.Count; i++)
			{
				floatDigitInfo = this.m_floatDigitInfoList[i];
				if (floatDigitInfo.m_attacker == attacker && floatDigitInfo.m_target == target && (floatDigitInfo.m_digitType == digitType || this.CanMergeToCritText(ref floatDigitInfo.m_digitType, digitType)))
				{
					floatDigitInfo.m_value += value;
					this.m_floatDigitInfoList[i] = floatDigitInfo;
					return;
				}
			}
			floatDigitInfo = new FloatDigitInfo(attacker, target, digitType, value);
			this.m_floatDigitInfoList.Add(floatDigitInfo);
		}

		private void updateFloatDigitInLastFrame()
		{
			if (MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				return;
			}
			if (this.m_floatDigitInfoList == null || this.m_floatDigitInfoList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < this.m_floatDigitInfoList.Count; i++)
			{
				FloatDigitInfo floatDigitInfo = this.m_floatDigitInfoList[i];
				if (floatDigitInfo.m_attacker && floatDigitInfo.m_target)
				{
					if (floatDigitInfo.m_digitType == DIGIT_TYPE.ReviveHp)
					{
						Vector3 position = floatDigitInfo.m_target.handle.myTransform.position;
						this.CreateBattleFloatDigit(floatDigitInfo.m_value, floatDigitInfo.m_digitType, ref position);
					}
					else
					{
						Vector3 position2 = floatDigitInfo.m_target.handle.myTransform.position;
						Vector3 position3 = floatDigitInfo.m_attacker.handle.myTransform.position;
						SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
						Vector3 a;
						Vector3 b;
						float num;
						float num2;
						if (curLvelContext != null && curLvelContext.m_isCameraFlip)
						{
							a = position2;
							b = position3;
							num = UnityEngine.Random.Range(0.5f, 1f);
							num2 = UnityEngine.Random.Range(-1f, -0.5f);
						}
						else
						{
							a = position3;
							b = position2;
							num = UnityEngine.Random.Range(-1f, -0.5f);
							num2 = UnityEngine.Random.Range(0.5f, 1f);
						}
						if ((a - b).x > 0f)
						{
							Vector3 position = new Vector3(position2.x + num, position2.y + Math.Abs(num), position2.z);
							this.CreateBattleFloatDigit(floatDigitInfo.m_value, floatDigitInfo.m_digitType, ref position, 1);
						}
						else
						{
							Vector3 position = new Vector3(position2.x + num2, position2.y + Math.Abs(num2), position2.z);
							this.CreateBattleFloatDigit(floatDigitInfo.m_value, floatDigitInfo.m_digitType, ref position, 0);
						}
					}
				}
			}
			this.m_floatDigitInfoList.Clear();
		}

		public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, ref Vector3 worldPosition)
		{
			if (GameSettings.RenderQuality == SGameRenderQuality.Low && digitType != DIGIT_TYPE.MagicAttackCrit && digitType != DIGIT_TYPE.PhysicalAttackCrit && digitType != DIGIT_TYPE.RealAttackCrit && digitType != DIGIT_TYPE.ReceiveGoldCoinInBattle && digitType != DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle)
			{
				return;
			}
			if (digitType == DIGIT_TYPE.ReviveHp && (long)digitValue < 50L)
			{
				return;
			}
			string[] array = CBattleFloatDigitManager.s_battleFloatDigitAnimatorStates[(int)digitType];
			if (array.Length <= 0)
			{
				return;
			}
			string text = (((digitType != DIGIT_TYPE.ReviveHp && digitType != DIGIT_TYPE.ReceiveSpirit && digitType != DIGIT_TYPE.ReceiveGoldCoinInBattle) || digitValue <= 0) ? string.Empty : "+") + Mathf.Abs(digitValue).ToString();
			if (digitType == DIGIT_TYPE.ReceiveSpirit)
			{
				text += "xp";
				return;
			}
			if (digitType == DIGIT_TYPE.ReceiveGoldCoinInBattle || digitType == DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle)
			{
				text += "g";
			}
			this.CreateBattleFloatText(text, ref worldPosition, array[UnityEngine.Random.Range(0, array.Length)], 0u);
		}

		public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, ref Vector3 worldPosition, int animatIndex)
		{
			if (GameSettings.RenderQuality == SGameRenderQuality.Low && digitType != DIGIT_TYPE.MagicAttackCrit && digitType != DIGIT_TYPE.PhysicalAttackCrit && digitType != DIGIT_TYPE.RealAttackCrit && digitType != DIGIT_TYPE.ReceiveGoldCoinInBattle)
			{
				return;
			}
			string[] array = CBattleFloatDigitManager.s_battleFloatDigitAnimatorStates[(int)digitType];
			if (array.Length <= 0 || animatIndex < 0 || animatIndex >= array.Length)
			{
				return;
			}
			string content = ((digitType != DIGIT_TYPE.ReceiveSpirit || digitValue <= 0) ? string.Empty : "+") + SimpleNumericString.GetNumeric(Mathf.Abs(digitValue));
			this.CreateBattleFloatText(content, ref worldPosition, array[animatIndex], 0u);
		}

		public void CreateRestrictFloatText(RESTRICT_TYPE restrictType, ref Vector3 worldPosition)
		{
			string text = Singleton<CTextManager>.GetInstance().GetText(CBattleFloatDigitManager.s_restrictTextKeys[(int)restrictType]);
			this.CreateBattleFloatText(text, ref worldPosition, CBattleFloatDigitManager.s_restrictTextAnimatorState, 0u);
		}

		public void CreateSpecifiedFloatText(uint floatTextID, ref Vector3 worldPosition)
		{
			ResBattleFloatText dataByKey = GameDataMgr.floatTextDatabin.GetDataByKey(floatTextID);
			if (dataByKey != null)
			{
				string animatorState = (dataByKey.szAnimation.Length <= 0) ? CBattleFloatDigitManager.s_restrictTextAnimatorState : dataByKey.szAnimation;
				this.CreateBattleFloatText(dataByKey.szText, ref worldPosition, animatorState, dataByKey.dwFontsize);
			}
		}

		public void CreateOtherFloatText(enOtherFloatTextContent otherFloatTextContent, ref Vector3 worldPosition, params string[] args)
		{
			if (GameSettings.RenderQuality == SGameRenderQuality.Low)
			{
				return;
			}
			string text = Singleton<CTextManager>.GetInstance().GetText(CBattleFloatDigitManager.s_otherFloatTextKeys[(int)otherFloatTextContent], args);
			this.CreateBattleFloatText(text, ref worldPosition, CBattleFloatDigitManager.s_otherFloatTextAnimatorStates[(int)otherFloatTextContent], 0u);
		}

		public void ClearAllBattleFloatText()
		{
			if (this.m_floatDigitInfoList != null)
			{
				this.m_floatDigitInfoList.Clear();
				this.m_floatDigitInfoList = null;
			}
		}

		public void LateUpdate()
		{
			this.updateFloatDigitInLastFrame();
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddParticle("Text/FloatText/FloatText.prefab");
		}

		private void CreateBattleFloatText(string content, ref Vector3 worldPosition, string animatorState, uint fontSize = 0u)
		{
			if (MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				return;
			}
			if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(animatorState))
			{
				return;
			}
			GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject("Text/FloatText/FloatText.prefab", enResourceType.BattleScene);
			BattleFloatTextComponent cachedComponent = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<BattleFloatTextComponent>(gameObject, false);
			if (gameObject == null)
			{
				return;
			}
			if (null == Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera())
			{
				return;
			}
			gameObject.transform.parent = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera().transform;
			gameObject.transform.localRotation = Quaternion.identity;
			TextMeshPro texMeshPro = cachedComponent.texMeshPro;
			Animator anim = cachedComponent.anim;
			Vector3 position = Camera.main.WorldToScreenPoint(worldPosition);
			position.Set(position.x, position.y, 30f);
			gameObject.transform.position = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera().ScreenToWorldPoint(position);
			if (animatorState.IndexOf("Crit") != -1 && cachedComponent.iconTrans != null)
			{
				Vector3 localPosition = cachedComponent.iconTrans.localPosition;
				if (animatorState.IndexOf("Left") != -1)
				{
					localPosition.x = -0.3f * (float)(content.Length + 1);
				}
				else
				{
					localPosition.x = -0.3f * (float)(content.Length + 1);
				}
				cachedComponent.iconTrans.localPosition = localPosition;
			}
			if (animatorState.IndexOf("LastHit") != -1 && cachedComponent.iconTrans != null)
			{
				Vector3 localPosition2 = cachedComponent.iconTrans.localPosition;
				localPosition2.x = -0.24f * (float)(content.Length + 1);
				cachedComponent.iconTrans.localPosition = localPosition2;
			}
			int num;
			if (int.TryParse(content, out num))
			{
				Vector3 localScale = gameObject.transform.localScale;
				if (num > 1500)
				{
					localScale.x = 1.2f;
					localScale.y = 1.2f;
				}
				else if (num > 600)
				{
					localScale.x = 1.1f;
					localScale.y = 1.1f;
				}
				else if (num > 300)
				{
					localScale.x = 1f;
					localScale.y = 1f;
				}
				else if (num > 100)
				{
					localScale.x = 0.8f;
					localScale.y = 0.8f;
				}
				else
				{
					localScale.x = 0.7f;
					localScale.y = 0.7f;
				}
				gameObject.transform.localScale = localScale;
			}
			if (anim != null)
			{
				anim.Play(animatorState);
			}
			if (texMeshPro != null)
			{
				texMeshPro.text = content;
				texMeshPro.fontSize = ((fontSize <= 0u) ? 6u : fontSize);
			}
			Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(gameObject, 2000, null, null, null);
		}

		public void ClearBattleFloatText(CUIAnimatorScript animatorScript)
		{
		}
	}
}
