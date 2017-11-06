using System;

namespace com.tencent.gsdk
{
	public class KartinRet
	{
		public int flag = -1;

		public string desc = "解析错误";

		public string tag = string.Empty;

		public int jump_network = -1;

		public int jump_signal = -1;

		public int jump_router = -1;

		public int router_status = -1;

		public string router_desc = string.Empty;

		public int jump_export = -1;

		public int export_status = -1;

		public string export_desc = string.Empty;

		public int jump_terminal = -1;

		public int terminal_status = -1;

		public string terminal_desc = string.Empty;

		public int jump_proxy = -1;

		public int jump_edge = -1;

		public string signal_desc = string.Empty;

		public int signal_status = -1;

		public KartinRet()
		{
		}

		public KartinRet(string _tag, int _flag, string _desc, int _jump_network, int _jump_signal, int _jump_router, int _router_status, string _router_desc, int _jump_export, int _export_status, string _export_desc, int _jump_terminal, int _terminal_status, string _terminal_desc, int _jump_proxy, int _jump_edge, string _signal_desc, int _signal_status)
		{
			this.tag = _tag;
			this.flag = _flag;
			this.desc = _desc;
			this.jump_network = _jump_network;
			this.jump_signal = _jump_signal;
			this.jump_router = _jump_router;
			this.router_status = _router_status;
			this.router_desc = _router_desc;
			this.jump_export = _jump_export;
			this.export_status = _export_status;
			this.export_desc = _export_desc;
			this.jump_terminal = _jump_terminal;
			this.terminal_status = _terminal_status;
			this.terminal_desc = _terminal_desc;
			this.jump_proxy = _jump_proxy;
			this.jump_edge = _jump_edge;
			this.signal_desc = _signal_desc;
			this.signal_status = _signal_status;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"tag:",
				this.tag,
				",flag:",
				this.flag,
				",desc:",
				this.desc,
				",detail(",
				this.jump_network,
				",",
				this.jump_signal,
				",",
				this.signal_status,
				",",
				this.signal_desc,
				",",
				this.jump_router,
				",",
				this.router_status,
				",",
				this.router_desc,
				",",
				this.jump_export,
				",",
				this.export_status,
				",",
				this.export_desc,
				",",
				this.jump_terminal,
				",",
				this.terminal_status,
				",",
				this.terminal_desc,
				",",
				this.jump_proxy,
				",",
				this.jump_edge,
				",)"
			});
		}
	}
}
