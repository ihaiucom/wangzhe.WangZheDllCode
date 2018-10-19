using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NewbieGuideScriptControl : MonoBehaviour
{
	public delegate void NewbieGuideScriptControlDelegate();

	public delegate NewbieGuideBaseScript AddScriptDelegate(NewbieGuideScriptType type, GameObject gameObject);

	private const float TimeOut = 30f;

	private NewbieGuideMainLineConf curMainLineConf;

	public NewbieGuideScriptControl.AddScriptDelegate addScriptDelegate;

	private int mSavePoint;

	private int mCurrentScriptIndex;

	private NewbieGuideBaseScript mCurrentScript;

	private ListView<NewbieGuideScriptConf> mConfList;

	public static string FormGuideMaskPath = "UGUI/Form/System/Dialog/Form_GuideMask";

	private float m_timeOutTimer;

	public event NewbieGuideScriptControl.NewbieGuideScriptControlDelegate CompleteEvent;

	public event NewbieGuideScriptControl.NewbieGuideScriptControlDelegate SaveEvent;

	public uint currentMainLineId
	{
		get;
		private set;
	}

	public int savePoint
	{
		get
		{
			return this.mSavePoint;
		}
	}

	public int currentScriptIndex
	{
		get
		{
			return this.mCurrentScriptIndex;
		}
	}

	public NewbieGuideBaseScript currentScript
	{
		get
		{
			return this.mCurrentScript;
		}
	}

	public int startIndex
	{
		get;
		private set;
	}

	public static CUIFormScript FormGuideMask
	{
		get;
		private set;
	}

	private string logTitle
	{
		get
		{
			return "[<color=cyan>新手引导</color>][<color=green>eason</color>]";
		}
	}

	public void SetData(uint id, int startIndex)
	{
		this.currentMainLineId = id;
		this.startIndex = startIndex;
		this.curMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConf(this.currentMainLineId);
	}

	public static void OpenGuideForm()
	{
		if (NewbieGuideScriptControl.FormGuideMask == null)
		{
			NewbieGuideScriptControl.FormGuideMask = Singleton<CUIManager>.GetInstance().OpenForm(NewbieGuideScriptControl.FormGuideMaskPath, true, true);
			if (NewbieGuideScriptControl.FormGuideMask != null)
			{
				Transform transform = NewbieGuideScriptControl.FormGuideMask.transform.FindChild("GuideTextStatic");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}
	}

	public static void CloseGuideForm()
	{
		if (NewbieGuideScriptControl.FormGuideMask != null)
		{
			NewbieGuideScriptControl.FormGuideMask.transform.FindChild("GuideTextStatic").transform.gameObject.CustomSetActive(false);
			Singleton<CUIManager>.GetInstance().CloseForm(NewbieGuideScriptControl.FormGuideMask);
			NewbieGuideScriptControl.FormGuideMask = null;
		}
	}

	private void Start()
	{
		NewbieGuideScriptControl.OpenGuideForm();
		this.mConfList = Singleton<NewbieGuideDataManager>.GetInstance().GetScriptList(this.currentMainLineId);
		if (this.mConfList == null)
		{
			this.CompleteAll();
		}
		else
		{
			NewbieGuideMainLineConf newbieGuideMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConf(this.currentMainLineId);
			if (newbieGuideMainLineConf != null)
			{
				this.mSavePoint = newbieGuideMainLineConf.iSavePoint;
				if (this.startIndex > 0)
				{
					this.SetCurrentScriptIndex(this.startIndex - 1);
				}
				else
				{
					this.SetCurrentScriptIndex(0);
				}
				this.CheckNext();
			}
		}
	}

	private void Update()
	{
		if (MonoSingleton<NewbieGuideManager>.GetInstance().bTimeOutSkip && this.m_timeOutTimer > 0f)
		{
			this.m_timeOutTimer -= Time.deltaTime;
			if (this.m_timeOutTimer <= 0f)
			{
				this.m_timeOutTimer = 0f;
				MonoSingleton<NewbieGuideManager>.instance.ForceCompleteNewbieGuide();
			}
		}
	}

	public bool CheckSavePoint()
	{
		return 0 < this.mSavePoint && this.mCurrentScriptIndex + 1 >= this.mSavePoint;
	}

	private void SetCurrentScriptIndex(int value)
	{
		this.mCurrentScriptIndex = value;
	}

	private void CheckNext()
	{
		if (this.currentScriptIndex < this.mConfList.Count)
		{
			if (!Singleton<NetworkModule>.GetInstance().lobbySvr.connected && this.curMainLineConf.bIndependentNet != 1)
			{
				MonoSingleton<NewbieGuideManager>.instance.ForceCompleteNewbieGuide();
			}
			else
			{
				NewbieGuideScriptConf newbieGuideScriptConf = this.mConfList[this.currentScriptIndex];
				this.mCurrentScript = this.AddScript((NewbieGuideScriptType)newbieGuideScriptConf.wType);
				if (null != this.mCurrentScript)
				{
					this.mCurrentScript.SetData(newbieGuideScriptConf);
					this.mCurrentScript.CompleteEvent += new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.ScriptCompleteHandler);
					this.mCurrentScript.onCompleteAll += new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.CompleteAll);
				}
				else
				{
					this.CompleteAll();
				}
			}
		}
		else
		{
			this.CompleteAll();
		}
		if (this.mCurrentScript != null && this.mCurrentScript.IsTimeOutSkip())
		{
			this.m_timeOutTimer = 30f;
		}
		else
		{
			this.m_timeOutTimer = 0f;
		}
	}

	private void ScriptCheckSaveHandler()
	{
		this.CheckSave();
	}

	private void CheckSave()
	{
		if (this.SaveEvent != null)
		{
			this.SaveEvent();
		}
	}

	private void CompleteAll()
	{
		NewbieGuideScriptControl.CloseGuideForm();
		this.curMainLineConf = null;
		this.currentMainLineId = 0u;
		if (this.CompleteEvent != null)
		{
			this.CompleteEvent();
		}
	}

	private void DestroyCurrentScript()
	{
		if (null != this.mCurrentScript)
		{
			this.mCurrentScript.CompleteEvent -= new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.ScriptCompleteHandler);
			this.mCurrentScript.onCompleteAll -= new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.CompleteAll);
			UnityEngine.Object.Destroy(this.mCurrentScript);
			this.mCurrentScript = null;
		}
	}

	private void ScriptCompleteHandler()
	{
		this.ScriptCheckSaveHandler();
		this.DestroyCurrentScript();
		this.SetCurrentScriptIndex(this.currentScriptIndex + 1);
		this.CheckNext();
	}

	private NewbieGuideBaseScript AddScript(NewbieGuideScriptType type)
	{
		if (this.addScriptDelegate != null)
		{
			return this.addScriptDelegate(type, base.gameObject);
		}
		return null;
	}

	public void Stop()
	{
		if (this.mCurrentScript != null)
		{
			this.mCurrentScript.Stop();
		}
		this.DestroyCurrentScript();
		NewbieGuideScriptControl.CloseGuideForm();
	}
}
