using System;

namespace Apollo
{
	public enum ApolloPermission
	{
		None,
		GetUserInfo = 2,
		GetSimpleUserInfo = 4,
		AddAlbum = 8,
		AddIdol = 16,
		AddOneBlog = 32,
		AddPicT = 64,
		AddShare = 128,
		AddTopic = 256,
		CheckPageFans = 512,
		DelIdol = 1024,
		DelT = 2048,
		GetFansList = 4096,
		GetIdolList = 8192,
		GetInfo = 16384,
		GetOhterInfo = 32768,
		GetRepostList = 65536,
		ListAlbum = 131072,
		UploadPic = 262144,
		GetVipInfo = 524288,
		GetVipRichInfo = 1048576,
		GetIntimateFriendsWeibo = 2097152,
		MatchNickTipsWeibo = 4194304,
		GetAppFriends = 8388608,
		All = 16777215
	}
}
