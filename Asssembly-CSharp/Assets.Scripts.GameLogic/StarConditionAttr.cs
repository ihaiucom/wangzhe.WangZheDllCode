using System;

namespace Assets.Scripts.GameLogic
{
	[StarCondition(2)]
	internal class StarConditionAttr : StarConditionProxy
	{
		protected static StarSystemFactory Factory = new StarSystemFactory(typeof(StarConditionAttrContextAttribute), typeof(IStarCondition));

		public virtual int attrID
		{
			get
			{
				return base.ConditionInfo.KeyDetail[0];
			}
		}

		public override IStarCondition CreateStarCondition()
		{
			IStarCondition starCondition = StarConditionAttr.Factory.Create(this.attrID) as IStarCondition;
			DebugHelper.Assert(starCondition != null, "can't create Attr id {0}", new object[]
			{
				this.attrID
			});
			if (starCondition != null)
			{
				starCondition.Initialize(base.ConditionInfo);
			}
			return starCondition;
		}
	}
}
