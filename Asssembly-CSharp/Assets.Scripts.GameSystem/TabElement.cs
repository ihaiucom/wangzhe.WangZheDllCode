using System;

namespace Assets.Scripts.GameSystem
{
	public class TabElement
	{
		public enum Type
		{
			None,
			Config,
			SelfDef
		}

		public TabElement.Type type;

		public uint cfgId;

		public string configContent;

		public string selfDefContent;

		public byte camp;

		public TabElement(string selfDef = "")
		{
			this.type = TabElement.Type.SelfDef;
			this.cfgId = 0u;
			this.configContent = null;
			this.selfDefContent = selfDef;
		}

		public TabElement(uint cfgid, string configContent = "")
		{
			this.type = TabElement.Type.Config;
			this.cfgId = cfgid;
			this.configContent = configContent;
			this.selfDefContent = null;
		}

		public TabElement Clone()
		{
			return new TabElement(string.Empty)
			{
				type = this.type,
				cfgId = this.cfgId,
				configContent = this.configContent,
				selfDefContent = this.selfDefContent,
				camp = this.camp
			};
		}
	}
}
