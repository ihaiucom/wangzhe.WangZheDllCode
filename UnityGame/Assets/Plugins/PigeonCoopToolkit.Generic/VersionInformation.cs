using System;

namespace PigeonCoopToolkit.Generic
{
	[Serializable]
	public class VersionInformation
	{
		public string Name;

		public int Major = 1;

		public int Minor;

		public int Patch;

		public VersionInformation(string name, int major, int minor, int patch)
		{
			this.Name = name;
			this.Major = major;
			this.Minor = minor;
			this.Patch = patch;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}.{2}.{3}", new object[]
			{
				this.Name,
				this.Major,
				this.Minor,
				this.Patch
			});
		}

		public bool Match(VersionInformation other, bool looseMatch)
		{
			if (looseMatch)
			{
				return other.Name == this.Name && other.Major == this.Major && other.Minor == this.Minor;
			}
			return other.Name == this.Name && other.Major == this.Major && other.Minor == this.Minor && other.Patch == this.Patch;
		}
	}
}
