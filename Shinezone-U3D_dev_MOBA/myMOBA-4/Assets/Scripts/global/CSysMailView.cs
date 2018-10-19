using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CSysMailView
{
	private CUIFormScript m_CUIForm;

	private CUIListScript m_CUIListScript;

	public CUIFormScript Form
	{
		set
		{
			this.m_CUIForm = value;
			this.m_CUIListScript = this.m_CUIForm.transform.FindChild("PanelAccess/List").GetComponent<CUIListScript>();
		}
	}

	public CMail Mail
	{
		set
		{
			this.Draw(value);
		}
	}

	public CustomMailType mailType
	{
		set
		{
			if (this.m_CUIForm != null)
			{
				if (value == CustomMailType.SYSTEM)
				{
					this.m_CUIForm.transform.FindChild("PanelAccess/GetAccess").GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Mail_SysAccess;
				}
				else if (value == CustomMailType.FRIEND)
				{
					this.m_CUIForm.transform.FindChild("PanelAccess/GetAccess").GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Mail_FriendAccess;
				}
			}
		}
	}

	private void Draw(CMail mail)
	{
		if (this.m_CUIForm != null)
		{
			this.m_CUIForm.transform.FindChild("PanelAccess").gameObject.CustomSetActive(true);
			Text component = this.m_CUIForm.transform.FindChild("PanelAccess/MailContent").GetComponent<Text>();
			Text component2 = this.m_CUIForm.transform.FindChild("PanelAccess/MailTitle").GetComponent<Text>();
			component.text = mail.mailContent;
			component2.text = mail.subject;
			this.m_CUIListScript.SetElementAmount(mail.accessUseable.Count);
			for (int i = 0; i < mail.accessUseable.Count; i++)
			{
				GameObject gameObject = this.m_CUIListScript.GetElemenet(i).transform.FindChild("itemCell").gameObject;
				CUICommonSystem.SetItemCell(this.m_CUIForm, gameObject, mail.accessUseable[i], true, true, false, false);
			}
			GameObject gameObject2 = this.m_CUIForm.transform.FindChild("PanelAccess/GetAccess").gameObject;
			gameObject2.CustomSetActive(mail.accessUseable.Count > 0);
			GameObject gameObject3 = this.m_CUIForm.transform.FindChild("PanelAccess/CheckAccess").gameObject;
			if (CHyperLink.Bind(gameObject3, mail.mailHyperlink))
			{
				gameObject3.CustomSetActive(true);
				gameObject2.CustomSetActive(false);
			}
			else
			{
				gameObject3.CustomSetActive(false);
			}
		}
	}
}
