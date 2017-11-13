using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MinimapSkillIndicator_3DUI
{
	private const int MaxNumberOfIndicator = 4;

	private GameObject mini_normalImgNode;

	private GameObject mini_redImgNode;

	private GameObject big_normalImgNode;

	private GameObject big_redImgNode;

	private Mask mini_maskCom;

	private Mask big_maskCom;

	private bool m_bEnable;

	private Vector2 m_dir = Vector2.zero;

	private Vector3 m_pos = Vector3.zero;

	private bool m_bDirDirty;

	private bool m_bPosDirty;

	private string[] sNormalImg;

	private string[] sRedImg;

	private string[] sAttackRangeImg;

	private string[] sAttackRedRangeImg;

	private float[] fSmallImgHeight;

	private float[] fSmallImgWidth;

	private float[] fBigImgHeight;

	private float[] fBigImgWidth;

	public SkillSlotType CurrentSlotType
	{
		get;
		private set;
	}

	public bool BIndicatorInited
	{
		get;
		private set;
	}

	public bool BAttackInited
	{
		get;
		private set;
	}

	public bool BDataInited
	{
		get;
		private set;
	}

	public static void AddIndicatorData(SkillSlotType slotType, string normalImg, string redImg, float smallImgHeight, float bigImgHeight)
	{
		MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
		if (theMinimapSys == null)
		{
			return;
		}
		if (theMinimapSys.MMinimapSkillIndicator_3Dui != null && !theMinimapSys.MMinimapSkillIndicator_3Dui.BDataInited)
		{
			theMinimapSys.MMinimapSkillIndicator_3Dui.AddInitData(slotType, normalImg, redImg, smallImgHeight, bigImgHeight);
		}
	}

	public static void UpdateIndicator(SkillSlotType slotType, ref Vector2 dir)
	{
		MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
		if (theMinimapSys == null)
		{
			return;
		}
		if (theMinimapSys.MMinimapSkillIndicator_3Dui == null)
		{
			return;
		}
		theMinimapSys.MMinimapSkillIndicator_3Dui.SetCurrentData(slotType);
		if (!theMinimapSys.MMinimapSkillIndicator_3Dui.BIndicatorInited && !theMinimapSys.MMinimapSkillIndicator_3Dui.BAttackInited)
		{
			return;
		}
		theMinimapSys.MMinimapSkillIndicator_3Dui.SetEnable(true, false);
		theMinimapSys.MMinimapSkillIndicator_3Dui.Update(ref dir);
	}

	public static void SetIndicator(SkillSlotType slotType, ref Vector3 forward)
	{
		MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
		if (theMinimapSys == null)
		{
			return;
		}
		if (theMinimapSys.MMinimapSkillIndicator_3Dui == null)
		{
			return;
		}
		theMinimapSys.MMinimapSkillIndicator_3Dui.SetCurrentData(slotType);
		if (!theMinimapSys.MMinimapSkillIndicator_3Dui.BIndicatorInited && !theMinimapSys.MMinimapSkillIndicator_3Dui.BAttackInited)
		{
			return;
		}
		theMinimapSys.MMinimapSkillIndicator_3Dui.SetEnable(true, false);
		theMinimapSys.MMinimapSkillIndicator_3Dui.SetIndicatorForward(ref forward);
	}

	public static void SetIndicatorColor(bool bNormal)
	{
		MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
		if (theMinimapSys == null)
		{
			return;
		}
		if (theMinimapSys.MMinimapSkillIndicator_3Dui == null || !theMinimapSys.MMinimapSkillIndicator_3Dui.BIndicatorInited)
		{
			return;
		}
		theMinimapSys.MMinimapSkillIndicator_3Dui.SetColor(bNormal);
	}

	public static void CancelIndicator()
	{
		MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
		if (theMinimapSys == null)
		{
			return;
		}
		if (theMinimapSys.MMinimapSkillIndicator_3Dui == null || !theMinimapSys.MMinimapSkillIndicator_3Dui.BIndicatorInited)
		{
			return;
		}
		theMinimapSys.MMinimapSkillIndicator_3Dui.SetEnable(false, true);
	}

	public void Clear()
	{
		this.mini_normalImgNode = (this.mini_redImgNode = null);
		this.big_normalImgNode = (this.big_redImgNode = null);
		this.mini_maskCom = (this.big_maskCom = null);
		this.mini_normalImgNode = (this.mini_redImgNode = null);
		this.big_normalImgNode = (this.big_redImgNode = null);
		this.mini_maskCom = (this.big_maskCom = null);
		this.sNormalImg = null;
		this.sRedImg = null;
		this.sAttackRangeImg = null;
		this.sAttackRedRangeImg = null;
		this.fSmallImgHeight = null;
		this.fSmallImgWidth = null;
		this.fBigImgHeight = null;
		this.fBigImgWidth = null;
		this.BIndicatorInited = false;
		this.BAttackInited = false;
		this.BDataInited = false;
	}

	public void Init(GameObject miniTrackNode, GameObject bigTrackNode)
	{
		if (miniTrackNode == null)
		{
			return;
		}
		if (bigTrackNode == null)
		{
			return;
		}
		this.mini_normalImgNode = miniTrackNode.transform.Find("normal").gameObject;
		this.mini_redImgNode = miniTrackNode.transform.Find("red").gameObject;
		this.big_normalImgNode = bigTrackNode.transform.Find("normal").gameObject;
		this.big_redImgNode = bigTrackNode.transform.Find("red").gameObject;
		this.BIndicatorInited = false;
		this.BAttackInited = false;
		this.BDataInited = false;
		this.mini_maskCom = miniTrackNode.GetComponent<Mask>();
		this.big_maskCom = bigTrackNode.GetComponent<Mask>();
		this.SetEnable(false, true);
		this.sNormalImg = new string[4];
		this.sRedImg = new string[4];
		this.fSmallImgHeight = new float[4];
		this.fSmallImgWidth = new float[4];
		this.fBigImgHeight = new float[4];
		this.fBigImgWidth = new float[4];
		if (this.mini_redImgNode != null)
		{
			this.mini_redImgNode.gameObject.CustomSetActive(false);
		}
		if (this.big_redImgNode != null)
		{
			this.big_redImgNode.gameObject.CustomSetActive(false);
		}
		this.CurrentSlotType = SkillSlotType.SLOT_SKILL_VALID;
	}

	public void AddInitData(SkillSlotType slotType, string normalImg, string redImg, float smallImgHeight, float bigImgHeight)
	{
		int num = slotType - SkillSlotType.SLOT_SKILL_1;
		if (num >= 0 && num < 4)
		{
			this.sNormalImg[num] = normalImg;
			this.sRedImg[num] = redImg;
			this.fSmallImgHeight[num] = smallImgHeight;
			this.fBigImgHeight[num] = bigImgHeight;
		}
		if (slotType == SkillSlotType.SLOT_SKILL_COUNT)
		{
			this.BDataInited = true;
		}
	}

	public void SetCurrentData(SkillSlotType slotType)
	{
		if (slotType != this.CurrentSlotType)
		{
			this.CurrentSlotType = slotType;
			int num = slotType - SkillSlotType.SLOT_SKILL_1;
			if (num >= 0 && num < this.sNormalImg.Length)
			{
				if (!string.IsNullOrEmpty(this.sNormalImg[num]) && !string.IsNullOrEmpty(this.sRedImg[num]))
				{
					this.mini_normalImgNode.GetComponent<Image>().SetSprite(this.sNormalImg[num], Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
					this.mini_redImgNode.GetComponent<Image>().SetSprite(this.sRedImg[num], Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
					this.big_normalImgNode.GetComponent<Image>().SetSprite(this.sNormalImg[num], Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
					this.big_redImgNode.GetComponent<Image>().SetSprite(this.sRedImg[num], Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
					this.SetWidthHeight(this.mini_normalImgNode, (this.fSmallImgWidth[num] > 0f) ? this.fSmallImgWidth[num] : 400f, this.fSmallImgHeight[num]);
					this.SetWidthHeight(this.mini_redImgNode, (this.fSmallImgWidth[num] > 0f) ? this.fSmallImgWidth[num] : 400f, this.fSmallImgHeight[num]);
					this.SetWidthHeight(this.big_normalImgNode, (this.fBigImgWidth[num] > 0f) ? this.fBigImgWidth[num] : 800f, this.fBigImgHeight[num]);
					this.SetWidthHeight(this.big_redImgNode, (this.fBigImgWidth[num] > 0f) ? this.fBigImgWidth[num] : 800f, this.fBigImgHeight[num]);
					this.BIndicatorInited = true;
				}
				else
				{
					this.BIndicatorInited = false;
				}
			}
		}
	}

	private void SetWidthHeight(GameObject obj, float width, float height)
	{
		if (obj == null)
		{
			return;
		}
		RectTransform rectTransform = obj.transform as RectTransform;
		if (rectTransform == null)
		{
			return;
		}
		rectTransform.sizeDelta = new Vector2(width, height);
	}

	public void SetEnable(bool bEnable, bool bForce = false)
	{
		if (bForce || (!bForce && this.m_bEnable != bEnable))
		{
			if (this.mini_maskCom != null)
			{
				this.mini_maskCom.enabled = bEnable;
			}
			if (this.big_maskCom != null)
			{
				this.big_maskCom.enabled = bEnable;
			}
			this.m_bEnable = bEnable;
			if (this.mini_normalImgNode != null)
			{
				this.mini_normalImgNode.transform.parent.gameObject.CustomSetActive(bEnable);
				this.mini_normalImgNode.gameObject.CustomSetActive(bEnable && this.BIndicatorInited);
			}
			if (this.big_normalImgNode != null)
			{
				this.big_normalImgNode.transform.parent.gameObject.CustomSetActive(bEnable);
				this.big_normalImgNode.gameObject.CustomSetActive(bEnable && this.BIndicatorInited);
			}
		}
	}

	public void ForceUpdate()
	{
		this.m_dir = Vector2.zero;
		this.m_pos = Vector3.zero;
	}

	public void Update()
	{
		MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
		if (theMinimapSys == null)
		{
			return;
		}
		Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
		if (hostPlayer == null || !hostPlayer.Captain)
		{
			return;
		}
		Vector3 vector = (Vector3)hostPlayer.Captain.handle.location;
		this.m_bPosDirty = (this.m_pos != vector);
		if (theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini)
		{
			if (this.m_bPosDirty)
			{
				this.UpdatePosition(this.mini_normalImgNode, ref vector, true);
				this.UpdatePosition(this.mini_redImgNode, ref vector, true);
				this.m_pos = vector;
			}
		}
		else if (theMinimapSys.CurMapType() == MinimapSys.EMapType.Big && this.m_bPosDirty)
		{
			this.UpdatePosition(this.big_normalImgNode, ref vector, false);
			this.UpdatePosition(this.big_redImgNode, ref vector, false);
			this.m_pos = vector;
		}
	}

	public void SetIndicatorForward(ref Vector3 forward)
	{
		float num = Mathf.Atan2(forward.z, forward.x) * 57.29578f;
		if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
		{
			num -= 180f;
		}
		Quaternion rotation = Quaternion.AngleAxis(num, Vector3.forward);
		if (this.mini_normalImgNode != null)
		{
			this.mini_normalImgNode.transform.rotation = rotation;
		}
		if (this.mini_redImgNode != null)
		{
			this.mini_redImgNode.transform.rotation = rotation;
		}
		if (this.big_normalImgNode != null)
		{
			this.big_normalImgNode.transform.rotation = rotation;
		}
		if (this.big_redImgNode != null)
		{
			this.big_redImgNode.transform.rotation = rotation;
		}
	}

	public void Update(ref Vector2 dir)
	{
		if (dir == Vector2.zero)
		{
			return;
		}
		MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
		if (theMinimapSys == null)
		{
			return;
		}
		Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
		if (hostPlayer == null || !hostPlayer.Captain)
		{
			return;
		}
		this.m_bDirDirty = (this.m_dir != dir);
		Vector3 vector = (Vector3)hostPlayer.Captain.handle.location;
		this.m_bPosDirty = (this.m_pos != vector);
		bool bSmallMap = theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
		if (this.m_bDirDirty)
		{
			this.UpdateRotation(this.mini_normalImgNode, ref dir);
			this.UpdateRotation(this.mini_redImgNode, ref dir);
			this.UpdateRotation(this.big_normalImgNode, ref dir);
			this.UpdateRotation(this.big_redImgNode, ref dir);
			this.m_dir = dir;
		}
		if (this.m_bPosDirty)
		{
			this.UpdatePosition(this.mini_normalImgNode, ref vector, bSmallMap);
			this.UpdatePosition(this.mini_redImgNode, ref vector, bSmallMap);
			this.UpdatePosition(this.big_normalImgNode, ref vector, bSmallMap);
			this.UpdatePosition(this.big_redImgNode, ref vector, bSmallMap);
			this.m_pos = vector;
		}
	}

	private void UpdateRotation(GameObject node, ref Vector2 dir)
	{
		float angle = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		node.transform.rotation = rotation;
	}

	private void UpdatePosition(GameObject node, ref Vector3 pos, bool bSmallMap)
	{
		if (node != null)
		{
			RectTransform rectTransform = node.transform as RectTransform;
			if (rectTransform != null)
			{
				if (bSmallMap)
				{
					rectTransform.anchoredPosition = new Vector2(pos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x, pos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y);
				}
				else
				{
					rectTransform.anchoredPosition = new Vector2(pos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.x, pos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.y);
				}
			}
		}
	}

	private void SetColor(bool bNormal)
	{
		if (!this.BIndicatorInited)
		{
			return;
		}
		if (Singleton<CBattleSystem>.GetInstance().TheMinimapSys == null)
		{
			return;
		}
		this.mini_normalImgNode.CustomSetActive(bNormal);
		this.mini_redImgNode.CustomSetActive(!bNormal);
		this.big_normalImgNode.CustomSetActive(bNormal);
		this.big_redImgNode.CustomSetActive(!bNormal);
	}
}
