using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class BackCityCom_3DUI
	{
		public static void ShowBack2City(PoolObjHandle<ActorRoot> actorHandle)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			if (theMinimapSys.MMiniMapBackCityCom_3Dui != null)
			{
				theMinimapSys.MMiniMapBackCityCom_3Dui.ShowBack2City_Imp(actorHandle);
			}
		}

		public static void HideBack2City(PoolObjHandle<ActorRoot> actorHandle)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			if (theMinimapSys.MMiniMapBackCityCom_3Dui != null)
			{
				theMinimapSys.MMiniMapBackCityCom_3Dui.HideBack2City_Imp(actorHandle);
			}
		}

		public void ShowBack2City_Imp(PoolObjHandle<ActorRoot> actorHandle)
		{
			this.SetActorReturnCityVisible(actorHandle, true);
		}

		public void HideBack2City_Imp(PoolObjHandle<ActorRoot> actorHandle)
		{
			this.SetActorReturnCityVisible(actorHandle, false);
		}

		private void SetActorReturnCityVisible(PoolObjHandle<ActorRoot> actorHandle, bool bShow)
		{
			if (!actorHandle)
			{
				return;
			}
			HudComponent3D hudControl = actorHandle.handle.HudControl;
			if (hudControl == null)
			{
				return;
			}
			if (hudControl.MapPointerSmall != null)
			{
				BackCityCom_3DUI.SetVisible(hudControl.MapPointerSmall, bShow);
			}
			if (hudControl.MapPointerBig != null)
			{
				BackCityCom_3DUI.SetVisible(hudControl.MapPointerBig, bShow);
			}
		}

		public static void SetVisible(GameObject node, bool bShow)
		{
			Transform transform = node.transform.Find("ReturCity");
			if (transform == null)
			{
				return;
			}
			transform.gameObject.CustomSetActive(bShow);
		}
	}
}
