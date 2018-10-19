using System;

namespace Apollo
{
	public interface IApolloSnsService : IApolloServiceBase
	{
		event OnApolloShareEvenHandle onShareEvent;

		event OnRelationNotifyHandle onRelationEvent;

		event OnQueryGroupInfoNotifyHandle onQueryGroupInfoEvent;

		event OnCreateWXGroupNotifyHandle onCreateWXGroupEvent;

		event OnJoinWXGroupNotifyHandle onJoinWXGroupEvent;

		event OnBindGroupNotifyHandle onBindGroupEvent;

		event OnUnbindGroupNotifyHandle onUnBindGroupEvent;

		event OnQueryGroupKeyNotifyHandle onQueryGroupKeyEvent;

		void SendToWeixin(string title, string desc, string mediaTagName, byte[] thumbImgData, int thumbDataLen, string extInfo);

		void SendToWeixinWithUrl(ApolloShareScene aScene, string title, string desc, string url, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt);

		void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imageDataLen);

		void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction);

		void SendToQQ(ApolloShareScene scene, string title, string desc, string url, string thumbImageUrl);

		void SendToQQWithPhoto(ApolloShareScene scene, string imgFilePath);

		bool QueryMyInfo(ApolloPlatform platform);

		bool QueryGameFriendsInfo(ApolloPlatform platform);

		bool SendToQQGameFriend(int act, string fopenid, string title, string summary, string targetUrl, string imgUrl, string previewText, string gameTag, string msdkExtInfo);

		bool SendToWXGameFriend(string fOpenId, string title, string description, string mediaId, string messageExt, string mediaTagName, string msdkExtInfo);

		void SendToWeixinWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction);

		void SendToQQWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string imgUrl);

		void BindQQGroup(string cUnionid, string cUnion_name, string cZoneid, string cSignature);

		void AddGameFriendToQQ(string cFopenid, string cDesc, string cMessage);

		void JoinQQGroup(string qqGroupKey);

		void QueryQQGroupInfo(string cUnionid, string cZoneid);

		void UnbindQQGroup(string cGroupOpenid, string cUnionid);

		void QueryQQGroupKey(string cGroupOpenid);

		void CreateWXGroup(string unionid, string chatRoomName, string chatRoomNickName);

		void JoinWXGroup(string unionid, string chatRoomNickName);

		void QueryWXGroupInfo(string unionid, string openIdList);

		void SendToWXGroup(int msgType, int subType, string unionid, string title, string description, string messageExt, string mediaTagName, string imgUrl, string msdkExtInfo);

		void SendToQQWithRichPhoto(string summary, ApolloImgPaths imgFilePaths);

		void SendToQQWithVideo(string summary, string videoPath);
	}
}
