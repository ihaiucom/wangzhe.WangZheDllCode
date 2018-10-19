using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BufferMark : BaseSkill
	{
		private uint markType;

		private int curLayer;

		private int immuneTime;

		private int curTime;

		private int lastMaxTime;

		private int triggerCDTime;

		private PoolObjHandle<ActorRoot> sourceActor;

		private PoolObjHandle<ActorRoot> originActor;

		public ResSkillMarkCfgInfo cfgData;

		private BufferMarkParticle markParticle;

		public BufferMark(int _markID)
		{
			this.SkillID = _markID;
			this.cfgData = GameDataMgr.skillMarkDatabin.GetDataByKey((long)_markID);
			this.markParticle = default(BufferMarkParticle);
		}

		private bool CheckTargetType(uint typeMask)
		{
			if (typeMask == 0u)
			{
				return true;
			}
			if (this.sourceActor)
			{
				int actorType = (int)this.sourceActor.handle.TheActorMeta.ActorType;
				if (((ulong)typeMask & (ulong)(1L << (actorType & 31))) > 0uL)
				{
					return true;
				}
			}
			return false;
		}

		public uint GetMarkType()
		{
			return this.markType;
		}

		public void SetCurLayer(int _layer)
		{
			this.curLayer = _layer;
			if (this.cfgData != null && this.CheckTargetType(this.cfgData.dwEffectMask))
			{
				this.markParticle.PlayParticle(ref this.originActor, ref this.sourceActor, this.curLayer);
			}
		}

		public int GetCurLayer()
		{
			return this.curLayer;
		}

		public void Init(BuffHolderComponent _buffHolder, PoolObjHandle<ActorRoot> _originator, uint _markType)
		{
			this.sourceActor = _buffHolder.actorPtr;
			this.originActor = _originator;
			if (this.cfgData != null)
			{
				this.ActionName = StringHelper.UTF8BytesToString(ref this.cfgData.szActionName);
				this.bAgeImmeExcute = (this.cfgData.bAgeImmeExcute == 1);
			}
			this.markParticle.Init(this.cfgData);
			this.SetCurLayer(1);
			this.immuneTime = 0;
			this.triggerCDTime = 0;
			this.curTime = 0;
			this.markType = _markType;
			this.lastMaxTime = ((this.cfgData.iLastMaxTime != 0) ? this.cfgData.iLastMaxTime : 2147483647);
		}

		public void UpdateLogic(int nDelta)
		{
			if (this.immuneTime > 0)
			{
				this.immuneTime -= nDelta;
				this.immuneTime = ((this.immuneTime <= 0) ? 0 : this.immuneTime);
			}
			if (this.triggerCDTime > 0)
			{
				this.triggerCDTime -= nDelta;
				this.triggerCDTime = ((this.triggerCDTime <= 0) ? 0 : this.triggerCDTime);
			}
			this.curTime += nDelta;
			if (this.curTime >= this.lastMaxTime)
			{
				this.SetCurLayer(0);
			}
		}

		public void AddLayer(int _addLayer)
		{
			int num = this.curLayer;
			num += _addLayer;
			num = ((num <= this.cfgData.iMaxLayer) ? num : this.cfgData.iMaxLayer);
			this.SetCurLayer(num);
			this.curTime = 0;
		}

		public void DecLayer(int _decLayer)
		{
			int num = this.curLayer;
			num -= _decLayer;
			num = ((num <= 0) ? 0 : num);
			this.SetCurLayer(num);
		}

		public override void OnActionStoped(ref PoolObjHandle<AGE.Action> action)
		{
			action.handle.onActionStop -= new ActionStopDelegate(this.OnActionStoped);
			if (!this.curAction)
			{
				return;
			}
			if (action == this.curAction)
			{
				this.curAction.Release();
			}
		}

		private void TriggerAction(PoolObjHandle<ActorRoot> _originator, SkillUseContext inUseContext, int triggerLayer)
		{
			SkillUseParam skillUseParam = default(SkillUseParam);
			skillUseParam.Init();
			skillUseParam.SetOriginator(_originator);
			skillUseParam.TargetActor = this.sourceActor;
			if (inUseContext != null)
			{
				skillUseParam.bExposing = inUseContext.bExposing;
				skillUseParam.uiFromId = inUseContext.uiFromId;
				skillUseParam.skillUseFrom = inUseContext.skillUseFrom;
				skillUseParam.MarkCount = triggerLayer;
			}
			this.Use(_originator, ref skillUseParam);
		}

		public void Trigger(PoolObjHandle<ActorRoot> _originator, SkillUseContext inUseContext)
		{
			int triggerLayer = 0;
			if (this.immuneTime == 0)
			{
				if (this.cfgData != null && this.cfgData.bLayerEffect == 1)
				{
					triggerLayer = this.curLayer;
				}
				this.DecLayer(this.cfgData.iCostLayer);
				this.immuneTime = this.cfgData.iImmuneTime;
				this.TriggerAction(_originator, inUseContext, triggerLayer);
			}
		}

		public void AutoTrigger(PoolObjHandle<ActorRoot> _originator, SkillUseContext inUseContext)
		{
			if (this.curLayer + 1 >= this.cfgData.iMaxLayer && this.cfgData.bAutoTrigger == 1)
			{
				if (this.triggerCDTime == 0)
				{
					this.AddLayer(1);
					this.triggerCDTime = this.cfgData.iCDTime;
					this.Trigger(_originator, inUseContext);
				}
				else
				{
					this.curTime = 0;
				}
			}
			else
			{
				this.AddLayer(1);
			}
		}

		public void UpperTrigger(PoolObjHandle<ActorRoot> _originator, SkillUseContext inUseContext)
		{
			if (this.curLayer >= this.cfgData.iTriggerLayer && this.cfgData.iTriggerLayer > 0)
			{
				this.Trigger(_originator, inUseContext);
			}
		}
	}
}
