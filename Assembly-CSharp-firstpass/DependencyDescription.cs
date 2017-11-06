using System;

public class DependencyDescription
{
	protected string[] Dpendencies;

	public int dependsIndex
	{
		get;
		protected set;
	}

	public DependencyDescription(int InIndex, string InValue)
	{
		this.dependsIndex = InIndex;
		this.Dpendencies = LinqS.Where(InValue.Split(new char[]
		{
			'|'
		}), (string x) => !string.IsNullOrEmpty(x.Trim()));
	}

	public bool ShouldBackOff(string InTest)
	{
		if (this.Dpendencies != null)
		{
			for (int i = 0; i < this.Dpendencies.Length; i++)
			{
				if (this.Dpendencies[i].Equals(InTest, 1))
				{
					return true;
				}
			}
		}
		return false;
	}
}
