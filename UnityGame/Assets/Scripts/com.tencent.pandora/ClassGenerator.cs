using System;

namespace com.tencent.pandora
{
	internal class ClassGenerator
	{
		private ObjectTranslator translator;

		private Type klass;

		public ClassGenerator(ObjectTranslator translator, Type klass)
		{
			this.translator = translator;
			this.klass = klass;
		}

		public object extractGenerated(IntPtr luaState, int stackPos)
		{
			return CodeGeneration.Instance.GetClassInstance(this.klass, this.translator.getTable(luaState, stackPos));
		}
	}
}
