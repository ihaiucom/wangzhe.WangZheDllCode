using apollo_talker;
using ApolloTdr;
using System;
using System.Text;

namespace Apollo
{
	internal class ApolloMessage
	{
		public float Life = 30f;

		internal TalkerMessageHandler Handler;

		internal TalkerMessageWithReceiptHandler HandlerWithReceipt;

		internal TalkerMessageWithoutReceiptHandler HandlerWithoutReceipt;

		internal RawMessageHandler RawMessageHandler;

		public bool IsRequest
		{
			get;
			private set;
		}

		public uint SeqNum
		{
			get;
			private set;
		}

		public TalkerCommand Command
		{
			get;
			set;
		}

		public Type RespType
		{
			get;
			set;
		}

		public Type ReceiptType
		{
			get;
			set;
		}

		public IPackable Request
		{
			get;
			set;
		}

		public IUnpackable Response
		{
			get;
			set;
		}

		public uint AsyncFlag
		{
			get;
			set;
		}

		public byte[] RawData
		{
			get;
			set;
		}

		public object Context
		{
			get;
			set;
		}

		internal ApolloTalker Talker
		{
			get;
			set;
		}

		public ApolloMessage()
		{
			this.SeqNum = ApolloCommon.MsgSeq;
			this.IsRequest = true;
		}

		public ApolloMessage(TalkerCommand command)
		{
			this.IsRequest = false;
			this.Command = command;
		}

		private void setFlag(ref byte flag, TalkerMessageType type, TalkerCommand.CommandValueType commandType)
		{
			flag = 0;
			flag |= ((byte)type & 7);
			if (commandType == TalkerCommand.CommandValueType.Raw)
			{
				flag |= 16;
			}
		}

		public static TalkerMessageType GetMessageType(int flag)
		{
			return (TalkerMessageType)(flag & 7);
		}

		private int calculateHeadSize(TalkerHead head)
		{
			return 100;
		}

		public bool PackRequestData(TalkerCommand command, IPackable request, out byte[] data, TalkerMessageType type)
		{
			if (command == null || command.Command == null)
			{
				throw new Exception("Invalid Argument!");
			}
			data = null;
			byte[] array = new byte[ApolloCommon.ApolloInfo.MaxMessageBufferSize];
			int usedBodySize = 0;
			if (request.packTLV(ref array, array.Length, ref usedBodySize, true) == TdrError.ErrorType.TDR_NO_ERROR && this.PackRequestData(command, array, usedBodySize, out data, type))
			{
				this.Request = request;
				return true;
			}
			ADebug.Log("PackRequestData request.pack error");
			return false;
		}

		public bool PackRequestData(TalkerCommand command, byte[] body, int usedBodySize, out byte[] data, TalkerMessageType type)
		{
			if (command == null || command.Command == null)
			{
				throw new Exception("Invalid Argument!");
			}
			data = null;
			if (body == null || usedBodySize <= 0)
			{
				ADebug.Log("PackRequestData request.pack error");
				return false;
			}
			TalkerHead talkerHead = new TalkerHead();
			this.setFlag(ref talkerHead.bFlag, type, command.Command.Type);
			ADebug.Log(string.Concat(new object[]
			{
				"PackRequestData head flag:",
				talkerHead.bFlag,
				" commandType:",
				command.Command.Type
			}));
			this.Command = command;
			talkerHead.bDomain = (byte)command.Domain;
			talkerHead.bCmdFmt = (byte)command.Command.Type;
			talkerHead.stCommand.iIntCmd = (int)command.Command.IntegerValue;
			if (command.Command.StringValue != null)
			{
				talkerHead.stCommand.szStrCmd = Encoding.get_UTF8().GetBytes(command.Command.StringValue);
			}
			if (type == TalkerMessageType.Response)
			{
				talkerHead.dwAsync = this.AsyncFlag;
			}
			else
			{
				talkerHead.dwAsync = this.SeqNum;
			}
			ADebug.Log(string.Concat(new object[]
			{
				"PackRequestData cmd: ",
				this.Command.ToString(),
				" type:",
				type,
				" async:",
				talkerHead.dwAsync
			}));
			int num = this.calculateHeadSize(talkerHead);
			byte[] array = new byte[num];
			int num2 = 0;
			TdrError.ErrorType errorType = talkerHead.packTLV(ref array, array.Length, ref num2, true);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				ADebug.Log(string.Concat(new object[]
				{
					"PackRequestData head.pack error:",
					errorType,
					" usedBodySize:",
					usedBodySize,
					" usedHeadSize:",
					num2,
					" headBufflen:",
					array.Length
				}));
				return false;
			}
			int num3 = usedBodySize + num2;
			data = new byte[num3];
			Array.Copy(array, data, num2);
			Array.Copy(body, 0, data, num2, usedBodySize);
			this.Request = null;
			return true;
		}

		public void HandleMessage()
		{
			if (this.Handler != null)
			{
				TalkerEventArgs e = new TalkerEventArgs(this.Response, this.Context);
				this.Handler(this.Request, e);
			}
			else if (this.HandlerWithoutReceipt != null)
			{
				this.HandlerWithoutReceipt(this.Response);
			}
			else if (this.HandlerWithReceipt != null)
			{
				IPackable packable = null;
				this.HandlerWithReceipt(this.Response, ref packable);
				if (packable != null)
				{
					ADebug.Log("HandlerWithReceipt receipt:" + packable);
					if (this.Talker != null && packable != null)
					{
						this.Talker.SendReceipt<NullResponse>(packable, this.AsyncFlag);
					}
					else
					{
						ADebug.Log("HandlerWithReceipt without receipt");
					}
				}
			}
			else if (this.RawMessageHandler != null)
			{
				ADebug.Log(("RawMessageHandler raw data size:" + this.RawData == null) ? 0 : this.RawData.Length);
				this.RawMessageHandler(new RawMessageEventArgs(this.RawData));
			}
			if (this.IsRequest)
			{
				ApolloMessageManager.Instance.RemoveMessage(this);
			}
		}
	}
}
