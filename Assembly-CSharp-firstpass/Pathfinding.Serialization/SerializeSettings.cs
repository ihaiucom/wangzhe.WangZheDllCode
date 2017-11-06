using System;

namespace Pathfinding.Serialization
{
	public class SerializeSettings
	{
		public bool nodes = true;

		public bool prettyPrint;

		public bool editorSettings;

		public static SerializeSettings Settings
		{
			get
			{
				return new SerializeSettings();
			}
		}

		public static SerializeSettings All
		{
			get
			{
				return new SerializeSettings
				{
					nodes = true
				};
			}
		}
	}
}
