using System;

namespace com.tencent.pandora
{
	internal struct MethodArgs
	{
		public int index;

		public ExtractValue extractValue;

		public bool isParamsArray;

		public Type paramsArrayType;
	}
}
