using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class ApolloSnsService : ApolloObject, IApolloSnsService, IApolloServiceBase
	{
		public static readonly ApolloSnsService Instance = new ApolloSnsService();

		public event OnApolloShareEvenHandle onShareEvent;

		public event OnRelationNotifyHandle onRelationEvent;

		public event OnQueryGroupInfoNotifyHandle onQueryGroupInfoEvent;

		public event OnCreateWXGroupNotifyHandle onCreateWXGroupEvent;

		public event OnJoinWXGroupNotifyHandle onJoinWXGroupEvent;

		public event OnBindGroupNotifyHandle onBindGroupEvent;

		public event OnUnbindGroupNotifyHandle onUnBindGroupEvent;

		public event OnQueryGroupKeyNotifyHandle onQueryGroupKeyEvent;

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
			ApolloSnsService.SendToQQ(base.ObjectId, scene, title, desc, url, thumbImageUrl, thumbImageUrl.Length);
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
			if (msg.Length > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloShareResult @object = apolloStringParser.GetObject<ApolloShareResult>("ShareResult");
				if (this.onShareEvent != null)
				{
					try
					{
						this.onShareEvent(@object);
					}
					catch (Exception arg)
					{
						ADebug.Log("onShareEvent:" + arg);
					}
				}
			}
		}

		private void OnRelationNotify(string msg)
		{
			if (msg.Length > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloRelation @object = apolloStringParser.GetObject<ApolloRelation>("Relation");
				if (this.onRelationEvent != null)
				{
					try
					{
						this.onRelationEvent(@object);
					}
					catch (Exception arg)
					{
						ADebug.Log("OnRelationNotify:" + arg);
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
					catch (Exception arg)
					{
						ADebug.Log("OnBindGroupNotify:" + arg);
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
					catch (Exception arg)
					{
						ADebug.Log("OnUnbindGroupNotify:" + arg);
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
					catch (Exception arg)
					{
						ADebug.Log("OnQueryGroupKeyNotify:" + arg);
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
					catch (Exception arg)
					{
						ADebug.Log("OnQueryGroupInfoNotify:" + arg);
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
					catch (Exception arg)
					{
						ADebug.Log("OnCreateWXGroupNotify:" + arg);
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
					catch (Exception arg)
					{
						ADebug.Log("OnJoinWXGroupNotify:" + arg);
					}
				}
			}
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToWeixin(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] thumbImgData, int thumbDataLen, [MarshalAs(UnmanagedType.LPStr)] string extInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToWeixinWithPhoto(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string pszMediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] pImgData, int nImgDataLen);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToWeixinWithPhotoWithTail(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string pszMediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] pImgData, int nImgDataLen, [MarshalAs(UnmanagedType.LPStr)] string messageExt, [MarshalAs(UnmanagedType.LPStr)] string messageAction);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToQQ(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string imgUrl, int imgUrlLen);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SendToQQWithPhoto(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string imgFilePath);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_QueryQQGroupInfo(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cUnionid, [MarshalAs(UnmanagedType.LPStr)] string cZoneid);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_UnbindQQGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cGroupOpenid, [MarshalAs(UnmanagedType.LPStr)] string cUnionid);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_QueryQQGroupKey(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cGroupOpenid);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_JoinQQGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string qqGroupKey);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_QueryMyInfo(ulong objId, ApolloPlatform platform);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_QueryGameFriendsInfo(ulong objId, ApolloPlatform platform);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_SendToQQGameFriend(ulong objId, int act, [MarshalAs(UnmanagedType.LPStr)] string fopenid, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string summary, [MarshalAs(UnmanagedType.LPStr)] string targetUrl, [MarshalAs(UnmanagedType.LPStr)] string imgUrl, [MarshalAs(UnmanagedType.LPStr)] string previewText, [MarshalAs(UnmanagedType.LPStr)] string gameTag, [MarshalAs(UnmanagedType.LPStr)] string msdkExtInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Sns_SendToWXGameFriend(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string fOpenId, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string description, [MarshalAs(UnmanagedType.LPStr)] string mediaId, [MarshalAs(UnmanagedType.LPStr)] string messageExt, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPStr)] string msdkExtInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToWeixinWithMusic(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string musicUrl, [MarshalAs(UnmanagedType.LPStr)] string musicDataUrl, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] imgData, int imgDataLen, [MarshalAs(UnmanagedType.LPStr)] string messageExt, [MarshalAs(UnmanagedType.LPStr)] string messageAction);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToQQWithMusic(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string musicUrl, [MarshalAs(UnmanagedType.LPStr)] string musicDataUrl, [MarshalAs(UnmanagedType.LPStr)] string imgUrl);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToWeixinWithUrl(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string Url, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] imgData, int imgDataLen, [MarshalAs(UnmanagedType.LPStr)] string messageExt);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_BindQQGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cUnionid, [MarshalAs(UnmanagedType.LPStr)] string cUnion_name, [MarshalAs(UnmanagedType.LPStr)] string cZoneid, [MarshalAs(UnmanagedType.LPStr)] string cSignature);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_AddGameFriendToQQ(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cFopenid, [MarshalAs(UnmanagedType.LPStr)] string cDesc, [MarshalAs(UnmanagedType.LPStr)] string cMessage);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_CreateWXGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string unionid, [MarshalAs(UnmanagedType.LPStr)] string chatRoomName, [MarshalAs(UnmanagedType.LPStr)] string chatRoomNickName);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_JoinWXGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string unionid, [MarshalAs(UnmanagedType.LPStr)] string chatRoomNickName);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_QueryWXGroupInfo(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string unionid, [MarshalAs(UnmanagedType.LPStr)] string openIdList);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToWXGroup(ulong objId, int msgType, int subTyoe, [MarshalAs(UnmanagedType.LPStr)] string unionid, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string description, [MarshalAs(UnmanagedType.LPStr)] string messageExt, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPStr)] string imgUrl, [MarshalAs(UnmanagedType.LPStr)] string msdkExtInfo);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToQQWithRichPhoto(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string summary, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int size);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Sns_SendToQQWithVideo(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string summary, string videoPath);
	}
}
