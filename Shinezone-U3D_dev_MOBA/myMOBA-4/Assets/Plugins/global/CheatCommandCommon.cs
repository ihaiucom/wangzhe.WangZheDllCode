using System;

public abstract class CheatCommandCommon : CheatCommandBase
{
	private CheatCommandAttribute CachedAttribute;

	public override CheatCommandName command
	{
		get
		{
			return this.CachedAttribute.command;
		}
	}

	public override string comment
	{
		get
		{
			return this.CachedAttribute.comment;
		}
	}

	public override ArgumentDescriptionAttribute[] argumentsTypes
	{
		get
		{
			return this.CachedAttribute.argumentsTypes;
		}
	}

	public override int messageID
	{
		get
		{
			return this.CachedAttribute.messageID;
		}
	}

	public CheatCommandCommon()
	{
		this.CacheAttribute();
		base.ValidateArgumentsBuffer();
	}

	protected void CacheAttribute()
	{
		object[] customAttributes = base.GetType().GetCustomAttributes(typeof(CheatCommandAttribute), false);
		DebugHelper.Assert(customAttributes != null && customAttributes.Length > 0);
		this.CachedAttribute = (customAttributes[0] as CheatCommandAttribute);
		DebugHelper.Assert(this.CachedAttribute != null);
		if (this.CachedAttribute != null)
		{
			customAttributes = base.GetType().GetCustomAttributes(typeof(ArgumentDescriptionAttribute), false);
			if (customAttributes != null)
			{
				this.CachedAttribute.IndependentInitialize(customAttributes);
			}
		}
	}
}
