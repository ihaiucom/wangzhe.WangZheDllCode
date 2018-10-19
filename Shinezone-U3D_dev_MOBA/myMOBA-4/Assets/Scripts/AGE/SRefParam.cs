using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	public class SRefParam
	{
		public enum ValType
		{
			Bool,
			Int,
			UInt,
			Float,
			VInt3,
			Vector3,
			Quaternion,
			Object,
			ActorRoot
		}

		public bool dirty;

		public SRefParam.ValType type = SRefParam.ValType.Object;

		public SUnion union = default(SUnion);

		public object obj;

		public PoolObjHandle<ActorRoot> handle
		{
			get
			{
				return new PoolObjHandle<ActorRoot>
				{
					_handleObj = (this.obj as ActorRoot),
					_handleSeq = this.union._uint
				};
			}
			set
			{
				this.obj = value._handleObj;
				this.union._uint = value._handleSeq;
			}
		}

		public SRefParam Clone()
		{
			SRefParam sRefParam = SObjPool<SRefParam>.New();
			sRefParam.type = this.type;
			sRefParam.dirty = false;
			if (this.type < SRefParam.ValType.Object)
			{
				sRefParam.union._quat = this.union._quat;
			}
			else
			{
				sRefParam.obj = this.obj;
				if (this.type == SRefParam.ValType.ActorRoot)
				{
					sRefParam.union._uint = this.union._uint;
				}
			}
			return sRefParam;
		}

		public void Destroy()
		{
			this.obj = null;
			SObjPool<SRefParam>.Delete(this);
		}
	}
}
