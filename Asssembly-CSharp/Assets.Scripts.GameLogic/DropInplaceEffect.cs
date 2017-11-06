using System;

namespace Assets.Scripts.GameLogic
{
	public class DropInplaceEffect : IDropDownEffect
	{
		private DropItem Item;

		private VInt3 InitPos;

		public bool isFinished
		{
			get
			{
				return true;
			}
		}

		public VInt3 location
		{
			get
			{
				return this.InitPos;
			}
		}

		public DropInplaceEffect(VInt3 InPos)
		{
			this.InitPos = InPos;
		}

		public void Bind(DropItem item)
		{
			this.Item = item;
			DebugHelper.Assert(this.Item != null);
			this.Item.SetLocation(this.InitPos);
		}

		public void OnUpdate(int delta)
		{
		}
	}
}
