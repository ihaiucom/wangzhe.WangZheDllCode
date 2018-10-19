using System;

namespace tsf4g_tdr_csharp
{
	public class TdrError
	{

		public enum ErrorType
		{
			TDR_NO_ERROR,
			TDR_ERR_SHORT_BUF_FOR_WRITE = -1,
			TDR_ERR_SHORT_BUF_FOR_READ = -2,
			TDR_ERR_STR_LEN_TOO_BIG = -3,
			TDR_ERR_STR_LEN_TOO_SMALL = -4,
			TDR_ERR_STR_LEN_CONFLICT = -5,
			TDR_ERR_MINUS_REFER_VALUE = -6,
			TDR_ERR_REFER_SURPASS_COUNT = -7,
			TDR_ERR_ARG_IS_NULL = -8,
			TDR_ERR_CUTVER_TOO_SMALL = -9,
			TDR_ERR_CUTVER_CONFILICT = -10,
			TDR_ERR_PARSE_TDRIP_FAILED = -11,
			TDR_ERR_INVALID_TDRIP_VALUE = -12,
			TDR_ERR_INVALID_TDRTIME_VALUE = -13,
			TDR_ERR_INVALID_TDRDATE_VALUE = -14,
			TDR_ERR_INVALID_TDRDATETIME_VALUE = -15,
			TDR_ERR_FUNC_LOCALTIME_FAILED = -16,
			TDR_ERR_INVALID_HEX_STR_LEN = -17,
			TDR_ERR_INVALID_HEX_STR_FORMAT = -18,
			TDR_ERR_INVALID_BUFFER_PARAMETER = -19,
			TDR_ERR_NET_CUTVER_INVALID = -20,
			TDR_ERR_ACCESS_VILOATION_EXCEPTION = -21,
			TDR_ERR_ARGUMENT_NULL_EXCEPTION = -22,
			TDR_ERR_USE_HAVE_NOT_INIT_VARIABLE_ARRAY = -23,
			TDR_ERR_INVALID_FORMAT = -24,
			TDR_ERR_HAVE_NOT_SET_SIZEINFO = -25,
			TDR_ERR_VAR_STRING_LENGTH_CONFILICT = -26,
			TDR_ERR_VAR_ARRAY_CONFLICT = -27,
			TDR_ERR_BAD_TLV_MAGIC = -28,
			TDR_ERR_UNMATCHED_LENGTH = -29,
			TDR_ERR_UNION_SELECTE_FIELD_IS_NULL = -30,
			TDR_ERR_SUSPICIOUS_SELECTOR = -31,
			TDR_ERR_UNKNOWN_TYPE_ID = -32,
			TDR_ERR_LOST_REQUIRED_FIELD = -33
		}

		private static string[] errorTab = new string[]
		{
			"no error",
			"available free space in buffer is not enough",
			"available data in buffer is not enough",
			"string length surpass defined size",
			"string length smaller than min string length",
			"string sizeinfo inconsistent with real length",
			"reffer value can not be minus",
			"reffer value bigger than count or size",
			"pointer-type argument is NULL",
			"cut-version is smaller than base-version",
			"cut-version not covers entry refered by versionindicator",
			"inet_ntoa failed when parse tdr_ip_t",
			"value variable of tdr_ip_t is invalid",
			"value variable of tdr_time_t is invalid",
			"value variable of tdr_date_t is invalid",
			"value variable of tdr_datetime_t is invalid",
			"function 'localtime' or 'localtime_r' failed",
			"invalid hex-string length, must be an even number",
			"invalid hex-string format, each character must be a hex-digit",
			"NULL array as parameter",
			"cutVer from net-msg not in [BASEVERSION, CURRVERSION]",
			"assess voliation exception cause by ptr is null , or bad formation",
			"argument null exception cause by arguments is null",
			"variable array have not alloc memory,you must alloc memory before use",
			"invalid string format cause FORMATEXCETPION",
			"the meta have not set sizeinfo attribute ",
			"string/wstring length confilict with meta file",
			"array reference allocated not enough",
			"bad tlv encode magic",
			"unmatched length",
			"the selecte union meta is null, may be not construct",
			"invalid selector",
			"unknown type id",
			"at least 1 required field been lost"
		};

		public static string getErrorString(TdrError.ErrorType errorCode)
		{
			int num = (int)TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE * (int)errorCode;
			if (0 > num || num >= TdrError.errorTab.GetLength(0))
			{
				return TdrError.errorTab[0];
			}
			return TdrError.errorTab[num];
		}
	}
}
