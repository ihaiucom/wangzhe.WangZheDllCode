using System;

public enum JoinMode : byte
{
	Default,
	CreateIfNotExists,
	JoinOrRejoin,
	RejoinOnly
}
