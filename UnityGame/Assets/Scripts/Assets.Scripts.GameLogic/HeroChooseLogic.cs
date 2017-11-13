using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class HeroChooseLogic : Singleton<HeroChooseLogic>
	{
		public static string s_heroInitChooseFormPath = "UGUI/Form/System/HeroInitChoose/Form_Hero_InitChoose.prefab";

		private Text heroDescTxt;

		private GameObject selectHeroBtn;

		private uint selectHeroId;

		private List<GameObject> cacheObjList = new List<GameObject>();

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Hero_Init_Select, new CUIEventManager.OnUIEventHandler(this.OnClickHeroModel));
		}

		public void OpenInitChooseHeroForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(HeroChooseLogic.s_heroInitChooseFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			this.heroDescTxt = cUIFormScript.transform.Find("heroDescTxt").GetComponent<Text>();
			this.heroDescTxt.set_text(string.Empty);
			this.selectHeroBtn = cUIFormScript.transform.Find("selectHeroBtn").gameObject;
			GUIEventListener.Get(this.selectHeroBtn).onClick += new GUIEventListener.VoidDelegate(this.OnConfirmChooseHero);
			this.selectHeroBtn.CustomSetActive(false);
			this.InitHeroPanel();
		}

		public void CloseInitChooseHeroForm()
		{
			this.OnDestroyActorList();
			Singleton<CUIManager>.GetInstance().CloseForm(HeroChooseLogic.s_heroInitChooseFormPath);
		}

		private void OnDestroyActorList()
		{
			for (int i = 0; i < this.cacheObjList.get_Count(); i++)
			{
				Object.DestroyObject(this.cacheObjList.get_Item(i));
			}
			this.cacheObjList.Clear();
		}

		private void OnConfirmChooseHero(GameObject go)
		{
			Singleton<LobbyLogic>.GetInstance().ReqSelectHero(this.selectHeroId);
		}

		public void OnClickHeroModel(CUIEvent uiEvent)
		{
			this.selectHeroId = uiEvent.m_eventParams.heroId;
			this.heroDescTxt.set_text(string.Empty);
			this.selectHeroBtn.CustomSetActive(true);
		}

		private void InitHeroPanel()
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(7u);
			this.CreateHeroPreview(dataByKey.dwConfValue, 0);
			ResGlobalInfo dataByKey2 = GameDataMgr.globalInfoDatabin.GetDataByKey(8u);
			this.CreateHeroPreview(dataByKey2.dwConfValue, 1);
			ResGlobalInfo dataByKey3 = GameDataMgr.globalInfoDatabin.GetDataByKey(9u);
			this.CreateHeroPreview(dataByKey3.dwConfValue, 2);
		}

		private void CreateHeroPreview(uint heroId, int i)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(HeroChooseLogic.s_heroInitChooseFormPath);
			if (form == null)
			{
				return;
			}
			string name = string.Format("RawImage{0}", i);
			GameObject gameObject = form.transform.Find(name).gameObject;
			CUIRawImageScript component = gameObject.GetComponent<CUIRawImageScript>();
			ObjData hero3DObj = CUICommonSystem.GetHero3DObj(heroId, true);
			if (hero3DObj.Object == null)
			{
				return;
			}
			component.AddGameObject(name, hero3DObj.Object, Vector3.zero, Quaternion.identity, hero3DObj.Object.transform.localScale);
			this.cacheObjList.Add(hero3DObj.Object);
			CUIEventScript cUIEventScript = gameObject.GetComponent<CUIEventScript>();
			if (cUIEventScript == null)
			{
				cUIEventScript = gameObject.AddComponent<CUIEventScript>();
				cUIEventScript.Initialize(form);
			}
			cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Hero_Init_Select, new stUIEventParams
			{
				heroId = heroId
			});
		}
	}
}
