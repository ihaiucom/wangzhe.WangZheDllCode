using System;

namespace com.tencent.pandora
{
	internal class DelegateGenerator
	{
		private ObjectTranslator translator;

		private Type delegateType;

		public DelegateGenerator(ObjectTranslator translator, Type delegateType)
		{
			this.translator = translator;
			this.delegateType = delegateType;
		}

		public object extractGenerated(IntPtr luaState, int stackPos)
		{
			return CodeGeneration.Instance.GetDelegate(this.delegateType, this.translator.getFunction(luaState, stackPos));
		}
	}
}
