using System;

namespace apollo_talker
{
	public class apollo_talkerMacros
	{
		public const int APOLLO_MAX_CMD_LEN = 64;

		public const int DOMAIN_COUNT = 256;

		public const int DATA_FMT_MASK = 16;

		public const int DATA_FLOW_MASK = 8;

		public const int DATA_TYPE_MASK = 7;

		public const int DOMAIN_APP = 1;

		public const int DOMAIN_TSS = 255;

		public const int CMD_FMT_NIL = 0;

		public const int CMD_FMT_STR = 1;

		public const int CMD_FMT_INT = 2;

		public const int DATA_FMT_MSG = 0;

		public const int DATA_FMT_BIN = 16;

		public const int DATA_FLOW_UP = 0;

		public const int DATA_FLOW_DOWN = 8;

		public const int DATA_TYPE_NOTICE = 0;

		public const int DATA_TYPE_REQUEST = 1;

		public const int DATA_TYPE_RESPONSE = 2;
	}
}
