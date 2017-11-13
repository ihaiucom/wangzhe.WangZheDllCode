using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	[GameState]
	public class LoginState : BaseState
	{
		public override void OnStateEnter()
		{
			Singleton<CUIManager>.GetInstance().CloseAllForm(null, true, true);
			Singleton<ResourceLoader>.GetInstance().LoadScene("EmptyScene", new ResourceLoader.LoadCompletedDelegate(this.OnLoginSceneCompleted));
			Singleton<CSoundManager>.CreateInstance();
		}

		private void OnLoginSceneCompleted()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("Login_Play", null);
			Singleton<CLoginSystem>.GetInstance().Draw();
			if (GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CURMEMNOTENOUGH_TIPS) == 1u && !DeviceCheckSys.CheckAvailMemory() && DeviceCheckSys.GetRecordCurMemNotEnoughPopTimes() < 3)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("CheckDevice_QuitGame_CurMemNotEnough"), false);
				DeviceCheckSys.RecordCurMemNotEnoughPopTimes();
			}
		}

		public override void OnStateLeave()
		{
			base.OnStateLeave();
			Singleton<CLoginSystem>.GetInstance().CloseLogin();
			Debug.Log("CloseLogin...");
			Singleton<CResourceManager>.GetInstance().RemoveCachedResources(new enResourceType[]
			{
				enResourceType.BattleScene,
				enResourceType.UI3DImage,
				enResourceType.UIForm,
				enResourceType.UIPrefab,
				enResourceType.UISprite
			});
		}
	}
}
