using Assets.Scripts.UI;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class PopupMenuListSys : Singleton<PopupMenuListSys>
	{
		public struct PopupMenuListItem
		{
			public delegate void Show(string content);

			public PopupMenuListSys.PopupMenuListItem.Show m_show;

			public string content;
		}

		public List<PopupMenuListSys.PopupMenuListItem> m_popupMenuList = new List<PopupMenuListSys.PopupMenuListItem>();

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MENU_PopupMenuFinish, new CUIEventManager.OnUIEventHandler(this.OnPopupMenuFinish));
		}

		public void OnPopupMenuFinish(CUIEvent evt)
		{
			if (this.m_popupMenuList != null && this.m_popupMenuList.Count != 0)
			{
				this.m_popupMenuList.RemoveAt(0);
				this.PopupMenuListStart();
			}
		}

		public void PopupMenuListStart()
		{
			if (this.m_popupMenuList != null && this.m_popupMenuList.Count != 0)
			{
				this.m_popupMenuList[0].m_show(this.m_popupMenuList[0].content);
			}
		}

		public void AddItem(PopupMenuListSys.PopupMenuListItem item)
		{
			this.m_popupMenuList.Add(item);
		}

		public void ClearAll()
		{
			this.m_popupMenuList.Clear();
		}
	}
}
