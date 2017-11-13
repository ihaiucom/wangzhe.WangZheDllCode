using System;

public class MoveDirectionState
{
	public const int theshold = 990268;

	public bool enabled;

	public bool applied;

	public VInt3 adjDir;

	public VInt3 firstAdjDir;

	public VInt3 curAdjDir;

	public VInt3 firstDir;

	public VInt3 curDir;

	public void Reset()
	{
		this.enabled = false;
		this.applied = false;
		this.adjDir = VInt3.zero;
		this.firstAdjDir = VInt3.zero;
		this.curAdjDir = VInt3.zero;
		this.firstDir = VInt3.zero;
		this.curDir = VInt3.zero;
	}

	public void SetNewDirection(ref VInt3 dir)
	{
		bool flag = false;
		if (this.enabled)
		{
			if (VInt3.Dot(ref this.firstDir, ref dir) < 990268)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			this.adjDir = dir;
			this.firstAdjDir = VInt3.zero;
			this.curAdjDir = VInt3.zero;
			this.firstDir = VInt3.zero;
			this.enabled = false;
		}
		this.curDir = dir;
	}

	public void BeginMove()
	{
		this.applied = false;
	}

	public void EndMove()
	{
		if (!this.applied)
		{
			this.enabled = false;
		}
	}

	public bool Equals(MoveDirectionState other)
	{
		return this.enabled == other.enabled && this.applied == other.applied && this.adjDir == other.adjDir && this.curAdjDir == other.curAdjDir && this.curDir == other.curDir && this.firstAdjDir == other.firstAdjDir && this.firstDir == other.firstDir;
	}

	public override bool Equals(object obj)
	{
		return obj != null && base.GetType() == obj.GetType() && this.Equals((MoveDirectionState)obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
