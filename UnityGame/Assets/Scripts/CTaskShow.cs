using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class CTaskShow : MonoBehaviour
{
	public enum Type
	{
		None,
		Hide,
		Show,
		Hide_ShowNew
	}

	public CTaskShow.Type type;

	private CanvasGroup el_0;

	private float step = 0.03f;

	public bool bValid;

	public bool bForward = true;

	public CUIFormScript formScript;

	public uint taskid;

	private Action _action;

	private bool bFinish;

	private void Awake()
	{
		this.el_0 = base.gameObject.GetComponent<CanvasGroup>();
	}

	private void Start()
	{
		this.el_0 = base.gameObject.GetComponent<CanvasGroup>();
		this.Reset();
	}

	private void Update()
	{
		if (this.bValid)
		{
			if (this.type == CTaskShow.Type.Hide_ShowNew)
			{
				if (this.el_0.alpha >= 1f)
				{
					if (this.bFinish)
					{
						this.Stop();
					}
					else
					{
						this.PlayParticle(false);
						this._action = new Action(this.decrease);
					}
				}
				if (this.el_0.alpha <= 0f)
				{
					if (this.taskid > 0u && this.formScript != null)
					{
						this.step = 0.06f;
					}
					this.PlayParticle(true);
					this.bFinish = true;
					this._action = new Action(this.Increase);
				}
			}
			else if (this.type == CTaskShow.Type.Show)
			{
				if (this.el_0.alpha <= 0f)
				{
					this._action = new Action(this.Increase);
				}
				if (this.el_0.alpha >= 1f)
				{
					this.Stop();
				}
			}
			else if (this.type == CTaskShow.Type.Hide)
			{
				if (this.el_0.alpha >= 1f)
				{
					this._action = new Action(this.decrease);
				}
				if (this.el_0.alpha <= 0f)
				{
					this.Stop();
				}
			}
		}
		if (this.bValid && this._action != null)
		{
			this._action.Invoke();
		}
	}

	public void Stop()
	{
		this.step = 0.03f;
		this.bValid = false;
		this._action = null;
	}

	public void Reset()
	{
		this.el_0.alpha = (float)(this.bForward ? 1 : 0);
	}

	public void Play(CTaskShow.Type type, CUIFormScript formScript, uint newTaskid = 0u)
	{
		this.bValid = true;
		this.type = type;
		this.formScript = formScript;
		this.taskid = newTaskid;
		if (type == CTaskShow.Type.Hide_ShowNew)
		{
			this.bFinish = false;
			this.el_0.alpha = 1f;
		}
		else if (type == CTaskShow.Type.Show)
		{
			this.el_0.alpha = 0f;
		}
		else if (type == CTaskShow.Type.Hide)
		{
			this.el_0.alpha = 1f;
		}
	}

	private void Increase()
	{
		this.el_0.alpha += this.step;
	}

	private void decrease()
	{
		this.el_0.alpha -= this.step;
	}

	public void PlayParticle(bool bShowEffect)
	{
		string parPath = (!bShowEffect) ? "UGUI/Particle/UI_renwu_effect_02/UI_renwu_effect_02" : "UGUI/Particle/UI_renwu_effect_01/UI_renwu_effect_01";
		Singleton<CUIParticleSystem>.instance.AddParticle(parPath, 1.5f, base.gameObject, this.formScript, default(Quaternion?));
		string eventName = bShowEffect ? "UI_renwu_shuaxin" : "UI_renwu_xiaoshi";
		Singleton<CSoundManager>.instance.PostEvent(eventName, null);
	}
}
