using System;

namespace com.tencent.pandora
{
	public enum MetaOp
	{
		None,
		Add,
		Sub,
		Mul = 4,
		Div = 8,
		Eq = 16,
		Neg = 32,
		ALL = 63
	}
}
