using System;

public class CheatCommandGroup
{
	public DictionaryView<string, ICheatCommand> Commands = new DictionaryView<string, ICheatCommand>();

	public DictionaryView<string, CheatCommandGroup> ChildrenGroups = new DictionaryView<string, CheatCommandGroup>();

	public void AddCommand(ICheatCommand InCommand, int HierarchiesIndex)
	{
		DebugHelper.Assert(InCommand != null);
		string[] groupHierarchies = InCommand.command.groupHierarchies;
		DebugHelper.Assert(groupHierarchies != null);
		if (HierarchiesIndex < groupHierarchies.Length)
		{
			CheatCommandGroup cheatCommandGroup = null;
			if (!this.ChildrenGroups.TryGetValue(groupHierarchies[HierarchiesIndex], out cheatCommandGroup))
			{
				cheatCommandGroup = new CheatCommandGroup();
				this.ChildrenGroups.Add(groupHierarchies[HierarchiesIndex], cheatCommandGroup);
			}
			DebugHelper.Assert(cheatCommandGroup != null);
			cheatCommandGroup.AddCommand(InCommand, HierarchiesIndex + 1);
		}
		else
		{
			this.Commands.Add(InCommand.command.baseName, InCommand);
		}
	}
}
