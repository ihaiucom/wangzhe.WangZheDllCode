using System;
using System.Text;

namespace tsf4g_tdr_csharp
{
	public class TdrBufUtil
	{
		public static TdrError.ErrorType printMultiStr(ref TdrVisualBuf buf, string str, int times)
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			for (int i = 0; i < times; i++)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					str
				});
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					break;
				}
			}
			return errorType;
		}

		public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, bool withSep)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				if (withSep)
				{
					errorType = buf.sprintf("{0}{1}", new object[]
					{
						variable,
						sep
					});
				}
				else
				{
					errorType = buf.sprintf("{0}: ", new object[]
					{
						variable
					});
				}
			}
			return errorType;
		}

		public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, bool withSep)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				if (withSep)
				{
					errorType = buf.sprintf("{0}[{1:d}]{2}", new object[]
					{
						variable,
						arrIdx,
						sep
					});
				}
				else
				{
					errorType = buf.sprintf("{0}[{1:d}]: ", new object[]
					{
						variable,
						arrIdx
					});
				}
			}
			return errorType;
		}

		public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, string format, params object[] args)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}: ", new object[]
				{
					variable
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf(format, args);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, string format, params object[] args)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}[{1:d}]: ", new object[]
				{
					variable,
					arrIdx
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf(format, args);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printArray(ref TdrVisualBuf buf, int indent, char sep, string variable, long count)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}[0:{1:d}]: ", new object[]
				{
					variable,
					count
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printString(ref TdrVisualBuf buf, int indent, char sep, string variable, byte[] bStr)
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			string text = string.Empty;
			int count = TdrTypeUtil.cstrlen(bStr);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				text = Encoding.ASCII.GetString(bStr, 0, count);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}: {1}{2}", new object[]
				{
					variable,
					text,
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printWString(ref TdrVisualBuf buf, int indent, char sep, string variable, short[] str)
		{
			TdrError.ErrorType errorType = buf.sprintf("{0}:  ", new object[]
			{
				variable
			});
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				int num = TdrTypeUtil.wstrlen(str);
				for (int i = 0; i < num; i++)
				{
					errorType = buf.sprintf("0x{0:X4}", new object[]
					{
						str[i]
					});
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						break;
					}
				}
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printString(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, byte[] bStr)
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			string text = string.Empty;
			int count = TdrTypeUtil.cstrlen(bStr);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				text = Encoding.ASCII.GetString(bStr, 0, count);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}[{1:d}]: {2}{3}", new object[]
				{
					variable,
					arrIdx,
					text,
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printWString(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, short[] str)
		{
			TdrError.ErrorType errorType = buf.sprintf("{0}[{1:d}]", new object[]
			{
				variable,
				arrIdx
			});
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				int num = TdrTypeUtil.wstrlen(str);
				for (int i = 0; i < num; i++)
				{
					errorType = buf.sprintf("0x{0:X4}", new object[]
					{
						str[i]
					});
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						break;
					}
				}
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrIP(ref TdrVisualBuf buf, int indent, char sep, string variable, uint ip)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}: ", new object[]
				{
					variable
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrIP2Str(ref buf, ip);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrIP(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, uint ip)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}[{1:d}]: ", new object[]
				{
					variable,
					arrIdx
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrIP2Str(ref buf, ip);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrTime(ref TdrVisualBuf buf, int indent, char sep, string variable, uint time)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}: ", new object[]
				{
					variable
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrTime2Str(ref buf, time);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrTime(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, uint time)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}[{1:d}]: ", new object[]
				{
					variable,
					arrIdx
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrTime2Str(ref buf, time);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrDate(ref TdrVisualBuf buf, int indent, char sep, string variable, uint date)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}: ", new object[]
				{
					variable
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrDate2Str(ref buf, date);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrDate(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, uint date)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}[{1:d}]: ", new object[]
				{
					variable,
					arrIdx
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrDate2Str(ref buf, date);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrDateTime(ref TdrVisualBuf buf, int indent, char sep, string variable, ulong datetime)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}: ", new object[]
				{
					variable
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrDateTime2Str(ref buf, datetime);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}

		public static TdrError.ErrorType printTdrDateTime(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, ulong datetime)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printMultiStr(ref buf, "    ", indent);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}[{1:d}]: ", new object[]
				{
					variable,
					arrIdx
				});
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = TdrTypeUtil.tdrDateTime2Str(ref buf, datetime);
			}
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				errorType = buf.sprintf("{0}", new object[]
				{
					sep
				});
			}
			return errorType;
		}
	}
}
