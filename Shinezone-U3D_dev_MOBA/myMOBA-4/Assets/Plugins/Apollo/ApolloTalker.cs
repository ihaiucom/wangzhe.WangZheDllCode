using ApolloTdr;
using System;

namespace Apollo
{
	internal class ApolloTalker : ApolloObject, IApolloTalker
	{
		private RecvedDataHandler recvDataHandler;

		private bool autoUpdate;

		private CircularCollection<ApolloMessage> apolloMessageCollections = new CircularCollection<ApolloMessage>(100);

		public IApolloConnector connector
		{
			get;
			private set;
		}

		public bool AutoUpdate
		{
			get
			{
				return this.autoUpdate;
			}
			set
			{
				if (this.autoUpdate != value)
				{
					this.autoUpdate = value;
					if (this.autoUpdate)
					{
						this.setRecvedDataHandler();
					}
					else
					{
						this.resetRecvedDataHandler();
					}
				}
			}
		}

		public ApolloTalker(IApolloConnector connector) : base(false, true)
		{
			this.connector = connector;
			if (connector == null)
			{
				throw new Exception("Invalid Argument");
			}
			this.AutoUpdate = true;
		}

		public ApolloResult Send<TResp>(TalkerCommand command, IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp : IUnpackable
		{
			return this.Send<TResp>(TalkerMessageType.Request, command, request, handler, context, timeout);
		}

		private ApolloResult SendReceipt<TResp>(TalkerCommand command, IPackable request, uint asyncFlag) where TResp : IUnpackable
		{
			return this.Send(TalkerMessageType.Response, command, request, new ApolloMessage
			{
				AsyncFlag = asyncFlag
			});
		}

		internal ApolloResult Send(TalkerCommand.CommandDomain domain, IPackable request)
		{
			return this.SendNotice(new TalkerCommand(domain, request), request);
		}

		internal ApolloResult SendNotice(TalkerCommand command, IPackable request)
		{
			ADebug.Log("SendNotice:" + request);
			ApolloMessage message = new ApolloMessage();
			return this.Send(TalkerMessageType.Notice, command, request, message);
		}

		public ApolloResult Send<TResp>(TalkerMessageType type, TalkerCommand command, IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp : IUnpackable
		{
			ApolloMessage apolloMessage = new ApolloMessage();
			apolloMessage.RespType = typeof(TResp);
			apolloMessage.Context = context;
			apolloMessage.Life = timeout;
			if (handler != null)
			{
				apolloMessage.Handler = delegate(object req, TalkerEventArgs loginInfo)
				{
					if (handler != null)
					{
						TalkerEventArgs<TResp> talkerEventArgs = new TalkerEventArgs<TResp>(loginInfo.Result, loginInfo.ErrorMessage);
						talkerEventArgs.Response = (TResp)((object)loginInfo.Response);
						talkerEventArgs.Context = loginInfo.Context;
						handler(req, talkerEventArgs);
					}
				};
			}
			return this.Send(type, command, request, apolloMessage);
		}

		public ApolloResult Send(TalkerMessageType type, TalkerCommand command, IPackable request, ApolloMessage message)
		{
			if (command == null || request == null)
			{
				return ApolloResult.InvalidArgument;
			}
			byte[] array = null;
			if (message == null)
			{
				message = new ApolloMessage();
			}
			if (message.PackRequestData(command, request, out array, type))
			{
				ADebug.Log(string.Concat(new object[]
				{
					"Sending:",
					command,
					" data size:",
					array.Length
				}));
				ApolloResult apolloResult = this.connector.WriteData(array, -1);
				if (apolloResult == ApolloResult.Success && message.Handler != null)
				{
					ApolloMessageManager.Instance.SeqMessageCollection.Add(message.SeqNum, message);
				}
				return apolloResult;
			}
			ADebug.LogError("Send: " + command + " msg.PackRequestDat error");
			return ApolloResult.InnerError;
		}

		public ApolloResult Send(TalkerMessageType type, TalkerCommand command, byte[] bodyData, int usedSize, ApolloMessage message)
		{
			if (command == null || bodyData == null || usedSize <= 0)
			{
				return ApolloResult.InvalidArgument;
			}
			byte[] array = null;
			if (message == null)
			{
				message = new ApolloMessage();
			}
			if (message.PackRequestData(command, bodyData, usedSize, out array, type))
			{
				ADebug.Log(string.Concat(new object[]
				{
					"Sending:",
					command,
					" data size:",
					array.Length
				}));
				return this.connector.WriteData(array, -1);
			}
			ADebug.LogError("Send: " + command + " msg.PackRequestData error");
			return ApolloResult.InnerError;
		}

		public ApolloResult Register<TResp>(TalkerCommand.CommandDomain domain, TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp : IUnpackable
		{
			Type typeFromHandle = typeof(TResp);
			string fullName = typeFromHandle.FullName;
			return this.Register<TResp>(new TalkerCommand(domain, fullName), handler);
		}

		public ApolloResult Register<TResp>(TalkerCommand command, TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp : IUnpackable
		{
			if (command != null && handler != null)
			{
				ADebug.Log("Register:" + command);
				if (ApolloMessageManager.Instance.Exist(command))
				{
				}
				ApolloMessage apolloMessage = new ApolloMessage(command);
				apolloMessage.RespType = typeof(TResp);
				apolloMessage.HandlerWithoutReceipt = delegate(IUnpackable resp)
				{
					handler((TResp)((object)resp));
				};
				ApolloMessageManager.Instance.Add(apolloMessage);
				return ApolloResult.Success;
			}
			return ApolloResult.InvalidArgument;
		}

		internal ApolloResult Register<TResp, TReceipt>(TalkerCommand command, TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp : IUnpackable where TReceipt : IPackable
		{
			if (command != null && handler != null)
			{
				if (ApolloMessageManager.Instance.Exist(command))
				{
				}
				ApolloMessage apolloMessage = new ApolloMessage(command);
				apolloMessage.RespType = typeof(TResp);
				apolloMessage.ReceiptType = typeof(TReceipt);
				apolloMessage.Talker = this;
				apolloMessage.HandlerWithReceipt = delegate(IUnpackable resp, ref IPackable receipt)
				{
					TReceipt tReceipt = default(TReceipt);
					handler((TResp)((object)resp), ref tReceipt);
					receipt = tReceipt;
					ADebug.Log("Register receipt:" + (receipt != null));
				};
				ApolloMessageManager.Instance.Add(apolloMessage);
				return ApolloResult.Success;
			}
			return ApolloResult.InvalidArgument;
		}

		public ApolloResult Register(TalkerCommand command, RawMessageHandler handler)
		{
			if (command != null && handler != null)
			{
				if (ApolloMessageManager.Instance.Exist(command))
				{
				}
				ApolloMessage apolloMessage = new ApolloMessage(command);
				apolloMessage.RawMessageHandler = handler;
				ApolloMessageManager.Instance.Add(apolloMessage);
				return ApolloResult.Success;
			}
			return ApolloResult.InvalidArgument;
		}

		public void Unregister(TalkerCommand command)
		{
			if (command != null)
			{
				ApolloMessageManager.Instance.Remove(command);
			}
		}

		private void setRecvedDataHandler()
		{
			if (this.connector == null)
			{
				return;
			}
			if (this.recvDataHandler == null)
			{
				this.recvDataHandler = new RecvedDataHandler(this.onRecvedData);
			}
			this.connector.RecvedDataEvent += this.recvDataHandler;
		}

		private void resetRecvedDataHandler()
		{
			if (this.connector == null)
			{
				return;
			}
			if (this.recvDataHandler != null)
			{
				this.connector.RecvedDataEvent -= this.recvDataHandler;
			}
		}

		public ApolloResult SendMessage(IPackable request)
		{
			return this.Send(request);
		}

		public ApolloResult RegisterMessage<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp : IUnpackable
		{
			return this.Register<TResp>(handler);
		}

		public void UnregisterMessage<TResp>()
		{
			this.Unregister<TResp>();
		}

		public ApolloResult Send<TResp>(IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp : IUnpackable
		{
			return this.Send<TResp>(new TalkerCommand(TalkerCommand.CommandDomain.App, request), request, handler, context, timeout);
		}

		internal ApolloResult SendReceipt<TResp>(IPackable request, uint asyncFlag) where TResp : IUnpackable
		{
			ApolloMessage apolloMessage = new ApolloMessage();
			apolloMessage.AsyncFlag = asyncFlag;
			return this.Send(TalkerMessageType.Response, new TalkerCommand(TalkerCommand.CommandDomain.App, request), request, apolloMessage);
		}

		private ApolloResult SendNotice(IPackable request)
		{
			ADebug.Log("SendNotice:" + request);
			ApolloMessage message = new ApolloMessage();
			return this.Send(TalkerMessageType.Notice, new TalkerCommand(TalkerCommand.CommandDomain.App, request), request, message);
		}

		public ApolloResult Send(IPackable request)
		{
			return this.SendNotice(request);
		}

		public ApolloResult Send(byte[] data, int usedSize)
		{
			return this.Send(TalkerMessageType.Notice, new TalkerCommand(TalkerCommand.CommandDomain.App, TalkerCommand.CommandValueType.Raw), data, usedSize, null);
		}

		public ApolloResult Register<TResp>(string command, TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp : IUnpackable
		{
			return this.Register<TResp>(new TalkerCommand(TalkerCommand.CommandDomain.App, command), handler);
		}

		public ApolloResult Register<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp : IUnpackable
		{
			Type typeFromHandle = typeof(TResp);
			string fullName = typeFromHandle.FullName;
			return this.Register<TResp>(fullName, handler);
		}

		public ApolloResult Register<TResp, TReceipt>(string command, TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp : IUnpackable where TReceipt : IPackable
		{
			return this.Register<TResp, TReceipt>(new TalkerCommand(TalkerCommand.CommandDomain.App, command), handler);
		}

		public ApolloResult Register<TResp, TReceipt>(TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp : IUnpackable where TReceipt : IPackable
		{
			Type typeFromHandle = typeof(TResp);
			string fullName = typeFromHandle.FullName;
			return this.Register<TResp, TReceipt>(fullName, handler);
		}

		public ApolloResult Register(RawMessageHandler handler)
		{
			return this.Register(new TalkerCommand(TalkerCommand.CommandDomain.App, TalkerCommand.CommandValueType.Raw), handler);
		}

		public void Unregister(string command)
		{
			this.Unregister(new TalkerCommand(TalkerCommand.CommandDomain.App, command));
		}

		public void Unregister<TResp>()
		{
			Type typeFromHandle = typeof(TResp);
			string fullName = typeFromHandle.FullName;
			this.Unregister(fullName);
		}

		public void UnregisterRawMessageHandler()
		{
			this.Unregister(new TalkerCommand(TalkerCommand.CommandDomain.App, TalkerCommand.CommandValueType.Raw));
		}

		public void Update(int num)
		{
			if (this.autoUpdate)
			{
				return;
			}
			if (num < 0)
			{
				return;
			}
			this.onRecvedData();
			for (int i = 0; i < num; i++)
			{
				if (!this.popAndHandleApolloMessage())
				{
					return;
				}
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			this.popAndHandleApolloMessage();
		}

		private void pushApolloMessage(ApolloMessage msg)
		{
			if (msg != null)
			{
				this.apolloMessageCollections.Add(msg);
			}
		}

		private bool popAndHandleApolloMessage()
		{
			if (this.apolloMessageCollections.Count == 0)
			{
				return false;
			}
			ADebug.Log("popAndHandleApolloMessage: " + this.apolloMessageCollections.Count);
			ApolloMessage next = this.apolloMessageCollections.Next;
			if (next != null)
			{
				if (next.Handler != null)
				{
					TalkerEventArgs e = new TalkerEventArgs(next.Response, next.Context);
					next.Handler(next.Request, e);
				}
				if (next.HandlerWithoutReceipt != null)
				{
					next.HandlerWithoutReceipt(next.Response);
				}
				if (next.HandlerWithReceipt != null)
				{
					IPackable packable = null;
					next.HandlerWithReceipt(next.Response, ref packable);
					if (packable != null)
					{
						ADebug.Log("HandlerWithReceipt receipt:" + packable);
						this.SendReceipt<NullResponse>(packable, next.AsyncFlag);
					}
				}
				if (next.IsRequest)
				{
					ApolloMessageManager.Instance.RemoveMessage(next);
				}
			}
			return true;
		}

		private void onRecvedData()
		{
			ADebug.Log("onRecvedData OnDataRecvedProc");
			while (true)
			{
				byte[] data = null;
				int realSize;
				ApolloResult apolloResult = this.connector.ReadData(out data, out realSize);
				if (apolloResult != ApolloResult.Success)
				{
					break;
				}
				try
				{
					ApolloMessage apolloMessage = ApolloMessageManager.Instance.UnpackResponseData(data, realSize);
					if (apolloMessage != null)
					{
						ADebug.Log(string.Concat(new object[]
						{
							"Recved:",
							apolloMessage.Command,
							" and resp is:",
							apolloMessage.Response
						}));
						ADebug.Log(string.Concat(new object[]
						{
							"OnDataRecvedProc: apolloMessage.Handler != null?: ",
							apolloMessage.Handler != null,
							" apolloMessage.HandlerWithReceipt != null?: ",
							apolloMessage.HandlerWithReceipt != null,
							" apolloMessage.HandlerWithoutReceipt != null?: ",
							apolloMessage.HandlerWithoutReceipt != null
						}));
						apolloMessage.HandleMessage();
					}
					else
					{
						ADebug.LogError("OnDataRecvedProc UnpackResponseData error");
					}
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
		}
	}
}
