using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CMail
	{
		public CustomMailType mailType;

		public byte subType;

		public int mailIndex;

		public COM_MAIL_STATE mailState;

		public bool autoDel;

		public string from;

		public string subject;

		public uint sendTime;

		public string mailContent;

		public string mailHyperlink;

		public ListView<CUseable> accessUseable = new ListView<CUseable>();

		public int accessUseableGeted;

		public bool isReceive;

		public bool isAccess;

		public ulong uid;

		public uint dwLogicWorldID;

		public byte relationType;

		public byte inviteType;

		public byte processType;

		public byte bMapType;

		public uint dwMapId;

		public uint dwGameSvrEntity;

		public int CanBeDeleted
		{
			get
			{
				int result = 0;
				if (this.mailState == COM_MAIL_STATE.COM_MAIL_HAVEREAD)
				{
					if (this.mailType != CustomMailType.ASK_FOR && this.accessUseable.Count > 0)
					{
						for (int i = 0; i < this.accessUseable.Count; i++)
						{
							if ((this.accessUseableGeted & 1 << i) == 1)
							{
								result = 10007;
								break;
							}
						}
					}
				}
				else
				{
					result = 10006;
				}
				return result;
			}
		}

		public CMail()
		{
		}

		public CMail(CustomMailType mailType, ref CSDT_GETMAIL_RES pkg)
		{
			this.mailType = mailType;
			this.subType = pkg.bMailType;
			this.mailIndex = pkg.iMailIndex;
			this.mailState = (COM_MAIL_STATE)pkg.bMailState;
			this.autoDel = (pkg.bAutoDel > 0);
			this.from = Utility.UTF8Convert(pkg.szFrom);
			this.sendTime = pkg.dwSendTime;
			this.subject = Utility.UTF8Convert(pkg.szSubject, (int)pkg.bSubjectLen);
			this.accessUseableGeted = 0;
			this.accessUseable.Clear();
			this.accessUseable = CMailSys.StAccessToUseable(pkg.astAccess, null, (int)pkg.bAccessCnt);
			for (int i = 0; i < this.accessUseable.Count; i++)
			{
				if (pkg.astAccess[i].bGeted == 1)
				{
					this.accessUseableGeted |= 1 << i;
				}
			}
		}

		public CMail(CustomMailType mailType, ref CSDT_ASKFORREQ_DETAIL pkg)
		{
			this.mailType = mailType;
			this.subType = (byte)mailType;
			this.mailIndex = pkg.iReqIndex;
			this.mailState = ((pkg.bIsRead <= 0) ? COM_MAIL_STATE.COM_MAIL_UNREAD : COM_MAIL_STATE.COM_MAIL_HAVEREAD);
			this.autoDel = false;
			this.from = Utility.UTF8Convert(pkg.stReqInfo.szAcntName);
			this.sendTime = pkg.stReqInfo.dwReqTime;
			COM_ITEM_TYPE wItemType = (COM_ITEM_TYPE)pkg.stReqInfo.stReqItem.wItemType;
			uint dwItemID = pkg.stReqInfo.stReqItem.dwItemID;
			int dwItemCnt = (int)pkg.stReqInfo.stReqItem.dwItemCnt;
			CUseable cUseable = CUseableManager.CreateUseable(wItemType, dwItemID, dwItemCnt);
			if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				this.subject = string.Format(Singleton<CTextManager>.GetInstance().GetText("HeroSkinBuy_Ask_For_Mail_Title_Skin"), cUseable.m_name);
			}
			else
			{
				this.subject = string.Format(Singleton<CTextManager>.GetInstance().GetText("HeroSkinBuy_Ask_For_Mail_Title_Hero"), cUseable.m_name);
			}
			if (pkg.stReqInfo.stMsgInfo.bMsgType == 1)
			{
				this.mailContent = StringHelper.UTF8BytesToString(ref pkg.stReqInfo.stMsgInfo.stMsgInfo.stMsgString.szContentStr);
				if (string.IsNullOrEmpty(this.mailContent))
				{
					this.mailContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("HeroSKinBuy_Ask_For_Default_Msg"), cUseable.m_name);
				}
			}
			else
			{
				uint dwMsgID = pkg.stReqInfo.stMsgInfo.stMsgInfo.stMsgIDInfo.dwMsgID;
				ulong num = Convert.ToUInt64(dwMsgID);
				num <<= 32;
				int num2 = Convert.ToInt32(wItemType);
				long key = (long)(num + (ulong)((long)num2));
				ResAskforTemplet dataByKey = GameDataMgr.askForTemplateDatabin.GetDataByKey(key);
				if (dataByKey == null)
				{
					this.mailContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("HeroSKinBuy_Ask_For_Default_Msg"), cUseable.m_name);
				}
				else
				{
					this.mailContent = string.Format(dataByKey.szContent, cUseable.m_name);
				}
			}
			this.accessUseableGeted = 0;
			this.accessUseable.Clear();
			this.accessUseable.Add(cUseable);
		}

		public void Read(CSDT_MAILOPTRES_READMAIL pkg)
		{
			this.isReceive = true;
			this.mailState = (COM_MAIL_STATE)pkg.bMailState;
			this.ParseContentAndHyperlink(pkg.szContent, (int)pkg.wContentLen, ref this.mailContent, ref this.mailHyperlink);
		}

		public void Read()
		{
			this.isReceive = true;
			this.mailState = COM_MAIL_STATE.COM_MAIL_HAVEREAD;
		}

		public void ParseContentAndHyperlink(sbyte[] srcContent, int srcContentLength, ref string content, ref string hyperlink)
		{
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < srcContentLength; i++)
			{
				if (srcContent[i].Equals(91))
				{
					num = i;
				}
				else if (srcContent[i].Equals(93))
				{
					num2 = i;
				}
			}
			if (0 < num && num < num2)
			{
				sbyte[] array = new sbyte[num];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = srcContent[j];
				}
				content = Utility.UTF8Convert(array, array.Length);
				sbyte[] array2 = new sbyte[num2 - num + 1 - 2];
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k] = srcContent[num + 1 + k];
				}
				hyperlink = Utility.UTF8Convert(array2, array2.Length);
			}
			if (!CHyperLink.IsStandCommond(hyperlink))
			{
				content = Utility.UTF8Convert(srcContent, srcContentLength);
				hyperlink = null;
			}
		}
	}
}
