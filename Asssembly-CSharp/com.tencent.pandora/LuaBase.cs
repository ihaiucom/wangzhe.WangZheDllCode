using System;

namespace com.tencent.pandora
{
	public abstract class LuaBase : IDisposable
	{
		private bool _Disposed;

		protected int _Reference;

		protected LuaState _Interpreter;

		protected ObjectTranslator translator;

		public string name;

		private int count;

		public LuaBase()
		{
			this.count = 1;
		}

		~LuaBase()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void AddRef()
		{
			this.count++;
		}

		public void Release()
		{
			if (this._Disposed || this.name == null)
			{
				this.Dispose();
				return;
			}
			this.count--;
			if (this.count > 0)
			{
				return;
			}
			if (this.name != null)
			{
				LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(this._Interpreter.L);
				if (mgrFromLuaState != null)
				{
					mgrFromLuaState.RemoveLuaRes(this.name);
				}
			}
			this.Dispose();
		}

		public virtual void Dispose(bool disposeManagedResources)
		{
			if (!this._Disposed)
			{
				if (this._Reference != 0 && this._Interpreter != null)
				{
					if (disposeManagedResources)
					{
						this._Interpreter.dispose(this._Reference);
						this._Reference = 0;
					}
					else if (this._Interpreter.L != IntPtr.Zero)
					{
						LuaScriptMgr.refGCList.Enqueue(new LuaRef(this._Interpreter.L, this._Reference));
						this._Reference = 0;
					}
				}
				this._Interpreter = null;
				this._Disposed = true;
			}
		}

		protected void PushArgs(IntPtr L, object o)
		{
			LuaScriptMgr.PushVarObject(L, o);
		}

		public override bool Equals(object o)
		{
			if (o is LuaBase)
			{
				LuaBase luaBase = (LuaBase)o;
				return this._Interpreter.compareRef(luaBase._Reference, this._Reference);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._Reference;
		}
	}
}
