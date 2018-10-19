using System;
using System.Collections.Generic;

public class CheatCommandsRepository : Singleton<CheatCommandsRepository>
{
	private CheatCommandGroup GeneralRepositories = new CheatCommandGroup();

	private DictionaryView<string, CheatCommandGroup> Repositories = new DictionaryView<string, CheatCommandGroup>();

	public DictionaryView<string, CheatCommandGroup> repositories
	{
		get
		{
			return this.Repositories;
		}
	}

	public CheatCommandGroup generalRepositories
	{
		get
		{
			return this.GeneralRepositories;
		}
	}

	public void RegisterCommand(ICheatCommand InCommand)
	{
		DebugHelper.Assert(InCommand != null && !this.HasCommand(InCommand.command.baseName));
		this.GeneralRepositories.Commands[InCommand.command.baseName.ToLower()] = InCommand;
		string[] groupHierarchies = InCommand.command.groupHierarchies;
		DebugHelper.Assert(groupHierarchies != null);
		string key = groupHierarchies[0];
		CheatCommandGroup cheatCommandGroup = null;
		if (!this.Repositories.TryGetValue(key, out cheatCommandGroup))
		{
			cheatCommandGroup = new CheatCommandGroup();
			this.Repositories[key] = cheatCommandGroup;
		}
		cheatCommandGroup.AddCommand(InCommand, 1);
	}

	public bool HasCommand(string InCommand)
	{
		return this.GeneralRepositories.Commands.ContainsKey(InCommand.ToLower());
	}

	public ICheatCommand FindCommand(string InCommand)
	{
		ICheatCommand result = null;
		this.GeneralRepositories.Commands.TryGetValue(InCommand.ToLower(), out result);
		return result;
	}

	public string ExecuteCommand(string InCommand, string[] InArgs)
	{
		if (this.HasCommand(InCommand))
		{
			return this.GeneralRepositories.Commands[InCommand.ToLower()].StartProcess(InArgs);
		}
		return "Command not found";
	}

	public ListView<ICheatCommand> FilterByString(string InPrefix)
	{
		DebugHelper.Assert(InPrefix != null);
		ListView<ICheatCommand> listView = new ListView<ICheatCommand>(16);
		DictionaryView<string, ICheatCommand>.Enumerator enumerator = this.GeneralRepositories.Commands.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, ICheatCommand> current = enumerator.Current;
			ICheatCommand value = current.Value;
			if (value.command.baseName.StartsWith(InPrefix, StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(InPrefix))
			{
				listView.Add(value);
			}
		}
		return listView;
	}
}
