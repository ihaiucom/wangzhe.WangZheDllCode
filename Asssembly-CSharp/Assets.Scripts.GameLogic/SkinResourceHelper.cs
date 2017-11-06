using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;
using System.Text;

namespace Assets.Scripts.GameLogic
{
	public class SkinResourceHelper
	{
		public static string GetSkinResourceName(int configID, int markID, string resName, int advancelevel = 0)
		{
			if (configID != 0 && markID != 0)
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long)configID);
				if (dataByKey == null)
				{
					return resName;
				}
				int num = resName.LastIndexOf('/');
				StringBuilder stringBuilder = new StringBuilder(resName);
				string text = "prefab_skill_effects/hero_skill_effects/";
				StringBuilder stringBuilder2 = new StringBuilder(text);
				stringBuilder2.AppendFormat("{0}_{1}/{2}", configID, dataByKey.szNamePinYin, markID);
				if (num >= 0)
				{
					stringBuilder.Remove(0, num);
					stringBuilder2.Append(stringBuilder);
					if (advancelevel > 0)
					{
						stringBuilder2.AppendFormat("_level{0}", advancelevel);
					}
					return stringBuilder2.ToString();
				}
			}
			return resName;
		}

		public static string GetResourceName(ref PoolObjHandle<ActorRoot> _attack, string _resName, bool _bUseAdvanceSkin)
		{
			if (_attack && _attack.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && _attack.handle.ActorControl != null)
			{
				HeroWrapper heroWrapper = (HeroWrapper)_attack.handle.ActorControl;
				uint num;
				if (heroWrapper != null && heroWrapper.GetSkinCfgID(out num))
				{
					int num2 = _resName.LastIndexOf('/');
					StringBuilder stringBuilder = new StringBuilder(_resName);
					StringBuilder stringBuilder2 = new StringBuilder(heroWrapper.GetSkinEffectPath());
					if (num2 >= 0)
					{
						stringBuilder.Remove(0, num2);
						stringBuilder2.Append(stringBuilder);
						if (_bUseAdvanceSkin)
						{
							int advanceSkinIndex = heroWrapper.GetAdvanceSkinIndex();
							if (advanceSkinIndex > 0)
							{
								stringBuilder2.AppendFormat("_level{0}", advanceSkinIndex);
							}
						}
						return stringBuilder2.ToString();
					}
				}
			}
			return _resName;
		}

		public static string GetResourceName(Action _action, string _resName, bool _bUseAdvanceSkin)
		{
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			if (refParamObject != null && refParamObject.Originator && refParamObject.Originator.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && refParamObject.Originator.handle.ActorControl != null)
			{
				HeroWrapper heroWrapper = (HeroWrapper)refParamObject.Originator.handle.ActorControl;
				uint num;
				if (heroWrapper != null && heroWrapper.GetSkinCfgID(out num))
				{
					int num2 = _resName.LastIndexOf('/');
					StringBuilder stringBuilder = new StringBuilder(_resName);
					StringBuilder stringBuilder2 = new StringBuilder(heroWrapper.GetSkinEffectPath());
					if (num2 >= 0)
					{
						stringBuilder.Remove(0, num2);
						stringBuilder2.Append(stringBuilder);
						if (_bUseAdvanceSkin)
						{
							int advanceSkinIndex = heroWrapper.GetAdvanceSkinIndex();
							if (advanceSkinIndex > 0)
							{
								stringBuilder2.AppendFormat("_level{0}", advanceSkinIndex);
							}
						}
						return stringBuilder2.ToString();
					}
				}
			}
			return _resName;
		}
	}
}
