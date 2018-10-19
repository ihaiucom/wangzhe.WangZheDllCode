using System;

namespace AGE
{
	public sealed class AssetReference : Attribute
	{
		public AssetRefType RefType
		{
			get;
			private set;
		}

		public AssetReference(AssetRefType refType)
		{
			this.RefType = refType;
		}
	}
}
