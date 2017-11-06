using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CSkillData
	{
		private static readonly string[] slotBgStrs = new string[]
		{
			"Common_Bg_Physicalbg",
			"Common_Bg_Spellbg",
			"Common_Bg_Realbg",
			"Common_Bg_Controlbg"
		};

		private uint m_heroCfgId;

		private CrypticInt32[] skillLevelArr = new CrypticInt32[5];

		private int[] skillIdArr = new int[5];

		public uint SelSkillID;

		public void InitSkillData(ResHeroCfgInfo heroCfgInfo, COMDT_SKILLARRAY svrSkillArr)
		{
			if (heroCfgInfo != null)
			{
				this.m_heroCfgId = heroCfgInfo.dwCfgID;
				for (int i = 0; i < heroCfgInfo.astSkill.Length; i++)
				{
					this.skillIdArr[i] = heroCfgInfo.astSkill[i].iSkillID;
				}
			}
			for (int j = 0; j < svrSkillArr.astSkillInfo.Length; j++)
			{
				if (svrSkillArr.astSkillInfo[j].bUnlocked > 0)
				{
					this.skillLevelArr[j] = (int)svrSkillArr.astSkillInfo[j].wLevel;
				}
				else
				{
					this.skillLevelArr[j] = 0;
				}
			}
			this.SelSkillID = svrSkillArr.dwSelSkillID;
		}

		public void UnLockSkill(CSDT_UNLOCKSKILL unlockSkill)
		{
			for (int i = 0; i < (int)unlockSkill.bUnlockCnt; i++)
			{
				int slotId = (int)unlockSkill.szSkillSlot[i];
				this.SetSkillLevel(slotId, 1);
			}
		}

		public void UnLockSkill(int slotId)
		{
			this.SetSkillLevel(slotId, 1);
		}

		public void SetSkillLevel(int slotId, int level)
		{
			this.skillLevelArr[slotId] = level;
			Singleton<EventRouter>.GetInstance().BroadCastEvent<uint>("HeroSkillLevelChange", this.m_heroCfgId);
		}

		public int GetSkillLevel(int slotId)
		{
			return this.skillLevelArr[slotId];
		}

		public int GetSkillId(int slotId)
		{
			return this.skillIdArr[slotId];
		}

		public void CreateSvrSkillInfo(ref COMDT_SKILLARRAY svrSkillArr)
		{
			if (svrSkillArr == null)
			{
				return;
			}
			for (int i = 0; i < this.skillLevelArr.Length; i++)
			{
				svrSkillArr.astSkillInfo[i].bUnlocked = ((this.skillLevelArr[i] > 0) ? 1 : 0);
				svrSkillArr.astSkillInfo[i].wLevel = (ushort)this.skillLevelArr[i];
			}
		}

		public string GetSkillDesc(int slotId, uint heroId)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CHeroInfo cHeroInfo;
			masterRoleInfo.GetHeroInfo(this.m_heroCfgId, out cHeroInfo, true);
			DebugHelper.Assert(cHeroInfo != null);
			ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long)this.skillIdArr[slotId]);
			DebugHelper.Assert(dataByKey != null);
			string text = StringHelper.UTF8BytesToString(ref dataByKey.szSkillDesc);
			string[] escapeString = CSkillData.GetEscapeString(text);
			if (escapeString != null)
			{
				for (int i = 0; i < escapeString.Length; i++)
				{
					text = text.Replace("[" + escapeString[i] + "]", CSkillData.CalcEscapeValue(escapeString[i], cHeroInfo, this.GetSkillLevel(slotId), 1, heroId).ToString());
				}
			}
			return text;
		}

		public static string[] GetEscapeString(string skillDesc)
		{
			if (skillDesc == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			int num = skillDesc.IndexOf("[");
			int num2 = skillDesc.IndexOf("]");
			while (num != -1 && num2 != -1)
			{
				list.Add(skillDesc.Substring(num + 1, num2 - num - 1));
				num = skillDesc.IndexOf("[", num2 + 1);
				num2 = skillDesc.IndexOf("]", num2 + 1);
			}
			return list.ToArray();
		}

		public static int CalcEscapeValue(string escapeString, CHeroInfo heroInfo, int skillLevel, int heroSoulLevel, uint heroId)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string[] array = escapeString.Split(new char[]
			{
				'+',
				'-'
			});
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<char> list3 = new List<char>();
			List<char> list4 = new List<char>();
			for (int i = 0; i < array.Length; i++)
			{
				list2.Clear();
				list4.Clear();
				text = array[i];
				string[] array2 = text.Split(new char[]
				{
					'*',
					'/'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					text2 = array2[j];
					int num;
					if (int.TryParse(text2, ref num))
					{
						list2.Add(num);
					}
					else if (CSkillData.ParsePlayerProperty(heroInfo, text2, out num))
					{
						list2.Add(num);
					}
					else if (CSkillData.ParseSkillProperty(text2, out num, heroId))
					{
						list2.Add(num);
					}
					else if (CSkillData.ParseSkillLevel(text2, out num, skillLevel))
					{
						list2.Add(num);
					}
					else
					{
						if (!CSkillData.ParseHeroSoulLevel(text2, out num, heroSoulLevel))
						{
							DebugHelper.Assert(false, "Skill Data Desc[{0}] can not be parsed..", new object[]
							{
								text2
							});
							return 0;
						}
						list2.Add(num);
					}
				}
				for (int k = 0; k < text.get_Length(); k++)
				{
					char c = text.get_Chars(k);
					if (c.Equals('*') || c.Equals('/'))
					{
						list4.Add(c);
					}
				}
				int num2 = list2.get_Item(0);
				for (int l = 0; l < list4.get_Count(); l++)
				{
					char c2 = list4.get_Item(l);
					if (c2 != '*')
					{
						if (c2 == '/')
						{
							num2 /= list2.get_Item(l + 1);
						}
					}
					else
					{
						num2 *= list2.get_Item(l + 1);
					}
				}
				list.Add(num2);
			}
			for (int m = 0; m < escapeString.get_Length(); m++)
			{
				char c3 = escapeString.get_Chars(m);
				if (c3.Equals('+') || c3.Equals('-'))
				{
					list3.Add(c3);
				}
			}
			int num3 = list.get_Item(0);
			for (int n = 0; n < list3.get_Count(); n++)
			{
				switch (list3.get_Item(n))
				{
				case '+':
					num3 += list.get_Item(n + 1);
					break;
				case '-':
					num3 -= list.get_Item(n + 1);
					break;
				}
			}
			return num3;
		}

		public static int CalcEscapeValue(string escapeString, ValueDataInfo[] valueData, int skillLevel, int heroSoulLevel, uint heroId)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string[] array = escapeString.Split(new char[]
			{
				'+',
				'-'
			});
			List<float> list = new List<float>();
			List<int> list2 = new List<int>();
			List<char> list3 = new List<char>();
			List<char> list4 = new List<char>();
			for (int i = 0; i < array.Length; i++)
			{
				list2.Clear();
				list4.Clear();
				text = array[i];
				string[] array2 = text.Split(new char[]
				{
					'*',
					'/'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					text2 = array2[j];
					int num;
					if (int.TryParse(text2, ref num))
					{
						list2.Add(num);
					}
					else if (CSkillData.ParsePlayerProperty(valueData, text2, out num))
					{
						list2.Add(num);
					}
					else if (CSkillData.ParseSkillProperty(text2, out num, heroId))
					{
						list2.Add(num);
					}
					else if (CSkillData.ParseSkillLevel(text2, out num, skillLevel))
					{
						list2.Add(num);
					}
					else
					{
						if (!CSkillData.ParseHeroSoulLevel(text2, out num, heroSoulLevel))
						{
							DebugHelper.Assert(false, "Skill Data Desc[{0}] can not be parsed..", new object[]
							{
								text2
							});
							return 0;
						}
						list2.Add(num);
					}
				}
				for (int k = 0; k < text.get_Length(); k++)
				{
					char c = text.get_Chars(k);
					if (c.Equals('*') || c.Equals('/'))
					{
						list4.Add(c);
					}
				}
				float num2 = (float)list2.get_Item(0);
				for (int l = 0; l < list4.get_Count(); l++)
				{
					char c2 = list4.get_Item(l);
					if (c2 != '*')
					{
						if (c2 == '/')
						{
							num2 /= (float)list2.get_Item(l + 1);
						}
					}
					else
					{
						num2 *= (float)list2.get_Item(l + 1);
					}
				}
				list.Add(num2);
			}
			for (int m = 0; m < escapeString.get_Length(); m++)
			{
				char c3 = escapeString.get_Chars(m);
				if (c3.Equals('+') || c3.Equals('-'))
				{
					list3.Add(c3);
				}
			}
			float num3 = list.get_Item(0);
			for (int n = 0; n < list3.get_Count(); n++)
			{
				switch (list3.get_Item(n))
				{
				case '+':
					num3 += list.get_Item(n + 1);
					break;
				case '-':
					num3 -= list.get_Item(n + 1);
					break;
				}
			}
			return (int)num3;
		}

		private static bool ParsePlayerProperty(CHeroInfo heroInfo, string s, out int value)
		{
			value = 0;
			if (s.get_Chars(0) == 'k')
			{
				int num = Convert.ToInt32(s.Substring(1));
				RES_FUNCEFT_TYPE key = (RES_FUNCEFT_TYPE)num;
				value = heroInfo.mActorValue[key].totalValue;
				return true;
			}
			return false;
		}

		private static bool ParsePlayerProperty(ValueDataInfo[] valueData, string s, out int value)
		{
			value = 0;
			if (s.get_Chars(0) == 'k')
			{
				int num = Convert.ToInt32(s.Substring(1));
				value = valueData[num].totalValue;
				return true;
			}
			return false;
		}

		private static bool ParseSkillProperty(string s, out int value, uint heroId)
		{
			value = 0;
			int num = s.IndexOf('p');
			int num2 = s.IndexOf('q');
			int num3 = s.IndexOf('g');
			int num4 = s.IndexOf('t');
			int num5 = s.IndexOf('a');
			int num6 = s.IndexOf('b');
			if (num != -1)
			{
				if (num2 != -1)
				{
					int num7 = Convert.ToInt32(s.Substring(0, num));
					int num8 = Convert.ToInt32(s.Substring(num + 1, num2 - num - 1));
					int num9 = Convert.ToInt32(s.Substring(num2 + 1));
					ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long)num7);
					DebugHelper.Assert(dataByKey != null, "ResSkillCombineCfgInfo[{0}] can not be found! from string:\"{1}\" heroId[{2}]", new object[]
					{
						num7,
						s,
						heroId.ToString()
					});
					if (dataByKey != null)
					{
						value = dataByKey.astSkillFuncInfo[num8 - 1].astSkillFuncParam[num9 - 1].iParam;
					}
					return true;
				}
				if (num3 != -1)
				{
					int num10 = Convert.ToInt32(s.Substring(0, num));
					int num11 = Convert.ToInt32(s.Substring(num + 1, num3 - num - 1));
					int num12 = Convert.ToInt32(s.Substring(num3 + 1));
					ResSkillCombineCfgInfo dataByKey2 = GameDataMgr.skillCombineDatabin.GetDataByKey((long)num10);
					DebugHelper.Assert(dataByKey2 != null);
					if (dataByKey2 != null)
					{
						value = dataByKey2.astSkillFuncInfo[num11 - 1].astSkillFuncGroup[num12 - 1].iParam;
					}
					return true;
				}
			}
			else if (num4 != -1)
			{
				if (num5 != -1)
				{
					int num13 = Convert.ToInt32(s.Substring(0, num4));
					ResSkillCombineCfgInfo dataByKey3 = GameDataMgr.skillCombineDatabin.GetDataByKey((long)num13);
					DebugHelper.Assert(dataByKey3 != null);
					if (dataByKey3 != null)
					{
						value = dataByKey3.iDuration;
					}
					return true;
				}
				if (num6 != -1)
				{
					int num14 = Convert.ToInt32(s.Substring(0, num4));
					ResSkillCombineCfgInfo dataByKey4 = GameDataMgr.skillCombineDatabin.GetDataByKey((long)num14);
					DebugHelper.Assert(dataByKey4 != null);
					if (dataByKey4 != null)
					{
						value = dataByKey4.iDurationGrow;
					}
					return true;
				}
			}
			return false;
		}

		private static bool ParseSkillLevel(string s, out int value, int skillLevel)
		{
			value = 0;
			if (s.get_Chars(0) == 's' && s.get_Chars(1) == 'l')
			{
				value = skillLevel;
				return true;
			}
			return false;
		}

		private static bool ParseHeroSoulLevel(string s, out int value, int heroSoulLevel)
		{
			value = 0;
			if (s.get_Chars(0) == 'h' && s.get_Chars(1) == 'l')
			{
				value = heroSoulLevel;
				return true;
			}
			return false;
		}

		public string GetSkillUpTip(int slotId, uint heroId)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CHeroInfo cHeroInfo;
			masterRoleInfo.GetHeroInfo(this.m_heroCfgId, out cHeroInfo, true);
			DebugHelper.Assert(cHeroInfo != null);
			ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long)this.skillIdArr[slotId]);
			DebugHelper.Assert(dataByKey != null);
			string text = StringHelper.UTF8BytesToString(ref dataByKey.szSkillUpTip);
			string[] escapeString = CSkillData.GetEscapeString(text);
			if (escapeString != null)
			{
				for (int i = 0; i < escapeString.Length; i++)
				{
					int num = CSkillData.CalcEscapeValue(escapeString[i], cHeroInfo, this.GetSkillLevel(slotId), 1, heroId);
					string text2 = string.Empty;
					if (num != 0)
					{
						text2 = num.ToString();
					}
					text = text.Replace("[" + escapeString[i] + "]", text2);
				}
			}
			return text;
		}

		public static ResSkillCfgInfo GetSkillCfgInfo(int skillId)
		{
			ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long)skillId);
			if (dataByKey == null)
			{
				return null;
			}
			return dataByKey;
		}

		public static string GetEffectDesc(SkillEffectType skillEffectType)
		{
			CTextManager instance = Singleton<CTextManager>.instance;
			string text = "{0}{1}";
			object obj = "Skill_Common_Effect_Type_";
			uint num = (uint)skillEffectType;
			return instance.GetText(string.Format(text, obj, num.ToString()));
		}

		public static string GetEffectSlotBg(SkillEffectType skillEffectType)
		{
			return string.Format("{0}{1}", "UGUI/Sprite/Common/", CSkillData.slotBgStrs[(int)((uint)((UIntPtr)((ulong)((long)(skillEffectType - SkillEffectType.Physical)))))]);
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			for (int i = 1; i <= 4; i++)
			{
				preloadTab.AddSprite(CSkillData.GetEffectSlotBg((SkillEffectType)i));
			}
		}
	}
}
