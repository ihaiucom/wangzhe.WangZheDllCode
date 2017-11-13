using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	public class TriggerNewbieEquipTick : TickEvent
	{
		public enum eNewbieEuipTimeType
		{
			BuyFirstEquip,
			HasEnoughMoneyBuyEquip,
			HideEquipPanel,
			EquipShopIntroduce
		}

		public TriggerNewbieEquipTick.eNewbieEuipTimeType triggerTimeType;

		public bool bHide;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			TriggerNewbieEquipTick triggerNewbieEquipTick = ClassObjPool<TriggerNewbieEquipTick>.Get();
			triggerNewbieEquipTick.CopyData(this);
			return triggerNewbieEquipTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerNewbieEquipTick triggerNewbieEquipTick = src as TriggerNewbieEquipTick;
			this.triggerTimeType = triggerNewbieEquipTick.triggerTimeType;
			this.bHide = triggerNewbieEquipTick.bHide;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			switch (this.triggerTimeType)
			{
			case TriggerNewbieEquipTick.eNewbieEuipTimeType.BuyFirstEquip:
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onBuyFirstEquip, new uint[0]);
				break;
			case TriggerNewbieEquipTick.eNewbieEuipTimeType.HasEnoughMoneyBuyEquip:
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onHasEnoughMoneyButEquip, new uint[0]);
				break;
			case TriggerNewbieEquipTick.eNewbieEuipTimeType.HideEquipPanel:
				Singleton<CBattleGuideManager>.GetInstance().ShowBuyEuipPanel(!this.bHide);
				break;
			case TriggerNewbieEquipTick.eNewbieEuipTimeType.EquipShopIntroduce:
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEuipShopGuide, new uint[0]);
				break;
			}
		}
	}
}
