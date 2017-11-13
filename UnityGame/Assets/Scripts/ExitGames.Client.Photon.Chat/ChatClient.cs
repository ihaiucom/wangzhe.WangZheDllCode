using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatClient : IPhotonPeerListener
	{
		private const int FriendRequestListMax = 1024;

		private const string ChatAppName = "chat";

		private string chatRegion = "EU";

		public int MessageLimit;

		public readonly Dictionary<string, ChatChannel> PublicChannels;

		public readonly Dictionary<string, ChatChannel> PrivateChannels;

		private readonly HashSet<string> PublicChannelsUnsubscribing;

		private readonly IChatClientListener listener;

		public ChatPeer chatPeer;

		private bool didAuthenticate;

		private int msDeltaForServiceCalls = 50;

		private int msTimestampOfLastServiceCall;

		public string NameServerAddress
		{
			get;
			private set;
		}

		public string FrontendAddress
		{
			get;
			private set;
		}

		public string ChatRegion
		{
			get
			{
				return this.chatRegion;
			}
			set
			{
				this.chatRegion = value;
			}
		}

		public ChatState State
		{
			get;
			private set;
		}

		public ChatDisconnectCause DisconnectedCause
		{
			get;
			private set;
		}

		public bool CanChat
		{
			get
			{
				return this.State == ChatState.ConnectedToFrontEnd && this.HasPeer;
			}
		}

		private bool HasPeer
		{
			get
			{
				return this.chatPeer != null;
			}
		}

		public string AppVersion
		{
			get;
			private set;
		}

		public string AppId
		{
			get;
			private set;
		}

		public AuthenticationValues AuthValues
		{
			get;
			set;
		}

		public string UserId
		{
			get
			{
				return (this.AuthValues == null) ? null : this.AuthValues.UserId;
			}
			private set
			{
				if (this.AuthValues == null)
				{
					this.AuthValues = new AuthenticationValues();
				}
				this.AuthValues.UserId = value;
			}
		}

		public bool UseBackgroundWorkerForSending
		{
			get;
			set;
		}

		public DebugLevel DebugOut
		{
			get
			{
				return this.chatPeer.DebugOut;
			}
			set
			{
				this.chatPeer.DebugOut = value;
			}
		}

		public ChatClient(IChatClientListener listener, ConnectionProtocol protocol = 0)
		{
			this.listener = listener;
			this.State = ChatState.Uninitialized;
			this.chatPeer = new ChatPeer(this, protocol);
			this.PublicChannels = new Dictionary<string, ChatChannel>();
			this.PrivateChannels = new Dictionary<string, ChatChannel>();
			this.PublicChannelsUnsubscribing = new HashSet<string>();
		}

		void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
		{
			this.listener.DebugReturn(level, message);
		}

		void IPhotonPeerListener.OnEvent(EventData eventData)
		{
			switch (eventData.Code)
			{
			case 0:
				this.HandleChatMessagesEvent(eventData);
				break;
			case 2:
				this.HandlePrivateMessageEvent(eventData);
				break;
			case 4:
				this.HandleStatusUpdate(eventData);
				break;
			case 5:
				this.HandleSubscribeEvent(eventData);
				break;
			case 6:
				this.HandleUnsubscribeEvent(eventData);
				break;
			}
		}

		void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
		{
			byte operationCode = operationResponse.OperationCode;
			switch (operationCode)
			{
			case 0:
			case 1:
			case 2:
			case 3:
				break;
			default:
				if (operationCode == 230)
				{
					this.HandleAuthResponse(operationResponse);
					return;
				}
				break;
			}
			if (operationResponse.ReturnCode != 0 && this.DebugOut >= 1)
			{
				if (operationResponse.ReturnCode == -2)
				{
					this.listener.DebugReturn(1, string.Format("Chat Operation {0} unknown on server. Check your AppId and make sure it's for a Chat application.", operationResponse.OperationCode));
				}
				else
				{
					this.listener.DebugReturn(1, string.Format("Chat Operation {0} failed (Code: {1}). Debug Message: {2}", operationResponse.OperationCode, operationResponse.ReturnCode, operationResponse.DebugMessage));
				}
			}
		}

		void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
		{
			if (statusCode != 1024)
			{
				if (statusCode != 1025)
				{
					if (statusCode != 1048)
					{
						if (statusCode == 1049)
						{
							this.State = ChatState.Disconnecting;
							this.chatPeer.Disconnect();
						}
					}
					else if (!this.didAuthenticate)
					{
						this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
						if (!this.didAuthenticate && this.DebugOut >= 1)
						{
							this.DebugReturn(1, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
						}
					}
				}
				else if (this.State == ChatState.Authenticated)
				{
					this.ConnectToFrontEnd();
				}
				else
				{
					this.State = ChatState.Disconnected;
					this.listener.OnChatStateChange(ChatState.Disconnected);
					this.listener.OnDisconnected();
				}
			}
			else
			{
				if (!this.chatPeer.IsProtocolSecure)
				{
					this.chatPeer.EstablishEncryption();
				}
				else if (!this.didAuthenticate)
				{
					this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
					if (!this.didAuthenticate && this.DebugOut >= 1)
					{
						this.DebugReturn(1, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
					}
				}
				if (this.State == ChatState.ConnectingToNameServer)
				{
					this.State = ChatState.ConnectedToNameServer;
					this.listener.OnChatStateChange(this.State);
				}
				else if (this.State == ChatState.ConnectingToFrontEnd)
				{
					this.AuthenticateOnFrontEnd();
				}
			}
		}

		public bool CanChatInChannel(string channelName)
		{
			return this.CanChat && this.PublicChannels.ContainsKey(channelName) && !this.PublicChannelsUnsubscribing.Contains(channelName);
		}

		public bool Connect(string appId, string appVersion, AuthenticationValues authValues)
		{
			this.chatPeer.TimePingInterval = 3000;
			this.DisconnectedCause = ChatDisconnectCause.None;
			if (authValues == null)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "Connect failed: no authentication values specified");
				}
				return false;
			}
			this.AuthValues = authValues;
			if (string.IsNullOrEmpty(this.AuthValues.UserId))
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "Connect failed: no UserId specified in authentication values.");
				}
				return false;
			}
			this.AppId = appId;
			this.AppVersion = appVersion;
			this.didAuthenticate = false;
			this.chatPeer.set_QuickResendAttempts(2);
			this.chatPeer.SentCountAllowance = 7;
			this.PublicChannels.Clear();
			this.PrivateChannels.Clear();
			this.PublicChannelsUnsubscribing.Clear();
			this.NameServerAddress = this.chatPeer.NameServerAddress;
			bool flag = this.chatPeer.Connect();
			if (flag)
			{
				this.State = ChatState.ConnectingToNameServer;
			}
			if (this.UseBackgroundWorkerForSending)
			{
				SupportClass.StartBackgroundCalls(new Func<bool>(this.SendOutgoingInBackground), this.msDeltaForServiceCalls, "ChatClient Service Thread");
			}
			return flag;
		}

		public void Service()
		{
			while (this.HasPeer && this.chatPeer.DispatchIncomingCommands())
			{
			}
			if (!this.UseBackgroundWorkerForSending && (Environment.get_TickCount() - this.msTimestampOfLastServiceCall > this.msDeltaForServiceCalls || this.msTimestampOfLastServiceCall == 0))
			{
				this.msTimestampOfLastServiceCall = Environment.get_TickCount();
				while (this.HasPeer && this.chatPeer.SendOutgoingCommands())
				{
				}
			}
		}

		private bool SendOutgoingInBackground()
		{
			while (this.HasPeer && this.chatPeer.SendOutgoingCommands())
			{
			}
			return this.State != ChatState.Disconnected;
		}

		[Obsolete("Better use UseBackgroundWorkerForSending and Service().")]
		public void SendAcksOnly()
		{
			if (this.HasPeer)
			{
				this.chatPeer.SendAcksOnly();
			}
		}

		public void Disconnect()
		{
			if (this.HasPeer && this.chatPeer.get_PeerState() != null)
			{
				this.chatPeer.Disconnect();
			}
		}

		public void StopThread()
		{
			if (this.HasPeer)
			{
				this.chatPeer.StopThread();
			}
		}

		public bool Subscribe(string[] channels)
		{
			return this.Subscribe(channels, 0);
		}

		public bool Subscribe(string[] channels, int messagesFromHistory)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "Subscribe called while not connected to front end server.");
				}
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, "Subscribe can't be called for empty or null channels-list.");
				}
				return false;
			}
			return this.SendChannelOperation(channels, 0, messagesFromHistory);
		}

		public bool Unsubscribe(string[] channels)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "Unsubscribe called while not connected to front end server.");
				}
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, "Unsubscribe can't be called for empty or null channels-list.");
				}
				return false;
			}
			for (int i = 0; i < channels.Length; i++)
			{
				string text = channels[i];
				this.PublicChannelsUnsubscribing.Add(text);
			}
			return this.SendChannelOperation(channels, 1, 0);
		}

		public bool PublishMessage(string channelName, object message, bool forwardAsWebhook = false)
		{
			return this.publishMessage(channelName, message, true, forwardAsWebhook);
		}

		internal bool PublishMessageUnreliable(string channelName, object message, bool forwardAsWebhook = false)
		{
			return this.publishMessage(channelName, message, false, forwardAsWebhook);
		}

		private bool publishMessage(string channelName, object message, bool reliable, bool forwardAsWebhook = false)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "PublishMessage called while not connected to front end server.");
				}
				return false;
			}
			if (string.IsNullOrEmpty(channelName) || message == null)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, "PublishMessage parameters must be non-null and not empty.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(1, channelName);
			dictionary.Add(3, message);
			Dictionary<byte, object> dictionary2 = dictionary;
			if (forwardAsWebhook)
			{
				dictionary2.Add(21, 1);
			}
			return this.chatPeer.OpCustom(2, dictionary2, reliable);
		}

		public bool SendPrivateMessage(string target, object message, bool forwardAsWebhook = false)
		{
			return this.SendPrivateMessage(target, message, false, forwardAsWebhook);
		}

		public bool SendPrivateMessage(string target, object message, bool encrypt, bool forwardAsWebhook)
		{
			return this.sendPrivateMessage(target, message, encrypt, true, forwardAsWebhook);
		}

		internal bool SendPrivateMessageUnreliable(string target, object message, bool encrypt, bool forwardAsWebhook = false)
		{
			return this.sendPrivateMessage(target, message, encrypt, false, forwardAsWebhook);
		}

		private bool sendPrivateMessage(string target, object message, bool encrypt, bool reliable, bool forwardAsWebhook = false)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "SendPrivateMessage called while not connected to front end server.");
				}
				return false;
			}
			if (string.IsNullOrEmpty(target) || message == null)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, "SendPrivateMessage parameters must be non-null and not empty.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(225, target);
			dictionary.Add(3, message);
			Dictionary<byte, object> dictionary2 = dictionary;
			if (forwardAsWebhook)
			{
				dictionary2.Add(21, 1);
			}
			return this.chatPeer.OpCustom(3, dictionary2, reliable, 0, encrypt);
		}

		private bool SetOnlineStatus(int status, object message, bool skipMessage)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "SetOnlineStatus called while not connected to front end server.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(10, status);
			Dictionary<byte, object> dictionary2 = dictionary;
			if (skipMessage)
			{
				dictionary2.set_Item(12, true);
			}
			else
			{
				dictionary2.set_Item(3, message);
			}
			return this.chatPeer.OpCustom(5, dictionary2, true);
		}

		public bool SetOnlineStatus(int status)
		{
			return this.SetOnlineStatus(status, null, true);
		}

		public bool SetOnlineStatus(int status, object message)
		{
			return this.SetOnlineStatus(status, message, false);
		}

		public bool AddFriends(string[] friends)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "AddFriends called while not connected to front end server.");
				}
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, "AddFriends can't be called for empty or null list.");
				}
				return false;
			}
			if (friends.Length > 1024)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, string.Concat(new object[]
					{
						"AddFriends max list size exceeded: ",
						friends.Length,
						" > ",
						1024
					}));
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(11, friends);
			Dictionary<byte, object> dictionary2 = dictionary;
			return this.chatPeer.OpCustom(6, dictionary2, true);
		}

		public bool RemoveFriends(string[] friends)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "RemoveFriends called while not connected to front end server.");
				}
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, "RemoveFriends can't be called for empty or null list.");
				}
				return false;
			}
			if (friends.Length > 1024)
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, string.Concat(new object[]
					{
						"RemoveFriends max list size exceeded: ",
						friends.Length,
						" > ",
						1024
					}));
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(11, friends);
			Dictionary<byte, object> dictionary2 = dictionary;
			return this.chatPeer.OpCustom(7, dictionary2, true);
		}

		public string GetPrivateChannelNameByUser(string userName)
		{
			return string.Format("{0}:{1}", this.UserId, userName);
		}

		public bool TryGetChannel(string channelName, bool isPrivate, out ChatChannel channel)
		{
			if (!isPrivate)
			{
				return this.PublicChannels.TryGetValue(channelName, ref channel);
			}
			return this.PrivateChannels.TryGetValue(channelName, ref channel);
		}

		public bool TryGetChannel(string channelName, out ChatChannel channel)
		{
			bool flag = this.PublicChannels.TryGetValue(channelName, ref channel);
			return flag || this.PrivateChannels.TryGetValue(channelName, ref channel);
		}

		private bool SendChannelOperation(string[] channels, byte operation, int historyLength)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(0, channels);
			Dictionary<byte, object> dictionary2 = dictionary;
			if (historyLength != 0)
			{
				dictionary2.Add(14, historyLength);
			}
			return this.chatPeer.OpCustom(operation, dictionary2, true);
		}

		private void HandlePrivateMessageEvent(EventData eventData)
		{
			object message = eventData.Parameters.get_Item(3);
			string text = (string)eventData.Parameters.get_Item(5);
			string privateChannelNameByUser;
			if (this.UserId != null && this.UserId.Equals(text))
			{
				string userName = (string)eventData.Parameters.get_Item(225);
				privateChannelNameByUser = this.GetPrivateChannelNameByUser(userName);
			}
			else
			{
				privateChannelNameByUser = this.GetPrivateChannelNameByUser(text);
			}
			ChatChannel chatChannel;
			if (!this.PrivateChannels.TryGetValue(privateChannelNameByUser, ref chatChannel))
			{
				chatChannel = new ChatChannel(privateChannelNameByUser);
				chatChannel.IsPrivate = true;
				chatChannel.MessageLimit = this.MessageLimit;
				this.PrivateChannels.Add(chatChannel.Name, chatChannel);
			}
			chatChannel.Add(text, message);
			this.listener.OnPrivateMessage(text, message, privateChannelNameByUser);
		}

		private void HandleChatMessagesEvent(EventData eventData)
		{
			object[] messages = (object[])eventData.Parameters.get_Item(2);
			string[] senders = (string[])eventData.Parameters.get_Item(4);
			string text = (string)eventData.Parameters.get_Item(1);
			ChatChannel chatChannel;
			if (!this.PublicChannels.TryGetValue(text, ref chatChannel))
			{
				if (this.DebugOut >= 2)
				{
					this.listener.DebugReturn(2, "Channel " + text + " for incoming message event not found.");
				}
				return;
			}
			chatChannel.Add(senders, messages);
			this.listener.OnGetMessages(text, senders, messages);
		}

		private void HandleSubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData.Parameters.get_Item(0);
			bool[] array2 = (bool[])eventData.Parameters.get_Item(15);
			for (int i = 0; i < array.Length; i++)
			{
				if (array2[i])
				{
					string text = array[i];
					if (!this.PublicChannels.ContainsKey(text))
					{
						ChatChannel chatChannel = new ChatChannel(text);
						chatChannel.MessageLimit = this.MessageLimit;
						this.PublicChannels.Add(chatChannel.Name, chatChannel);
					}
				}
			}
			this.listener.OnSubscribed(array, array2);
		}

		private void HandleUnsubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData.get_Item(0);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				this.PublicChannels.Remove(text);
				this.PublicChannelsUnsubscribing.Remove(text);
			}
			this.listener.OnUnsubscribed(array);
		}

		private void HandleAuthResponse(OperationResponse operationResponse)
		{
			if (this.DebugOut >= 3)
			{
				this.listener.DebugReturn(3, operationResponse.ToStringFull() + " on: " + this.chatPeer.NameServerAddress);
			}
			if (operationResponse.ReturnCode == 0)
			{
				if (this.State == ChatState.ConnectedToNameServer)
				{
					this.State = ChatState.Authenticated;
					this.listener.OnChatStateChange(this.State);
					if (operationResponse.Parameters.ContainsKey(221))
					{
						if (this.AuthValues == null)
						{
							this.AuthValues = new AuthenticationValues();
						}
						this.AuthValues.Token = (operationResponse.get_Item(221) as string);
						this.FrontendAddress = (string)operationResponse.get_Item(230);
						this.chatPeer.Disconnect();
					}
					else if (this.DebugOut >= 1)
					{
						this.listener.DebugReturn(1, "No secret in authentication response.");
					}
				}
				else if (this.State == ChatState.ConnectingToFrontEnd)
				{
					this.State = ChatState.ConnectedToFrontEnd;
					this.listener.OnChatStateChange(this.State);
					this.listener.OnConnected();
				}
			}
			else
			{
				short returnCode = operationResponse.ReturnCode;
				switch (returnCode)
				{
				case 32755:
					this.DisconnectedCause = ChatDisconnectCause.CustomAuthenticationFailed;
					break;
				case 32756:
					this.DisconnectedCause = ChatDisconnectCause.InvalidRegion;
					break;
				case 32757:
					this.DisconnectedCause = ChatDisconnectCause.MaxCcuReached;
					break;
				default:
					if (returnCode != -3)
					{
						if (returnCode == 32767)
						{
							this.DisconnectedCause = ChatDisconnectCause.InvalidAuthentication;
						}
					}
					else
					{
						this.DisconnectedCause = ChatDisconnectCause.OperationNotAllowedInCurrentState;
					}
					break;
				}
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "Authentication request error: " + operationResponse.ReturnCode + ". Disconnecting.");
				}
				this.State = ChatState.Disconnecting;
				this.chatPeer.Disconnect();
			}
		}

		private void HandleStatusUpdate(EventData eventData)
		{
			string user = (string)eventData.Parameters.get_Item(5);
			int status = (int)eventData.Parameters.get_Item(10);
			object message = null;
			bool flag = eventData.Parameters.ContainsKey(3);
			if (flag)
			{
				message = eventData.Parameters.get_Item(3);
			}
			this.listener.OnStatusUpdate(user, status, flag, message);
		}

		private void ConnectToFrontEnd()
		{
			this.State = ChatState.ConnectingToFrontEnd;
			if (this.DebugOut >= 3)
			{
				this.listener.DebugReturn(3, "Connecting to frontend " + this.FrontendAddress);
			}
			this.chatPeer.Connect(this.FrontendAddress, "chat");
		}

		private bool AuthenticateOnFrontEnd()
		{
			if (this.AuthValues == null)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "Can't authenticate on front end server. Authentication Values are not set");
				}
				return false;
			}
			if (this.AuthValues.Token == null || this.AuthValues.Token == string.Empty)
			{
				if (this.DebugOut >= 1)
				{
					this.listener.DebugReturn(1, "Can't authenticate on front end server. Secret is not set");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(221, this.AuthValues.Token);
			Dictionary<byte, object> dictionary2 = dictionary;
			return this.chatPeer.OpCustom(230, dictionary2, true);
		}
	}
}
