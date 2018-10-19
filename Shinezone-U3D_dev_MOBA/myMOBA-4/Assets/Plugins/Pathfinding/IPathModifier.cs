using System;

namespace Pathfinding
{
	public interface IPathModifier
	{
		int Priority
		{
			get;
			set;
		}

		ModifierData input
		{
			get;
		}

		ModifierData output
		{
			get;
		}

		void ApplyOriginal(Path p);

		void Apply(Path p, ModifierData source);

		void PreProcess(Path p);
	}
}
