using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class ApolloSnsService : ApolloObject, IApolloServiceBase, IApolloSnsService
	{
		public static readonly ApolloSnsService Instance = new ApolloSnsService();

		public event OnApolloShareEvenHandle onShareEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onShareEvent = (OnApolloShareEvenHandle)Delegate.Combine(this.onShareEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onShareEvent = (OnApolloShareEvenHandle)Delegate.Remove(this.onShareEvent, value);
			}
		}

		public event OnRelationNotifyHandle onRelationEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onRelationEvent = (OnRelationNotifyHandle)Delegate.Combine(this.onRelationEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onRelationEvent = (OnRelationNotifyHandle)Delegate.Remove(this.onRelationEvent, value);
			}
		}

		public event OnQueryGroupInfoNotifyHandle onQueryGroupInfoEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onQueryGroupInfoEvent = (OnQueryGroupInfoNotifyHandle)Delegate.Combine(this.onQueryGroupInfoEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onQueryGroupInfoEvent = (OnQueryGroupInfoNotifyHandle)Delegate.Remove(this.onQueryGroupInfoEvent, value);
			}
		}

		public event OnCreateWXGroupNotifyHandle onCreateWXGroupEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onCreateWXGroupEvent = (OnCreateWXGroupNotifyHandle)Delegate.Combine(this.onCreateWXGroupEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onCreateWXGroupEvent = (OnCreateWXGroupNotifyHandle)Delegate.Remove(this.onCreateWXGroupEvent, value);
			}
		}

		public event OnJoinWXGroupNotifyHandle onJoinWXGroupEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onJoinWXGroupEvent = (OnJoinWXGroupNotifyHandle)Delegate.Combine(this.onJoinWXGroupEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onJoinWXGroupEvent = (OnJoinWXGroupNotifyHandle)Delegate.Remove(this.onJoinWXGroupEvent, value);
			}
		}

		public event OnBindGroupNotifyHandle onBindGroupEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onBindGroupEvent = (OnBindGroupNotifyHandle)Delegate.Combine(this.onBindGroupEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onBindGroupEvent = (OnBindGroupNotifyHandle)Delegate.Remove(this.onBindGroupEvent, value);
			}
		}

		public event OnUnbindGroupNotifyHandle onUnBindGroupEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onUnBindGroupEvent = (OnUnbindGroupNotifyHandle)Delegate.Combine(this.onUnBindGroupEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onUnBindGroupEvent = (OnUnbindGroupNotifyHandle)Delegate.Remove(this.onUnBindGroupEvent, value);
			}
		}

		public event OnQueryGroupKeyNotifyHandle onQueryGroupKeyEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onQueryGroupKeyEvent = (OnQueryGroupKeyNotifyHandle)Delegate.Combine(this.onQueryGroupKeyEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onQueryGroupKeyEvent = (OnQueryGroupKeyNotifyHandle)Delegate.Remove(this.onQueryGroupKeyEvent, value);
			}
		}

		private ApolloSnsService()
		{
		}

		public void SendToWeixin(ApolloShareScene scene, string title, string desc, string url, string mediaTagName, byte[] thumbImgData, int thumbDataLen)
		{
			throw new Exception("Api Not supported at current version");
		}

		public void SendToWeixin(string title, string desc, string mediaTagName, byte[] thumbImgData, int thumbDataLen, string extInfo)
		{
			ApolloSnsService.SendToWeixin(base.ObjectId, title, desc, mediaTagName, thumbImgData, thumbDataLen, extInfo);
		}

		public void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imgDataLen)
		{
			ApolloSnsService.SendToWeixinWithPhoto(base.ObjectId, aScene, mediaTagName, imageData, imgDataLen);
		}

		public void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction)
		{
			ApolloSnsService.SendToWeixinWithPhotoWithTail(base.ObjectId, aScene, mediaTagName, imageData, imageDataLen, messageExt, messageAction);
		}

		public void SendToQQ(ApolloShareScene scene, string title, string desc, string url, string thumbImageUrl)
		{
			ApolloSnsService.SendToQQ(base.ObjectId, scene, title, desc, url, thumbImageUrl, thumbImageUrl.get_Length());
		}

		public void SendToQQWithPhoto(ApolloShareScene scene, string imgFilePath)
		{
			ApolloSnsService.SendToQQWithPhoto(base.ObjectId, scene, imgFilePath);
		}

		public void SendToWeixinWithUrl(ApolloShareScene aScene, string title, string desc, string url, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt)
		{
			ApolloSnsService.Apollo_Sns_SendToWeixinWithUrl(base.ObjectId, aScene, title, desc, url, mediaTagName, imageData, imageDataLen, messageExt);
		}

		public bool QueryMyInfo(ApolloPlatform platform)
		{
			return ApolloSnsService.Apollo_Sns_QueryMyInfo(base.ObjectId, platform);
		}

		public bool QueryGameFriendsInfo(ApolloPlatform platform)
		{
			return ApolloSnsService.Apollo_Sns_QueryGameFriendsInfo(base.ObjectId, platform);
		}

		public bool SendToQQGameFriend(int act, string fopenid, string title, string summary, string targetUrl, string imgUrl, string previewText, string gameTag, string msdkExtInfo)
		{
			ADebug.Log(string.Concat(new object[]
			{
				"CApolloSnsService::SendToQQGameFriend act:",
				act,
				"fopenid:",
				fopenid,
				"title:",
				title,
				"summary:",
				summary,
				"targetUrl:",
				targetUrl,
				"imgUrl:",
				imgUrl,
				"previewText:",
				previewText,
				"gameTag:",
				gameTag,
				"msdkExtInfo:",
				msdkExtInfo
			}));
			return ApolloSnsService.Apollo_Sns_SendToQQGameFriend(base.ObjectId, act, fopenid, title, summary, targetUrl, imgUrl, previewText, gameTag, msdkExtInfo);
		}

		public bool SendToWXGameFriend(string fOpenId, string title, string description, string mediaId, string messageExt, string mediaTagName, string msdkExtInfo)
		{
			ADebug.Log(string.Concat(new string[]
			{
				"CApolloSnsService::SendToWXGameFriend fOpenId:",
				fOpenId,
				"title:",
				title,
				"description:",
				description,
				"mediaId:",
				mediaId,
				"messageExt:",
				messageExt,
				"mediaTagName:",
				mediaTagName,
				"msdkExtInfo:",
				msdkExtInfo
			}));
			return ApolloSnsService.Apollo_Sns_SendToWXGameFriend(base.ObjectId, fOpenId, title, description, mediaId, messageExt, mediaTagName, msdkExtInfo);
		}

		public void SendToWeixinWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction)
		{
			ADebug.Log(string.Concat(new string[]
			{
				"CApolloSnsService::SendToWeixinWithMusic title:",
				title,
				"desc:",
				desc,
				"musicUrl:",
				musicUrl,
				"musicDataUrl:",
				musicDataUrl,
				"mediaTagName:",
				mediaTagName,
				"messageExt:",
				messageExt,
				"messageAction:",
				messageAction
			}));
			ApolloSnsService.Apollo_Sns_SendToWeixinWithMusic(base.ObjectId, aScene, title, desc, musicUrl, musicDataUrl, mediaTagName, imageData, imageDataLen, messageExt, messageAction);
		}

		public void SendToQQWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string imgUrl)
		{
			ApolloSnsService.Apollo_Sns_SendToQQWithMusic(base.ObjectId, aScene, title, desc, musicUrl, musicDataUrl, imgUrl);
		}

		public void BindQQGroup(string cUnionid, string cUnion_name, string cZoneid, string cSignature)
		{
			ApolloSnsService.Apollo_Sns_BindQQGroup(base.ObjectId, cUnionid, cUnion_name, cZoneid, cSignature);
		}

		public void AddGameFriendToQQ(string cFopenid, string cDesc, string cMessage)
		{
			ApolloSnsService.Apollo_Sns_AddGameFriendToQQ(base.ObjectId, cFopenid, cDesc, cMessage);
		}

		public void JoinQQGroup(string qqGroupKey)
		{
			ApolloSnsService.Apollo_Sns_JoinQQGroup(base.ObjectId, qqGroupKey);
		}

		public void QueryQQGroupInfo(string cUnionid, string cZoneid)
		{
			ApolloSnsService.Apollo_Sns_QueryQQGroupInfo(base.ObjectId, cUnionid, cZoneid);
		}

		public void UnbindQQGroup(string cGroupOpenid, string cUnionid)
		{
			ApolloSnsService.Apollo_Sns_UnbindQQGroup(base.ObjectId, cGroupOpenid, cUnionid);
		}

		public void QueryQQGroupKey(string cGroupOpenid)
		{
			ApolloSnsService.Apollo_Sns_QueryQQGroupKey(base.ObjectId, cGroupOpenid);
		}

		public void SendToQQWithRichPhoto(string summary, ApolloImgPaths imgFilePaths)
		{
			if (imgFilePaths != null)
			{
				byte[] array;
				imgFilePaths.Encode(out array);
				if (array != null)
				{
					ApolloSnsService.Apollo_Sns_SendToQQWithRichPhoto(base.ObjectId, summary, array, array.Length);
				}
				else
				{
					ADebug.LogError("SendToQQWithRichPhoto Encode Error");
				}
			}
		}

		public void SendToQQWithVideo(string summary, string videoPath)
		{
			ApolloSnsService.Apollo_Sns_SendToQQWithVideo(base.ObjectId, summary, videoPath);
		}

		public void CreateWXGroup(string unionid, string chatRoomName, string chatRoomNickName)
		{
			ApolloSnsService.Apollo_Sns_CreateWXGroup(base.ObjectId, unionid, chatRoomName, chatRoomNickName);
		}

		public void JoinWXGroup(string unionid, string chatRoomNickName)
		{
			ApolloSnsService.Apollo_Sns_JoinWXGroup(base.ObjectId, unionid, chatRoomNickName);
		}

		public void QueryWXGroupInfo(string unionid, string openIdList)
		{
			ApolloSnsService.Apollo_Sns_QueryWXGroupInfo(base.ObjectId, unionid, openIdList);
		}

		public void SendToWXGroup(int msgType, int subTyoe, string unionid, string title, string description, string messageExt, string mediaTagName, string imgUrl, string msdkExtInfo)
		{
			ApolloSnsService.Apollo_Sns_SendToWXGroup(base.ObjectId, msgType, subTyoe, unionid, title, description, messageExt, mediaTagName, imgUrl, msdkExtInfo);
		}

		private void OnShareNotify(string msg)
		{
			if (msg.get_Length() > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloShareResult @object = apolloStringParser.GetObject<ApolloShareResult>("ShareResult");
				if (this.onShareEvent != null)
				{
					try
					{
						this.onShareEvent(@object);
					}
					catch (Exception ex)
					{
						ADebug.Log("onShareEvent:" + ex);
					}
				}
			}
		}

		private void OnRelationNotify(string msg)
		{
			if (msg.get_Length() > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloRelation @object = apolloStringParser.GetObject<ApolloRelation>("Relation");
				if (this.onRelationEvent != null)
				{
					try
					{
						this.onRelationEvent(@object);
					}
					catch (Exception ex)
					{
						ADebug.Log("OnRelationNotify:" + ex);
					}
				}
			}
		}

		private void OnBindGroupNotify(byte[] data)
		{
			ADebug.Log("OnBindGroupNotify");
			if (data.Length > 0)
			{
				ApolloGroupResult apolloGroupResult = new ApolloGroupResult();
				if (!apolloGroupResult.Decode(data))
				{
					ADebug.Log("OnBindGroupNotify Decode failed");
				}
				if (this.onBindGroupEvent != null)
				{
					try
					{
						this.onBindGroupEvent(apolloGroupResult);
					}
					catch (Exception ex)
					{
						ADebug.Log("OnBindGroupNotify:" + ex);
					}
				}
			}
		}

		private void OnUnbindGroupNotify(byte[] data)
		{
			ADebug.Log("OnUnbindGroupNotify");
			if (data.Length > 0)
			{
				ApolloGroupResult apolloGroupResult = new ApolloGroupResult();
				if (!apolloGroupResult.Decode(data))
				{
					ADebug.Log("OnUnbindGroupNotify Decode failed");
				}
				if (this.onUnBindGroupEvent != null)
				{
					try
					{
						this.onUnBindGroupEvent(apolloGroupResult);
					}
					catch (Exception ex)
					{
						ADebug.Log("OnUnbindGroupNotify:" + ex);
					}
				}
			}
		}

		private void OnQueryGroupKeyNotify(byte[] data)
		{
			ADebug.Log("OnQueryGroupKeyNotify");
			if (data.Length > 0)
			{
				ApolloGroupResult apolloGroupResult = new ApolloGroupResult();
				if (!apolloGroupResult.Decode(data))
				{
					ADebug.Log("OnQueryGroupKeyNotify Decode failed");
				}
				if (this.onQueryGroupKeyEvent != null)
				{
					try
					{
						this.onQueryGroupKeyEvent(apolloGroupResult);
					}
					catch (Exception ex)
					{
						ADebug.Log("OnQueryGroupKeyNotify:" + ex);
					}
				}
			}
		}

		private void OnQueryGroupInfoNotify(byte[] data)
		{
			ADebug.Log("OnQueryGroupInfoNotify");
			if (data.Length > 0)
			{
				ApolloGroupResult apolloGroupResult = new ApolloGroupResult();
				if (!apolloGroupResult.Decode(data))
				{
					ADebug.Log("OnQueryGroupInfoNotify Decode failed");
				}
				if (this.onQueryGroupInfoEvent != null)
				{
					try
					{
						this.onQueryGroupInfoEvent(apolloGroupResult);
					}
					catch (Exception ex)
					{
						ADebug.Log("OnQueryGroupInfoNotify:" + ex);
					}
				}
			}
		}

		private void OnCreateWXGroupNotify(byte[] data)
		{
			if (data.Length > 0)
			{
				ApolloGroupResult apolloGroupResult = new ApolloGroupResult();
				if (!apolloGroupResult.Decode(data))
				{
					ADebug.Log("OnCreateWXGroupNotify Decode failed");
				}
				if (this.onCreateWXGroupEvent != null)
				{
					try
					{
						this.onCreateWXGroupEvent(apolloGroupResult);
					}
					catch (Exception ex)
					{
						ADebug.Log("OnCreateWXGroupNotify:" + ex);
					}
				}
			}
		}

		private void OnJoinWXGroupNotify(byte[] data)
		{
			if (data.Length > 0)
			{
				ApolloGroupResult apolloGroupResult = new ApolloGroupResult();
				if (!apolloGroupResult.Decode(data))
				{
					ADebug.Log("OnJoinWXGroupNotify Decode failed");
				}
				if (this.onJoinWXGroupEvent != null)
				{
					try
					{
						this.onJoinWXGroupEvent(apolloGroupResult);
					}
					catch (Exception ex)
					{
						ADebug.Log("OnJoinWXGroupNotify:" + ex);
					}
				}
			}
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToWeixin(ulong objId, [MarshalAs(20)] string title, [MarshalAs(20)] string desc, [MarshalAs(20)] string mediaTagName, [MarshalAs(42)] byte[] thumbImgData, int thumbDataLen, [MarshalAs(20)] string extInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToWeixinWithPhoto(ulong objId, ApolloShareScene scene, [MarshalAs(20)] string pszMediaTagName, [MarshalAs(42)] byte[] pImgData, int nImgDataLen);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToWeixinWithPhotoWithTail(ulong objId, ApolloShareScene scene, [MarshalAs(20)] string pszMediaTagName, [MarshalAs(42)] byte[] pImgData, int nImgDataLen, [MarshalAs(20)] string messageExt, [MarshalAs(20)] string messageAction);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToQQ(ulong objId, ApolloShareScene scene, [MarshalAs(20)] string title, [MarshalAs(20)] string desc, [MarshalAs(20)] string url, [MarshalAs(20)] string imgUrl, int imgUrlLen);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToQQWithPhoto(ulong objId, ApolloShareScene scene, [MarshalAs(20)] string imgFilePath);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_QueryQQGroupInfo(ulong objId, [MarshalAs(20)] string cUnionid, [MarshalAs(20)] string cZoneid);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_UnbindQQGroup(ulong objId, [MarshalAs(20)] string cGroupOpenid, [MarshalAs(20)] string cUnionid);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_QueryQQGroupKey(ulong objId, [MarshalAs(20)] string cGroupOpenid);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_JoinQQGroup(ulong objId, [MarshalAs(20)] string qqGroupKey);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_QueryMyInfo(ulong objId, ApolloPlatform platform);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_QueryGameFriendsInfo(ulong objId, ApolloPlatform platform);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_SendToQQGameFriend(ulong objId, int act, [MarshalAs(20)] string fopenid, [MarshalAs(20)] string title, [MarshalAs(20)] string summary, [MarshalAs(20)] string targetUrl, [MarshalAs(20)] string imgUrl, [MarshalAs(20)] string previewText, [MarshalAs(20)] string gameTag, [MarshalAs(20)] string msdkExtInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_SendToWXGameFriend(ulong objId, [MarshalAs(20)] string fOpenId, [MarshalAs(20)] string title, [MarshalAs(20)] string description, [MarshalAs(20)] string mediaId, [MarshalAs(20)] string messageExt, [MarshalAs(20)] string mediaTagName, [MarshalAs(20)] string msdkExtInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToWeixinWithMusic(ulong objId, ApolloShareScene scene, [MarshalAs(20)] string title, [MarshalAs(20)] string desc, [MarshalAs(20)] string musicUrl, [MarshalAs(20)] string musicDataUrl, [MarshalAs(20)] string mediaTagName, [MarshalAs(42)] byte[] imgData, int imgDataLen, [MarshalAs(20)] string messageExt, [MarshalAs(20)] string messageAction);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToQQWithMusic(ulong objId, ApolloShareScene scene, [MarshalAs(20)] string title, [MarshalAs(20)] string desc, [MarshalAs(20)] string musicUrl, [MarshalAs(20)] string musicDataUrl, [MarshalAs(20)] string imgUrl);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToWeixinWithUrl(ulong objId, ApolloShareScene scene, [MarshalAs(20)] string title, [MarshalAs(20)] string desc, [MarshalAs(20)] string Url, [MarshalAs(20)] string mediaTagName, [MarshalAs(42)] byte[] imgData, int imgDataLen, [MarshalAs(20)] string messageExt);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_BindQQGroup(ulong objId, [MarshalAs(20)] string cUnionid, [MarshalAs(20)] string cUnion_name, [MarshalAs(20)] string cZoneid, [MarshalAs(20)] string cSignature);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_AddGameFriendToQQ(ulong objId, [MarshalAs(20)] string cFopenid, [MarshalAs(20)] string cDesc, [MarshalAs(20)] string cMessage);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_CreateWXGroup(ulong objId, [MarshalAs(20)] string unionid, [MarshalAs(20)] string chatRoomName, [MarshalAs(20)] string chatRoomNickName);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_JoinWXGroup(ulong objId, [MarshalAs(20)] string unionid, [MarshalAs(20)] string chatRoomNickName);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_QueryWXGroupInfo(ulong objId, [MarshalAs(20)] string unionid, [MarshalAs(20)] string openIdList);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToWXGroup(ulong objId, int msgType, int subTyoe, [MarshalAs(20)] string unionid, [MarshalAs(20)] string title, [MarshalAs(20)] string description, [MarshalAs(20)] string messageExt, [MarshalAs(20)] string mediaTagName, [MarshalAs(20)] string imgUrl, [MarshalAs(20)] string msdkExtInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToQQWithRichPhoto(ulong objId, [MarshalAs(20)] string summary, [MarshalAs(42)] byte[] data, int size);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToQQWithVideo(ulong objId, [MarshalAs(20)] string summary, string videoPath);
	}
}
