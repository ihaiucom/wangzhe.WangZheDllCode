using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class InBattleMsgUT
	{
		public static void ShowInBattleMsg(COM_INBATTLE_CHAT_TYPE type, ulong playerid, uint heroID, string content, string sound)
		{
			if (type == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_SIGNAL)
			{
				CSignalTips_InBatMsg obj = new CSignalTips_InBatMsg(playerid, heroID, content, sound);
				Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel().Add_SignalTip(obj);
				InBattleMsgUT.PlaySound(sound, playerid);
			}
			else if (type == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_BUBBLE)
			{
				InBattleMsgUT.ShowBubble(playerid, heroID, content);
				InBattleShortcut shortcutChat = Singleton<InBattleMsgMgr>.instance.m_shortcutChat;
				if (shortcutChat != null)
				{
					shortcutChat.UpdatePlayerBubbleTimer(playerid, heroID);
				}
				InBattleMsgUT.PlaySound(sound, playerid);
			}
		}

		private static void PlaySound(string sound, ulong playerid)
		{
			Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(playerid);
			if (playerByUid == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(sound) && playerByUid.Captain)
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound(sound, playerByUid.Captain, playerByUid.Captain.handle.gameObject);
			}
		}

		public static void ShowBubble(ulong playerid, uint heroID, string content)
		{
			Player playerByUid = Singleton<GamePlayerCenter>.instance.GetPlayerByUid(playerid);
			if (playerByUid == null)
			{
				return;
			}
			ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = playerByUid.GetAllHeroes();
			for (int i = 0; i < allHeroes.Count; i++)
			{
				if (allHeroes[i].handle != null && (long)allHeroes[i].handle.TheActorMeta.ConfigId == (long)((ulong)heroID))
				{
					allHeroes[i].handle.HudControl.SetTextHud(content, 12, 0, true);
					return;
				}
			}
		}
	}
}
