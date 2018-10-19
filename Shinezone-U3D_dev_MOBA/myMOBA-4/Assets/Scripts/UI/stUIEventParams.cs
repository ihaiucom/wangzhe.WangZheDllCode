using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public struct stUIEventParams
	{
		public stSnsFriendEventParams snsFriendEventParams;

		public stItemGetInfoParams itemGetInfoParams;

		public stSkillTipParams skillTipParam;

		public stSkillPropertyPrams[] skillPropertyDesc;

		public stHeroSkinEventParams heroSkinParam;

		public CUseable iconUseable;

		public SkillSlotType m_skillSlotType;

		public enSelectGameType heroSelectGameType;

		public stSymbolEventParams symbolParam;

		public stSymbolTransformParams symbolTransParam;

		public stDianQuanBuyParam dianQuanBuyPar;

		public stOpenHeroFormParams openHeroFormPar;

		public stBattleEquipParams battleEquipPar;

		public stFriendHeroSkinParams friendHeroSkinPar;

		public uint heroId;

		public List<uint> tagList;

		public uint taskId;

		public uint weakGuideId;

		public int selectIndex;

		public int tag;

		public int tag2;

		public int tag3;

		public uint tagUInt;

		public string tagStr;

		public string tagStr1;

		public uint commonUInt32Param1;

		public ushort commonUInt16Param1;

		public ulong commonUInt64Param1;

		public ulong commonUInt64Param2;

		public int skillSlotId;

		public float sliderValue;

		public bool togleIsOn;

		public bool commonBool;

		public enUIEventID srcUIEventID;

		public string pwd;

		public CUseableContainer useableContainer;

		public GameObject commonGameObject;
	}
}
