using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class NewbieWeakGuideControl : Singleton<NewbieWeakGuideControl>
{
	private const float TYR_TO_ADD_EFFECT_TIME = 5f;

	private NewbieWeakGuideImpl guideImpl;

	public DictionaryView<uint, GameObject> mEffectCache;

	private Dictionary<uint, int> mWeakGuideTriggerTime;

	public NewbieWeakGuideMainLineConf curWeakMainLineConf;

	private ListView<NewbieGuideWeakConf> mConfList;

	private uint m_CurStep;

	private ListView<NewbieGuideWeakConf> mToaddConfList;

	private NewbieGuideWeakConf curToAddEffectConf;

	private float tryToAddEffectTime;

	public bool isGuiding
	{
		get;
		private set;
	}

	private string logTitle
	{
		get
		{
			return "[<color=cyan>新手弱引导</color>][<color=green>eason</color>]";
		}
	}

	public void OpenGuideForm()
	{
		if (this.guideImpl != null)
		{
			this.guideImpl.OpenGuideForm();
		}
	}

	public void CloseGuideForm()
	{
		if (this.guideImpl != null)
		{
			this.guideImpl.CloseGuideForm();
		}
	}

	public override void Init()
	{
		base.Init();
		this.mEffectCache = new DictionaryView<uint, GameObject>();
		this.guideImpl = new NewbieWeakGuideImpl();
		this.mToaddConfList = new ListView<NewbieGuideWeakConf>();
		this.mWeakGuideTriggerTime = new Dictionary<uint, int>();
		this.guideImpl.Init();
		List<uint> weakMianLineIDList = Singleton<NewbieGuideDataManager>.GetInstance().GetWeakMianLineIDList();
		int count = weakMianLineIDList.get_Count();
		for (int i = 0; i < count; i++)
		{
			this.mWeakGuideTriggerTime.Add(weakMianLineIDList.get_Item(i), 0);
		}
	}

	public override void UnInit()
	{
		this.RemoveAllEffect();
		this.CloseGuideForm();
		this.guideImpl.UnInit();
		this.guideImpl = null;
		this.mToaddConfList.Clear();
		this.mToaddConfList = null;
		base.UnInit();
	}

	public void Update()
	{
		if (this.guideImpl != null)
		{
			this.guideImpl.Update();
		}
		if (this.mToaddConfList != null && this.mToaddConfList.Count > 0)
		{
			this.checkTryToAddEffectTimeOut();
			this.AddEffectProcess();
		}
	}

	public bool TriggerWeakGuide(uint mainLineId, uint startIndex = 1u)
	{
		NewbieWeakGuideMainLineConf newbieWeakGuideMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieWeakGuideMainLineConf(mainLineId);
		int num;
		if (this.mWeakGuideTriggerTime.TryGetValue(newbieWeakGuideMainLineConf.dwID, ref num) && CRoleInfo.GetCurrentUTCTime() - num < (int)newbieWeakGuideMainLineConf.bCDTime)
		{
			return true;
		}
		if (this.isGuiding)
		{
			this.RemoveAllEffect();
		}
		this.clear();
		this.curWeakMainLineConf = newbieWeakGuideMainLineConf;
		this.isGuiding = true;
		this.mConfList = Singleton<NewbieGuideDataManager>.GetInstance().GetWeakScriptList(mainLineId);
		this.m_CurStep = startIndex;
		this.OpenGuideForm();
		if (this.mConfList == null && this.curWeakMainLineConf != null)
		{
			this.CompleteAll();
		}
		else
		{
			this.CheckNext();
			this.mWeakGuideTriggerTime.set_Item(mainLineId, CRoleInfo.GetCurrentUTCTime());
		}
		return true;
	}

	private void CheckNext()
	{
		int count = this.mConfList.Count;
		if ((ulong)this.m_CurStep <= (ulong)((long)count))
		{
			NewbieGuideWeakConf conf = this.mConfList[(int)(this.m_CurStep - 1u)];
			this.AddEffect(conf);
		}
		else
		{
			this.CompleteAll();
		}
	}

	private void CompleteAll()
	{
		if (this.curWeakMainLineConf.bOnlyOnce > 0)
		{
			MonoSingleton<NewbieGuideManager>.GetInstance().SetWeakGuideComplete(this.curWeakMainLineConf.dwID, true, true);
		}
		MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.preNewBieWeakGuideComleteAll, new uint[]
		{
			this.curWeakMainLineConf.dwID
		});
		this.RemoveAllEffect();
		this.clear();
	}

	private void clear()
	{
		this.curWeakMainLineConf = null;
		this.isGuiding = false;
		this.mToaddConfList.Clear();
	}

	private void checkTryToAddEffectTimeOut()
	{
		this.tryToAddEffectTime += Time.deltaTime;
		if (this.tryToAddEffectTime >= 5f)
		{
			this.mToaddConfList.Clear();
		}
	}

	private bool AddEffect(NewbieGuideWeakConf conf)
	{
		if (this.HasEffect(conf.dwID))
		{
			return false;
		}
		this.curToAddEffectConf = conf;
		this.mToaddConfList.Add(conf);
		this.tryToAddEffectTime = 0f;
		return true;
	}

	private void AddEffectProcess()
	{
		GameObject gameObject = null;
		if (this.curToAddEffectConf == null)
		{
			return;
		}
		if (!this.mEffectCache.TryGetValue(this.curToAddEffectConf.dwID, out gameObject))
		{
			bool flag = this.guideImpl.AddEffect(this.curToAddEffectConf, this, out gameObject);
			if (flag)
			{
				this.mToaddConfList.RemoveAt(0);
				if (gameObject != null)
				{
					this.TryToAddClickEvent(this.curToAddEffectConf.dwID, gameObject);
					this.mEffectCache.Add(this.curToAddEffectConf.dwID, gameObject);
				}
			}
		}
	}

	private bool HasEffect(uint weakGuideId)
	{
		if (this.mEffectCache != null && this.mEffectCache.ContainsKey(weakGuideId))
		{
			if (this.mEffectCache[weakGuideId] != null)
			{
				return true;
			}
			this.mEffectCache.Remove(weakGuideId);
			this.guideImpl.ClearEffectText();
		}
		return false;
	}

	public void RemoveAllEffect()
	{
		List<uint> list = new List<uint>(this.mEffectCache.Keys);
		int count = list.get_Count();
		for (int i = 0; i < count; i++)
		{
			uint weakGuideId = list.get_Item(i);
			this.RemoveEffect(weakGuideId);
		}
	}

	private void RemoveEffect(uint weakGuideId)
	{
		GameObject gameObject = null;
		if (this.mEffectCache.TryGetValue(weakGuideId, out gameObject))
		{
			this.mEffectCache.Remove(weakGuideId);
			if (gameObject != null)
			{
				CUICommonSystem.DestoryObj(gameObject, 0.1f);
			}
			this.guideImpl.ClearEffectText();
		}
	}

	public void RemoveEffectByHilight(GameObject hilighter)
	{
		if (hilighter != null)
		{
			uint weakGuideId = 0u;
			CUIEventScript componentInParent = hilighter.transform.parent.GetComponentInParent<CUIEventScript>();
			if (componentInParent != null)
			{
				weakGuideId = componentInParent.m_onClickEventParams.weakGuideId;
				CUIEventScript cUIEventScript = componentInParent;
				cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
			}
			CUIMiniEventScript componentInParent2 = hilighter.transform.parent.GetComponentInParent<CUIMiniEventScript>();
			if (componentInParent2 != null)
			{
				weakGuideId = componentInParent2.m_onClickEventParams.weakGuideId;
				CUIMiniEventScript cUIMiniEventScript = componentInParent2;
				cUIMiniEventScript.onClick = (CUIMiniEventScript.OnUIEventHandler)Delegate.Remove(cUIMiniEventScript.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
			}
			this.RemoveEffect(weakGuideId);
		}
	}

	public void ForceCompleteWeakGuide()
	{
		if (this.curWeakMainLineConf != null)
		{
			this.CompleteAll();
		}
	}

	public void Complete(uint weakGuideId, uint nextStep = 0u)
	{
		NewbieGuideWeakConf newbieGuideWeakConf = this.mConfList[(int)(this.m_CurStep - 1u)];
		this.RemoveEffect(weakGuideId);
		if (newbieGuideWeakConf.Param[2] > 0u)
		{
			this.CompleteAll();
			return;
		}
		if (nextStep == 0u)
		{
			this.m_CurStep += 1u;
		}
		else
		{
			this.m_CurStep = nextStep;
		}
		this.CheckNext();
	}

	private void TryToAddClickEvent(uint id, GameObject EffectObj)
	{
		if (EffectObj == null)
		{
			return;
		}
		CUIEventScript componentInParent = EffectObj.GetComponentInParent<CUIEventScript>();
		if (componentInParent != null)
		{
			componentInParent.m_onClickEventParams.weakGuideId = id;
			CUIEventScript cUIEventScript = componentInParent;
			cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Combine(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		}
		CUIMiniEventScript componentInParent2 = EffectObj.GetComponentInParent<CUIMiniEventScript>();
		if (componentInParent2 != null)
		{
			componentInParent2.m_onClickEventParams.weakGuideId = id;
			CUIMiniEventScript cUIMiniEventScript = componentInParent2;
			cUIMiniEventScript.onClick = (CUIMiniEventScript.OnUIEventHandler)Delegate.Combine(cUIMiniEventScript.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
		}
	}

	private void ClickHandler(CUIEvent uiEvent)
	{
		uint weakGuideId = uiEvent.m_eventParams.weakGuideId;
		GameObject gameObject = null;
		this.mEffectCache.TryGetValue(uiEvent.m_eventParams.weakGuideId, out gameObject);
		if (gameObject == null)
		{
			return;
		}
		CUIEventScript componentInParent = gameObject.GetComponentInParent<CUIEventScript>();
		if (componentInParent != null)
		{
			CUIEventScript cUIEventScript = componentInParent;
			cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		}
		this.Complete(weakGuideId, 0u);
	}
}
