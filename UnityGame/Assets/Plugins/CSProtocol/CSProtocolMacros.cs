using System;

namespace CSProtocol
{
	public class CSProtocolMacros
	{
		public const int MAX_OPENID_LEN = 64;

		public const int LBS_IMPOSIBLE_RSSI_VALUE = 65535;

		public const int MAX_MACADDR_LEN = 64;

		public const int MAX_WIFI_CNT = 20;

		public const int MAX_CELL_CNT = 20;

		public const int LBS_LOCATION_NOT_EXIST = 0;

		public const int LBS_LOCATION_EXIST = 1;

		public const int MAX_TLOGMSG_LEN = 512;

		public const int MAX_SHARE_FRIEND_COUNT = 2;

		public const int SHARE_STATUS_LIST_LENGTH = 50;

		public const int MAX_TOKEN_LEN = 512;

		public const int MAX_CLIENTVERSION_LENGTH = 64;

		public const int MAX_APPLEREVIEW_SVRURI_LEN = 64;

		public const int MAX_VERSION_IN_APPLEREVIEW = 10;

		public const int MAX_VERSION_UPDATE_URL = 200;

		public const int MAX_NICKNAME_LEN = 128;

		public const int MAX_POTRAITURL_LEN = 512;

		public const int MAX_EXTRA_INFO = 512;

		public const int SSKR_NOREASON = 0;

		public const int SSKR_KICKBYLOGIN = 1;

		public const int SSKR_SERVERERROR = 2;

		public const int SSKR_TOKENFAIL = 3;

		public const int SSKR_TOKENPAYFAIL = 4;

		public const int SSKR_VERSIONERROR = 5;

		public const int SSKR_NEEDAUTHERROR = 6;

		public const int SSKR_IDIPOPERATION = 7;

		public const int SSKR_PLAYERFULL = 8;

		public const int SSKR_COUNT = 9;

		public const int LR_LOGINPLAYERFULL = -19;

		public const int LR_AREAREGISTER_CONTROL = -18;

		public const int LR_CLIENT_FAKE = -17;

		public const int LR_SERVER_REFUSE = -16;

		public const int LR_SERVER_BUSY = -15;

		public const int LR_SERVER_REDIRECT = -14;

		public const int LR_SERVER_COLSE = -13;

		public const int LR_REGISTER_CLOSE = -12;

		public const int LR_REGISTER_CONTROL = -11;

		public const int LR_PAYTOKEN_INVALID = -10;

		public const int LR_SERVERVERSION_INVALID = -9;

		public const int LR_CLIENTVERSION_INVALID = -8;

		public const int LR_CLIENTVERSION_TOOOLD = -7;

		public const int LR_TOKENEXPIRE = -6;

		public const int LR_OPENPFVERIFYFAIL = -5;

		public const int LR_DBERROR = -4;

		public const int LR_PREVILEGE = -3;

		public const int LR_IDIP = -2;

		public const int LR_CHEAT = -1;

		public const int LR_NOLOCK = 0;

		public const int PID_ALL = -1;

		public const int PID_IOS = 0;

		public const int PID_ANDROID = 1;

		public const int PID_COUNT = 2;

		public const int SHAREACTION_ALLOW_SHARE = 0;

		public const int SHAREACTION_FORBID_SHARE = 1;

		public const int SHAREACTION_DEFAULT_ALLOW_SHARE = 2;

		public const int SHAREACTION_DEFAULT_FORBID_SHARE = 3;

		public const int VERSION_CONFIG_BAD = -2;

		public const int VERSION_BAD = -1;

		public const int VERSION_NEWEST = 0;

		public const int VERSION_FORCEUPDATE = 1;

		public const int VERSION_CANUPDATE = 2;

		public const int VERSION_IN_REVIEW = 3;

		public const int LANG_UNKNOW = 0;

		public const int LANG_ZH = 1;

		public const int LANG_EN = 2;

		public const int LANG_CNT = 3;

		public const int ACCOUNT_OPENID_STATE_NORMAL = 0;

		public const int ACCOUNT_OPENID_STATE_IDIP_OP = 1;

		public const int ACCOUNT_OPENID_STATE_IDIP_FINISH = 2;

		public const int COM_VERSION = 1;

		public const int COM_MAX_PLAYER_ONEGAME_LIMIT = 10;

		public const int COM_MAX_PLAYER_ONEROOM_LIMIT = 15;

		public const int COM_MAX_BOOTFRAP_DETAIL_NUM = 40;

		public const int COM_MAX_PLAYER_ONECAMP_LIMIT = 5;

		public const int COM_MAX_ROOMMATCH_SCORE = 5000;

		public const int COM_PLAYERCAMP_FIGHT = 2;

		public const int COM_MAX_LEVEL_STAR_NUM = 3;

		public const int COM_MAX_CHAPTER_LEVEL_NUM = 4;

		public const int COM_MAX_CHAPTER_NUM = 10;

		public const int COM_MAX_STATISTIC_BASE_KEY_NUM = 4;

		public const int COM_MAX_STATISTIC_BASE_VALUE_NUM = 2;

		public const int COM_MAX_STATISTIC_STRUCT_NUM_IN_PILE = 200;

		public const int COM_MAX_STATISTIC_PILE_NUM = 5;

		public const int COM_MAX_REWARD_NUM = 200;

		public const int COM_MAX_REWARD_SHORTNUM = 20;

		public const int COM_MAX_HERO_NUM_IN_GAME = 3;

		public const int COM_MAX_ACNT_CHOOSE_HERO = 3;

		public const int COM_SUCCESS = 0;

		public const int COM_MAX_INVITE_INFO_NUM = 5;

		public const int COM_MAX_ACTIVITY_NUM = 30;

		public const int COM_MAX_SUBACTIVITY_NUM = 20;

		public const int COM_MAX_APPID_LEN = 32;

		public const int COM_MAX_APPKEY_LEN = 48;

		public const int COM_MAX_SESSION_LEN = 32;

		public const int COM_MAX_PF_LEN = 512;

		public const int COM_MAX_ZONEID_LEN = 16;

		public const int COM_MAX_SNS_NICK_NAME_LEN = 256;

		public const int COM_MAX_SNS_FRIEND_NUM = 500;

		public const int COM_MAX_BLACKLIST_NUM = 300;

		public const int COM_MAX_HEAD_URL_LEN = 256;

		public const int COM_MAX_PLAYERROLENAME_LEN = 64;

		public const int COM_MAX_ACNTPACKAGE_POSNUM = 400;

		public const int COM_MAX_SHOPTYPE_CNT = 20;

		public const int COM_MAX_SHOPITEM_CNT = 12;

		public const int COM_MAX_SYMBOLPAGE_CNT = 50;

		public const int COM_MAX_SYMBOLPOS_CNT = 30;

		public const int COM_MAX_ENCHANTEFT_PEREQUIP = 8;

		public const int COM_MAX_COINBUY_TIMES = 10;

		public const int COM_MAX_SYMBOLPAGENAME_LEN = 64;

		public const int COM_MAX_SPECAIL_SAILITEM_CNT = 20;

		public const int COM_MAX_ACNT_COINCNT = 20;

		public const int COM_MAX_ACNT_BANTIME_CNT = 100;

		public const int COM_MAX_SHOPDRAW_CNT = 15;

		public const int COM_MAX_SHOPDRAW_SUBTYPE_CNT = 5;

		public const int COM_MAX_SHOPBUY_LIMITCNT = 20;

		public const int COM_MAX_SAMESYMBOL_WEARCNT = 10;

		public const int COM_MAX_SCRET_SAILITEM_CNT = 100;

		public const int COM_MAX_AKALISHOP_ITEMCNT = 50;

		public const int COM_MAX_AKALISHOP_RECMDSTR_LEN = 128;

		public const int COM_MAX_AKALISHOP_FLOWID_LEN = 64;

		public const int COM_MAX_FRIEND_NUM = 220;

		public const int COM_MAX_FRIEND_INVITE_ACHIEVE_NUM = 10;

		public const int APOLLO_MAX_PF_LENGTH = 256;

		public const int GAIN_CHEST_ID_MAX_LEN = 256;

		public const int COM_MAX_CHEST_SUBJECT = 128;

		public const int COM_MAX_CHEST_CONTENT = 256;

		public const int COM_MAX_LBS_NUM = 20;

		public const int COM_MAX_PICK_NUM = 2;

		public const int COM_MAX_TGWVIP_NUM = 30;

		public const int COM_MAX_HONOR_NUM = 6;

		public const int COM_MAX_BATTLE_HERO = 8;

		public const int COM_MAX_GEAR_PER_HERO = 6;

		public const int COM_MAX_ABSORBEQUIP_PER_HERO = 90;

		public const int COM_MAX_BATTLELIST_NUM = 100;

		public const int COM_MAX_SKILL_PER_HERO = 5;

		public const int COM_MAX_HERO_NUM = 200;

		public const int COM_MAX_HEROEQUIP_NUM = 6;

		public const int COM_MAX_SELFDEFINE_EQUIP_HERONUM = 200;

		public const int COM_MAX_EQUIPEVAL_NUM_PERACNT = 5;

		public const int COM_TOPN_EQUIPEVAL_NUM = 15;

		public const int COM_MAX_EQUIPEVAL_NUM = 20;

		public const int COM_MAX_EVAL_PEREQUIP = 20;

		public const int COM_MAX_EVAL_SCORE = 5;

		public const int COM_MAX_MOST_USED_HERO_NUM = 20;

		public const int COM_MAX_FREEHERO_INACNT = 50;

		public const int COM_MAX_TASK_PERACNT = 85;

		public const int COM_MAX_USUALTASK_PERACNT = 30;

		public const int COM_MAX_MAINTASKID_NUM = 10;

		public const int COM_MAX_PREREQUISITE_NUM = 3;

		public const int COM_MAX_TASK_REWARDNUM = 3;

		public const int COM_MAX_TASK_HAVE = 3;

		public const int COM_MAX_TASKREFRSH_DEFAULTNUM = 1;

		public const int COM_MAX_MAIL_DAYS = 30;

		public const int COM_MAX_MAIL_NUM = 100;

		public const int COM_MAX_SYSMAIL_NUM = 100;

		public const int COM_MAX_FRIENDMAIL_NUM = 30;

		public const int COM_MAX_ASKFORREQ_NUM = 100;

		public const int COM_MAX_MAIL_ACCESS = 10;

		public const int COM_MAX_MAIL_SUBJECT = 128;

		public const int COM_MAX_MAIL_CONTENT = 600;

		public const int COM_MAX_NOTICE_CONTENT = 2560;

		public const int COM_MAX_MSG_CONTENT = 300;

		public const int COM_MAX_NOTICE_CNT = 30;

		public const int COM_MAX_PRESENTMSG_LEN = 400;

		public const int COM_MAX_REDDOT_CNT = 100;

		public const int COM_MAX_ROLLINGMSG_CONTENT = 512;

		public const int COM_MAX_ROLLINGMSG_NUM = 50;

		public const int COM_MAX_ROOMNAME_LEN = 64;

		public const int COM_MAX_RANDOM_REWARD_NUM = 200;

		public const int COM_MAX_NEWBIE_STATUS_BITS_NUM = 5;

		public const int COM_MAX_CLIENT_BITS_NUM = 2;

		public const int COM_MAX_NEWCLIENT_BITS_NUM = 5;

		public const int COM_MAX_ADVENTURE_DIFFICULTY_NUM = 4;

		public const int COM_MAX_INTIMACY_RELATION_NUM = 4;

		public const int COM_MAX_CLASSNUM_OF_RANKGRADE = 5000;

		public const int COM_THREE_STAR_BITS = 7;

		public const int COM_MAX_FRIEND_RANK_INFO_NUM = 10;

		public const int COM_MAX_HEROINFONUM_OF_RANK = 3;

		public const int COM_MAX_ACNTNUM_PER_RANKCLASS = 250;

		public const int COM_MAX_TALENT_NUM = 5;

		public const int COM_MAX_TALENT_RCD_CNT = 20;

		public const int COM_MAX_COINDROW_STEP = 4;

		public const int COM_MAX_SKIN_ID = 63;

		public const int COM_MAX_STATISTIC_SINGLE_NUM = 20;

		public const int COM_MAX_STATISTIC_MULTI_NUM = 40;

		public const int COM_MAX_OPENID_LEN = 64;

		public const int COM_MAX_USER_COMPLAINT_REMARK_LEN = 256;

		public const int COM_MAX_VERIFICATIONINFO_LEN = 128;

		public const int COM_MAX_ARENA_BLOCK_NUM = 200;

		public const int COM_MAX_ARENABLOCK_MEMBER_NUM = 200;

		public const int COM_MAX_ARENA_FIGHTER_NUM = 100;

		public const int COM_MAX_WEAL_NUM = 40;

		public const int COM_MAX_TEAM_WEAL_NUM = 10;

		public const int COM_MAX_ARENA_FIGHT_HISTORY_NUM = 10;

		public const int COM_MAX_STATISTIC_KEY_VALUE_NUM = 50;

		public const int COM_MAX_WARM_BATTLE_TYPE_NUM = 10;

		public const int COM_MAX_GET_BURNING_ENEMY_NUM = 10;

		public const int COM_MAX_PVPMATCH_LEVEL_NUM = 50;

		public const int COM_MAX_WEAL_MULTIPLE_PERIOD_NUM = 4;

		public const int COM_MAX_WEAL_REWARD_MULTIPLE = 10000;

		public const int COM_MAX_WEAL_CONDITION_NUM = 10;

		public const int COM_MAX_WEAL_STATISTIC_NUM = 4;

		public const int COM_MAX_WEAL_CON_DATA_NUM = 20;

		public const int COM_MAX_WEAL_POINT_DATA_NUM = 20;

		public const int COM_MAX_PROP_MULTIPLE_NUM = 4;

		public const int COM_MAX_QQVIP_MULTIPLE = 10000;

		public const int COM_MAX_PROP_MULTIPLE = 40000;

		public const int COM_MAX_PROP_MULTIPLE_DAYS = 31;

		public const int COM_MAX_PROP_MULTIPLE_WIN = 31;

		public const int COM_MAX_FOREVER_FREEHERO_NUM = 7;

		public const int COM_MAX_PVPDAILY_MULTIPLE = 100000;

		public const int COM_MAX_RANK_NUM = 32;

		public const int COM_MAX_RANK_COMMON_USED_HERO_NUM = 5;

		public const int COM_MAX_RANK_CURSEASON_RECORD_NUM = 10;

		public const int COM_MAX_RANK_PASTSEASON_RECORD_NUM = 10;

		public const int COM_MAX_PLAYER_FIGHT_RECORD_NUM = 100;

		public const int COM_MAX_INGAME_CHEAT_TYPE = 10;

		public const int COM_MAX_DRAGON_DEAD_TIMES = 10;

		public const int COM_MAX_RUNE_TYPES = 10;

		public const int COM_MAX_GAME_TIME_PER_30SEC = 60;

		public const int COM_MAX_LUCKYDRAW_REWARD_PER_POOL = 14;

		public const int COM_MAX_ITEM_CNT_PER_LIMIT = 14;

		public const int COM_MAX_ITEM_LIMIT_CNT = 20;

		public const int COM_MAX_NONHERO_CNT = 20;

		public const int COM_MAX_DONATE_NUM = 100;

		public const int COM_MAX_SKIN_LIMIT_NUM = 100;

		public const int COM_MAX_BUY_INBATTLE_EQUIP_NUM = 30;

		public const int COM_MAX_FINAL_INBATTLE_EQUIP_ONE_ACNT = 6;

		public const int COM_MAX_INGAME_STATISTIC_REVIVE_NUM = 20;

		public const int COM_MAX_DAYHUOYUEDUREWARD_NUM = 10;

		public const int COM_MAX_WEEKHUOYUEDUREWARD_NUM = 10;

		public const int COM_MAX_INBATTLE_AGE_LEVEL_NUM = 10;

		public const int COM_MAX_INBATTLE_AGE_NUM = 20;

		public const int COM_MAX_WXGAME_MULTIPLE = 5000;

		public const int COM_MAX_QQGAMECENTER_MULTIPLE = 5000;

		public const int COM_MAX_IOSVISITOR_MULTIPLE = 5000;

		public const int COM_MAX_GUILD_MULTIPLE = 5000;

		public const int COM_MAX_RECRUTMENT_MULTIPLE = 5000;

		public const int COM_MAX_CLT_BUILD_NUMBER_LEN = 64;

		public const int COM_MAX_CLT_SVN_VERSION_LEN = 64;

		public const int COM_MAX_FRIEND_FILTER_LEN = 700;

		public const int COM_MAX_PVPSPEC_OUTPUT_CNT = 3;

		public const int COM_MAX_WEAL_EXCHANGE_NUM = 15;

		public const int COM_MAX_REWARD_ITEM_CNT = 20;

		public const int COM_MAX_ACNT_LICENSE_CNT = 64;

		public const int COM_MAX_ACNT_PAYBILLNO_LEN = 64;

		public const int COM_DEFAULT_ACNT_CREDIT_VALUE = 100;

		public const int COM_MAX_SELFDEFINE_CHATINFO_NUM = 30;

		public const int COM_MAX_RANKMATCH_TOP_NUM = 50;

		public const int COM_MAX_WORLDRANK_ITEM_NUM = 100;

		public const int COM_MAX_ACNT_RANK_STATISTIC_NUM = 15;

		public const int COM_MAX_INGAME_STATISTIC_DEATH_NUM = 50;

		public const int COM_MAX_RECRUITMENT_REWARD_NUM = 10;

		public const int COM_MAX_INGAME_EYE_NUM = 15;

		public const int COM_MAX_MASTER_LEVEL = 10;

		public const int COM_MAX_SPECIALBS_ID = 32;

		public const int COM_MAX_IDIPADDITEM_CNT = 5;

		public const int COM_MAX_PLAT_CHANNEL_NUM = 7;

		public const int COM_MAX_REWARDMATCH_NUM = 8;

		public const int COM_MAX_GUILDMATCH_SIGNUP_BEIZHU_LEN = 256;

		public const int COM_MAX_GUILDMATCH_INVITECARD_NUM = 20;

		public const int COM_MAX_GUILDMATCH_SIGNUPHERO_NUM = 3;

		public const int COM_MAX_SIGNATUREINFO_LEN = 128;

		public const int COM_MAX_ACNT_PASSWD_LEN = 64;

		public const int COM_MAX_PLACE_LEN = 64;

		public const int COM_MAX_DATING_DECLARATION_LEN = 128;

		public const int COM_MAX_TAG_NUM = 5;

		public const int INVALID_WEAL_ID = 0;

		public const int WEAL_REFRESH_DAYTIME = 0;

		public const int COM_MAX_CUSTOM_EQUIP_NUM_PER_HERO = 1000;

		public const int COM_MAX_ITEMCNT_PER_GIFTLIMIT = 60;

		public const int COM_MAX_HEADIMAGE_CNT = 300;

		public const int COM_MAX_TIMESTR_LEN = 16;

		public const int COM_MAX_GAMEVIP_MULTIPLE = 10000;

		public const int COM_MAX_REWARD_MATCH_NUM = 128;

		public const int COM_DEFAULT_BATTLE_DIFFICULTY_ID = 0;

		public const int COM_MAX_PVPSPECITEM_LIMIT_CNT = 15;

		public const int COM_MAX_CLICK_NUM = 30;

		public const int COM_MAX_OPERATE_NUM = 20;

		public const int COM_MAX_DRAGON_BATTLE_TO_DEAD = 10;

		public const int COM_MONSTER_DEAD_TYPE_NUM = 20;

		public const int COM_MAX_INGAME_ADVANTAGE_NUM = 16;

		public const int COM_MAX_GREATMATCH_NUM = 24;

		public const int COM_MAX_LABEL_NUM = 2;

		public const int COM_ULR_ADDR_LEN = 128;

		public const int COM_MAX_BP_ROUND_NUM = 20;

		public const int COM_MAX_RANK_BIG_GRADE = 10;

		public const int COM_MAX_HERO_STATISTIC_TYPE_NUM = 2;

		public const int COM_MAX_INVITE_DENY_REASON_LEN = 60;

		public const int COM_MAX_RECENT_EXTRA_STATISTIC_NUM = 100;

		public const int COM_MAX_BATCH_EXCHAGE_NUM = 300;

		public const int COM_MAX_STUDENTINFONTF_NUM = 10;

		public const int COM_MAX_RECENT_USED_HERO_NUM = 3;

		public const int COM_MAX_PROFIT_LIMIT_NUM = 2;

		public const int COM_MAX_MASTERREQ_NUM = 10;

		public const int COM_MAX_PROCESS_STUDENT_NUM = 6;

		public const int COM_MAX_SELFDEFINEEQUIP_PER_HERO = 3;

		public const int COM_MAX_SELFDEFINEEQUIP_NAME_LEN = 20;

		public const int COM_MMR_PER_OLDGRADESTAR = 16;

		public const int COM_MAX_CLIENT_FPS_NUM = 500;

		public const int COM_MAX_CLIENT_PING_NUM = 500;

		public const int COM_MAX_CHAT_CONTENT_LEN = 64;

		public const int COM_MAX_HORN_CONTENT_LEN = 256;

		public const int COM_ACNT_OPER_GETARENAFIGHTHISTORY_INTERVAL = 60000;

		public const int COM_ACNT_OPER_GETFRIENDSTATE_INTERVAL = 60000;

		public const int COM_ACNT_OPER_GETSTUDENT_INTERVAL = 1000;

		public const int COM_MAX_GUILD_EVENTINFO_LEN = 128;

		public const int COM_MAX_GUILD_EVENT_NUM = 100;

		public const int COM_MAX_OFFLINE_CHAT_CNT = 50;

		public const int COM_MAX_BURNING_LEVEL_NUM = 10;

		public const int COM_MAX_BURNING_DIFFICULT_NUM = 2;

		public const int COM_MAX_BURNING_LUCKY_BUFF_NUM = 3;

		public const int COM_MAX_CREATE_GUILD_MEMBER_NUM = 20;

		public const int COM_MAX_GUILD_MEMBER_NUM = 150;

		public const int COM_MAX_GUILDNAME_LEN = 32;

		public const int COM_MAX_GUILDNOTICE_LEN = 128;

		public const int COM_MAX_GUILD_LIST_PAGE_CNT = 10;

		public const int COM_MAX_GUILD_APPLY_PAGE_CNT = 25;

		public const int COM_MAX_GUILD_RECOMMEND_PAGE_CNT = 25;

		public const int COM_MAX_GUILD_BUILDING_CNT = 10;

		public const int COM_MAX_POSITION_CHANGE_CNT = 8;

		public const int COM_MAX_GUILD_SELF_RECOMMEND_CNT = 2;

		public const int COM_MAX_GUILD_GROUP_OPENID_LEN = 128;

		public const int COM_MAX_GUILD_RECRUIT_INFO_NUM = 100;

		public const int COM_MAX_GUILD_MATCH_OB_NUM = 10;

		public const int COM_MAX_ACHIEVEMENT_NUM = 400;

		public const int COM_MAX_ACHIEVEMENT_DONE_TYPE_NUM = 100;

		public const int COM_MAX_TROPHYLVL_TYPE_NUM = 100;

		public const int COM_MAX_ACHIEVESHOW_NUM = 3;

		public const int COM_MAX_REWARDMATCH_RECORD_NUM = 4;

		public const int COM_MAX_GUILD_MATCH_HISTORY_NUM = 40;

		public const int COM_REDDOTLABEL_REDDOT = 1;

		public const int COM_REDDOTLABEL_SAISHI = 2;

		public const int COM_REDDOTLABEL_LOWZHEKOU = 3;

		public const int COM_REDDOTLABEL_HIGHZHEKOU = 4;

		public const int COM_REDDOTLABEL_NEWPRODUCT = 5;

		public const int COM_REDDOTLABEL_SALE = 6;

		public const int COM_REDDOTLABEL_XIANDING = 7;

		public const int COM_REDDOTENTER_SAISHI = 1;

		public const int COM_REDDOTENTER_SHOP = 2;

		public const int COM_REDDOTENTER_HUODONG = 3;

		public const int COM_REDDOTENTER_SHOPTUIJIAN = 4;

		public const int COM_REDDOTENTER_SHOPXINPIN = 5;

		public const int COM_REDDOTENTER_SHOPHERO = 6;

		public const int COM_REDDOTENTER_SHOPSKIN = 7;

		public const int COM_REDDOTENTER_SHOPTEHUI = 8;

		public const int COM_GUILDMATCH_INVITECARD_STATE_NULL = 0;

		public const int COM_GUILDMATCH_INVITECARD_STATE_ACCEPT = 1;

		public const int COM_GUILDMATCH_INVITECARD_STATE_REFUSE = 2;

		public const int COM_SNSGENDER_NONE = 0;

		public const int COM_SNSGENDER_MALE = 1;

		public const int COM_SNSGENDER_FEMALE = 2;

		public const int COM_ISP_NULL = 0;

		public const int COM_ISP_TELECOM = 1;

		public const int COM_ISP_UNICOM = 2;

		public const int COM_ISP_MOBILE = 3;

		public const int COM_ISP_HK = 4;

		public const int COM_ISP_COMM = 5;

		public const int COM_ISP_LIMIT = 6;

		public const int COM_PLAYERCAMP_MID = 0;

		public const int COM_PLAYERCAMP_1 = 1;

		public const int COM_PLAYERCAMP_2 = 2;

		public const int COM_PLAYERCAMP_COUNT = 3;

		public const int COM_ROOM_FROM_INTERNAL = 0;

		public const int COM_ROOM_FROM_QQSPROT = 1;

		public const int COM_LIKETYPE_TEAMMATE = 0;

		public const int COM_LIKETYPE_OPPONENT = 1;

		public const int COM_ROOMCHG_PLAYERADD = 0;

		public const int COM_ROOMCHG_PLAYERLEAVE = 1;

		public const int COM_ROOMCHG_SELFBEKICK = 2;

		public const int COM_ROOMCHG_STATECHG = 3;

		public const int COM_ROOMCHG_MASTERCHG = 4;

		public const int COM_ROOMCHG_CHGMEMBERPOS = 5;

		public const int DEL_FRIEND_SELF = 1;

		public const int DEL_FRIEND_PEER = 2;

		public const int DEL_FRIEND_REQ_SELF = 1;

		public const int DEL_FRIEND_REQ_PEER = 2;

		public const int ADD_FRIEND_SELF = 1;

		public const int ADD_FRIEND_PEER = 2;

		public const int COM_OBJTYPE_NULL = 0;

		public const int COM_OBJTYPE_ACCOUNT = 1;

		public const int COM_OBJTYPE_ITEMPROP = 2;

		public const int COM_OBJTYPE_ITEMEQUIP = 3;

		public const int COM_OBJTYPE_HERO = 4;

		public const int COM_OBJTYPE_ITEMSYMBOL = 5;

		public const int COM_OBJTYPE_ITEMGEAR = 6;

		public const int COM_OBJTYPE_HEROSKIN = 7;

		public const int COM_OBJTYPE_HEADIMG = 8;

		public const int COM_OBJTYPE_MAX = 9;

		public const int COM_COINTYPE_COIN = 0;

		public const int COM_COINTYPE_PVPCOIN = 1;

		public const int COM_COINTYPE_BURNING_COIN = 2;

		public const int COM_COINTYPE_ARENA_COIN = 3;

		public const int COM_COINTYPE_SKINCOIN = 4;

		public const int COM_COINTYPE_SYMBOLCOIN = 5;

		public const int COM_COINTYPE_DIAMOND = 6;

		public const int COM_COINTYPE_SCORE = 7;

		public const int COM_COINTYPE_MAX = 8;

		public const int COM_COINTYPE_COUPONS = 9;

		public const int COM_COINTYPE_MIXPAY = 10;

		public const int COM_ACNT_BANTIME_LOGINLIMIT = 0;

		public const int COM_ACNT_BANTIME_COUPONSFROZEN = 1;

		public const int COM_ACNT_BANTIME_COINFROZEN = 2;

		public const int COM_ACNT_BANTIME_PVPCOINFROZEN = 3;

		public const int COM_ACNT_BANTIME_ARENACOINFROZEN = 4;

		public const int COM_ACNT_BANTIME_BURNINGCOINFROZEN = 5;

		public const int COM_ACNT_BANTIME_GUILDCOINFROZEN = 6;

		public const int COM_ACNT_BANTIME_SKINCOINFROZEN = 7;

		public const int COM_ACNT_BANTIME_DENYCHAT = 8;

		public const int COM_ACNT_BANTIME_BANPLAYPVP = 9;

		public const int COM_ACNT_BANTIME_BANPLAYPVE = 10;

		public const int COM_ACNT_BANTIME_BANPLAYZHONGSHILIAN = 11;

		public const int COM_ACNT_BANTIME_BANPLAYBURNING = 12;

		public const int COM_ACNT_BANTIME_BANPLAYMENGJING = 13;

		public const int COM_ACNT_BANTIME_BANPLAYAREAN = 14;

		public const int COM_ACNT_BANTIME_SYMBOLCOINFROZEN = 15;

		public const int COM_ACNT_BANTIME_DIAMONDFROZEN = 16;

		public const int COM_ACNT_BANTIME_BANTRANK = 17;

		public const int COM_ACNT_BANTIME_ZEROPROFIT = 18;

		public const int COM_ACNT_BANTIME_SCOREFROZEN = 19;

		public const int COM_ACNT_BANTIME_ADDFRIEND = 20;

		public const int COM_ACNT_BANTIME_LADDER = 21;

		public const int COM_ACNT_BANTIME_FIVEVSFIVE = 22;

		public const int COM_ACNT_BANTIME_THREEVSTHREE = 23;

		public const int COM_ACNT_BANTIME_ONEVSONE = 24;

		public const int COM_ACNT_BANTIME_ENTERTAINMENT = 25;

		public const int COM_ACNT_BANTIME_MAX = 26;

		public const int COM_TASK_ONGOING = 0;

		public const int COM_TASK_HAVEDONE = 1;

		public const int COM_TASK_DISCARD = 2;

		public const int COM_TASK_COMMIT = 3;

		public const int COM_PROP_GIFT_USE_TYPE_NULL = 0;

		public const int COM_PROP_GIFT_USE_TYPE_IMMI = 1;

		public const int COM_PROP_GIFT_USE_TYPE_DELAY = 2;

		public const int COM_BATTLE_FINISH_TYPE_NORMAL = 1;

		public const int COM_BATTLE_FINISH_TYPE_ABNORMAL = 2;

		public const int COM_REWARDS_TYPE_COIN = 0;

		public const int COM_REWARDS_TYPE_ITEM = 1;

		public const int COM_REWARDS_TYPE_EXP = 2;

		public const int COM_REWARDS_TYPE_COUPONS = 3;

		public const int COM_REWARDS_TYPE_EQUIP = 4;

		public const int COM_REWARDS_TYPE_HERO = 5;

		public const int COM_REWARDS_TYPE_SYMBOL = 6;

		public const int COM_REWARDS_TYPE_BURNING_COIN = 7;

		public const int COM_REWARDS_TYPE_ARENA_COIN = 8;

		public const int COM_REWARDS_TYPE_AP = 9;

		public const int COM_REWARDS_TYPE_SKIN = 10;

		public const int COM_REWARDS_TYPE_HONOUR = 11;

		public const int COM_REWARDS_TYPE_HEROPOOLEXP = 12;

		public const int COM_REWARDS_TYPE_SKINCOIN = 13;

		public const int COM_REWARDS_TYPE_SYMBOLCOIN = 14;

		public const int COM_REWARDS_TYPE_PVPEXP = 15;

		public const int COM_REWARDS_TYPE_DIAMOND = 16;

		public const int COM_REWARDS_TYPE_HUOYUEDU = 17;

		public const int COM_REWARDS_TYPE_MATCH_POINT_PERSON = 18;

		public const int COM_REWARDS_TYPE_MATCH_POINT_GUILD = 19;

		public const int COM_REWARDS_TYPE_HEADIMAGE = 20;

		public const int COM_REWARDS_TYPE_ACHIEVE = 21;

		public const int COM_REWARDS_TYPE_MASTERPOINT = 22;

		public const int COM_REWARDS_TYPE_MAX = 23;

		public const int COM_REWARDS_FROM_NULL = 0;

		public const int COM_REWARDS_FROM_HERO = 1;

		public const int COM_REWARDS_FROM_SKIN = 2;

		public const int COM_LEVEL_TYPE_NULL = 0;

		public const int COM_LEVEL_TYPE_SINGLE = 1;

		public const int COM_LEVEL_TYPE_MULTI = 2;

		public const int COM_LEVEL_TYPE_MAX = 3;

		public const int COM_BATTLE_RESULT_TYPE_NULL = 0;

		public const int COM_BATTLE_RESULT_TYPE_WIN = 1;

		public const int COM_BATTLE_RESULT_TYPE_LOSE = 2;

		public const int COM_BATTLE_RESULT_TYPE_DRAW = 3;

		public const int COM_BATTLE_PLAY_MODE_BALANCE = 1;

		public const int COM_BATTLE_PLAY_MODE_ENTERTAINMENT = 2;

		public const int COM_BATTLE_PLAY_MODE_MAX = 3;

		public const int COM_BATTLE_GAME_TYPE_MATCH = 1;

		public const int COM_BATTLE_GAME_TYPE_NORMAL = 2;

		public const int COM_MAIL_SYSTEM = 1;

		public const int COM_MAIL_FRIEND = 2;

		public const int COM_MAIL_FRIEND_INVITE = 3;

		public const int COM_MAIL_SYSTEM_NOTICE = 1;

		public const int COM_MAIL_SYSTEM_ACCESS = 2;

		public const int COM_MAIL_FRIEND_COM = 1;

		public const int COM_MAIL_FRIEND_HEART = 2;

		public const int COM_MAIL_GUILD = 3;

		public const int COM_MAIL_UNREAD = 1;

		public const int COM_MAIL_HAVEREAD = 2;

		public const int COM_MAILACCESSMONEY_COIN = 1;

		public const int COM_MAILACCESSMONEY_COUPONS = 2;

		public const int COM_MAILACCESSMONEY_RACEDAIBI = 3;

		public const int COM_MAILACCESSMONEY_BURNINGDAIBI = 4;

		public const int COM_MAILACCESSMONEY_GUILDSHOPDAIBI = 5;

		public const int COM_MAILACCESSMONEY_SYMBOLCOIN = 6;

		public const int COM_MAILACCESSMONEY_DIAMOND = 7;

		public const int COM_MAILACCESS_PROP = 1;

		public const int COM_MAILACCESS_MONEY = 2;

		public const int COM_MAILACCESS_HEART = 3;

		public const int COM_MAILACCESS_RONGYU = 4;

		public const int COM_MAILACCESS_EXP = 5;

		public const int COM_MAILACCESS_HERO = 6;

		public const int COM_MAILACCESS_PIFU = 7;

		public const int COM_MAILACCESS_EXPHERO = 8;

		public const int COM_MAILACCESS_EXPSKIN = 9;

		public const int COM_MAILACCESS_HEADIMG = 10;

		public const int COM_MAILACCESS_MASTERPOINT = 11;

		public const int COM_MAILACCESSGET_UNGET = 1;

		public const int COM_MAILACCESSGET_GETED = 2;

		public const int COM_MAILOPTRESULT_SENDSUCCESS = 1;

		public const int COM_MAILOPTRESULT_SENDFAIL = 2;

		public const int COM_MAILOPTRESULT_NEWMAIL = 3;

		public const int COM_MAILOPTRESULT_PACKAGEFULL = 4;

		public const int COM_MAILOPTRESULT_PACKAGECLEAN = 5;

		public const int COM_MAILOPTRESULT_LISTGETING = 6;

		public const int COM_MAILOPTRESULT_SAMEVERSION = 7;

		public const int COM_MAILOPTRESULT_GETHEARTLIMIT = 8;

		public const int COM_MAILOPTRESULT_MAILTITLE = 9;

		public const int COM_MAILOPTRESULT_MAILCONTENT = 10;

		public const int COM_MAILOPTRESULT_ALLGETED = 11;

		public const int COM_MAILSOURCE_GM = 1;

		public const int COM_MAILSOURCE_GLOABALREWARD = 2;

		public const int COM_MAILSOURCE_GUILD = 3;

		public const int COM_MAILSOURCE_IDIP = 4;

		public const int COM_MAILSOURCE_TASK = 5;

		public const int COM_MAILSOURCE_AREAN = 6;

		public const int COM_MAILSOURCE_ACTIVITY = 7;

		public const int COM_MAILSOURCE_PAY = 8;

		public const int COM_MAILSOURCE_ITEM = 9;

		public const int COM_MAILSOURCE_SWEEP = 10;

		public const int COM_MAILSOURCE_PVP = 11;

		public const int COM_MAILSOURCE_PVE = 12;

		public const int COM_MAILSOURCE_GAMEVIP = 13;

		public const int COM_MAILSOURCE_RANK = 14;

		public const int COM_MAILSOURCE_HERO = 15;

		public const int COM_MAILSOURCE_ACHIEVEMENT = 16;

		public const int COM_MAILSOURCE_LUCKYDRAW = 17;

		public const int COM_MAILSOURCE_EXPCARD = 18;

		public const int COM_MAILSOURCE_RANDDRAW = 19;

		public const int COM_MAILSOURCE_MONTHWEEK = 20;

		public const int COM_MAILSOURCE_GAINCHEST = 21;

		public const int COM_MAILSOURCE_REWARDMATCH = 22;

		public const int COM_MAILSOURCE_FRIENDDONATE = 23;

		public const int COM_MAILSOURCE_LUCKYREWARD = 24;

		public const int COM_MAILSOURCE_CREDITREWARD = 25;

		public const int COM_MAILSOURCE_GAMECENTER = 26;

		public const int COM_MAILSOURCE_TROPHY = 27;

		public const int COM_MAILSOURCE_RECRUITMENT = 28;

		public const int COM_MAILSOURCE_LBS = 29;

		public const int COM_MAILSOURCE_XUNYOUCHK = 30;

		public const int COM_MAILSOURCE_ASKFORREQ = 31;

		public const int COM_BATTLE_MAP_TYPE_VERSUS = 1;

		public const int COM_BATTLE_MAP_TYPE_COUNTERPART = 2;

		public const int COM_BATTLE_MAP_TYPE_RANK = 3;

		public const int COM_BATTLE_MAP_TYPE_ENTERTAINMENT = 4;

		public const int COM_BATTLE_MAP_TYPE_REWARDMATCH = 5;

		public const int COM_BATTLE_MAP_TYPE_GUILDMATCH = 6;

		public const int COM_BATTLE_MAP_TYPE_NUM = 7;

		public const int COM_ROOM_TYPE_NULL = 0;

		public const int COM_ROOM_TYPE_MATCH = 1;

		public const int COM_ROOM_TYPE_NORMAL = 2;

		public const int COM_ROOM_TYPE_MAX = 3;

		public const int COM_GAMEPK_WITHOUT_AI = 1;

		public const int COM_GAMEPK_WITH_AI = 2;

		public const int COM_MATCH_SUCC = 1;

		public const int COM_MATCH_PROCESS = 2;

		public const int COM_MATCH_ERR = 3;

		public const int COM_MATCH_ERRCODE_BANTIME = 1;

		public const int COM_MATCH_ERRCODE_NEEDITEM = 2;

		public const int COM_MATCH_ERRCODE_WRONGTIME = 3;

		public const int COM_MATCH_ERRCODE_BEPUNISHED = 4;

		public const int COM_MATCH_ERRCODE_ABORT = 5;

		public const int COM_MATCH_ERRCODE_OTHERS = 6;

		public const int COM_MATCH_ERRCODE_CREDIT = 7;

		public const int COM_MATCH_ERRCODE_INVALID_LADDERBP = 8;

		public const int COM_MATCH_ERRCODE_FORBID_LADDER = 9;

		public const int COM_MATCH_ERRCODE_BANLADDERTIME = 10;

		public const int COM_MATCH_ERRCODE_BANONEVSONE = 11;

		public const int COM_ROLLINGMSG_TYPE_IDIP = 0;

		public const int COM_ROLLINGMSG_TYPE_HORN = 1;

		public const int COM_ROLLINGMSG_TYPE_EVENT = 2;

		public const int COM_ROLLINGMSG_DISPLAY_NONE = 0;

		public const int COM_ROLLINGMSG_DISPLAY_CHAT = 1;

		public const int COM_ROLLINGMSG_DISPLAY_HORN = 2;

		public const int COM_ROLLINGMSG_DISPLAY_BOTH = 3;

		public const int COM_CHAT_MSG_TYPE_LOGIC_WORLD = 1;

		public const int COM_CHAT_MSG_TYPE_PRIVATE = 2;

		public const int COM_CHAT_MSG_TYPE_ROOM = 3;

		public const int COM_CHAT_MSG_TYPE_GUILD = 4;

		public const int COM_CHAT_MSG_TYPE_BATTLE = 5;

		public const int COM_CHAT_MSG_TYPE_TEAM = 6;

		public const int COM_CHAT_MSG_TYPE_INBATTLE = 7;

		public const int COM_CHAT_MSG_TYPE_SETTLE = 8;

		public const int COM_CHAT_MSG_TYPE_SMALL_HORN = 9;

		public const int COM_CHAT_MSG_TYPE_BIG_HORN = 10;

		public const int COM_CHAT_MSG_TYPE_GUILD_TEAM = 11;

		public const int COM_BATTLE_CHATTYPE_ID = 1;

		public const int COM_BATTLE_CHATTYPE_STR = 2;

		public const int COM_INBATTLE_CHATTYPE_SIGNAL = 1;

		public const int COM_INBATTLE_CHATTYPE_BUBBLE = 2;

		public const int COM_INBATTLE_CHATTYPE_SELFINPUT = 3;

		public const int COM_INBATTLE_CHATTYPE_SELFDEFINEINPUT = 4;

		public const int COM_ACNT_QUIT_ROOM_FROM_NULL = 0;

		public const int COM_ACNT_QUIT_ROOM_FROM_ROOM = 1;

		public const int COM_ACNT_QUIT_ROOM_FROM_TEAM = 2;

		public const int COM_AI_LEVEL_EASY_OF_NORMALBATTLE = 1;

		public const int COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE = 2;

		public const int COM_AI_LEVEL_HARD_OF_NORMALBATTLE = 3;

		public const int COM_AI_LEVEL_PROFESSIONAL_OF_NORMALBATTLE = 4;

		public const int COM_AI_LEVEL_EASY_OF_WARMBATTLE = 5;

		public const int COM_AI_LEVEL_HARD_OF_WARMBATTLE = 6;

		public const int COM_AI_LEVEL_MAX = 7;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_NEWBIE_LEVEL = 0;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_PUT_ON_EQUIP = 1;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_HERO_ADVANCED = 2;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_USE_SOUL_STONE = 3;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_HERO_STAR = 4;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_HERO_SKILL_LVUP = 5;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_OPEN_CHEST = 6;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_THREE_HEROES = 7;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_PVP_TASK_REWARD = 8;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_BUY_HERO = 9;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_BUY_ITEM_IN_SHOP = 10;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_GET_CHAPTER_REWARD = 11;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_SWEEP = 12;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_COMBAT_GAME = 13;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_MASTER_ACTIVITY = 14;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_DREAM_ACTIVITY = 15;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_BURNING = 16;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_ELITE_ADVENTURE = 17;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_ENTER_GUILD = 18;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_USE_SYMBOL = 19;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_ENTER_SECRET_SHOP = 20;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_PVP_GUIDE = 21;

		public const int COM_ACNT_NEWBIT_STATUS_TYPE_COUPONS_FIRSTPAY = 22;

		public const int COM_ACNT_NEWBIT_STATUS_TYPE_COUPONS_FIRSTREWARD = 23;

		public const int COM_ACNT_NEWBIT_STATUS_TYPE_COUPONS_RENEWAL = 24;

		public const int COM_ACNT_NEWBIT_STATUS_TYPE_COUPONS_RENEWREWARD = 25;

		public const int COM_ACNT_NEWBIT_STATUS_PVPMATCH3V3_WITH_AI = 26;

		public const int COM_ACNT_NEWBIT_STATUS_PVPMATCH3V3_WITHOUT_AI = 27;

		public const int COM_ACNT_NEWBIT_STATUS_ENTER_PVPCOIN_SHOP = 28;

		public const int COM_ACNT_NEWBIT_STATUS_LEVELUP_GEAR = 29;

		public const int COM_ACNT_NEWBIT_STATUS_ADVANCE_GEAR = 30;

		public const int COM_ACNT_NEWBIT_STATUS_USE_EXP_ITEM = 31;

		public const int COM_ACNT_NEWBIT_STATUS_LEVELUP_HERO = 32;

		public const int COM_ACNT_NEWBIT_STATUS_ENTER_ARENA = 33;

		public const int COM_ACNT_NEWBIT_STATUS_PVE_TASK_REWARD = 34;

		public const int COM_ACNT_NEWBIT_STATUS_START_NEWBIE_LEVEL_1 = 35;

		public const int COM_ACNT_NEWBIT_STATUS_START_NEWBIE_LEVEL_2 = 36;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_MASTER_ACTIVITY_ENDOK = 39;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_ARENA_ENDOK = 40;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_DREAM_ENDOK = 41;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_BURNING_ENDOK = 42;

		public const int COM_ACNT_NEWBIT_STATUS_QQVIP_REGISTERGIFT = 43;

		public const int COM_ACNT_NEWBIT_STATUS_COMBAT_3V3 = 44;

		public const int COM_ACNT_NEWBIT_STATUS_COMBAT_3V3_START = 45;

		public const int COM_ACNT_NEWBIT_STATUS_SINGLE_3V3_ENTER = 46;

		public const int COM_ACNT_NEWBIT_STATUS_SINGLE_3V3_START = 47;

		public const int COM_ACNT_NEWBIT_STATUS_1_1_1_START = 48;

		public const int COM_ACNT_NEWBIT_STATUS_1_1_1_FIN = 49;

		public const int COM_ACNT_NEWBIT_STATUS_ARENA_SET = 50;

		public const int COM_ACNT_NEWBIT_STATUS_COMBAT_3V3_ENTER = 51;

		public const int COM_ACNT_NEWBIT_STATUS_1_1_1_START_33NOTFIN = 52;

		public const int COM_ACNT_NEWBIT_STATUS_1_1_1_FIN_33NOTFIN = 53;

		public const int COM_ACNT_NEWBIT_STATUS_SINGLE_33_ENTER_1_1_1_NOT = 54;

		public const int COM_ACNT_NEWBIT_STATUS_SINGLE_33_START_1_1_1_NOT = 55;

		public const int COM_ACNT_NEWBIT_STATUS_SINGLE_33_WIN_1_1_1_NOT = 56;

		public const int COM_ACNT_NEWBIT_STATUS_SINGLE_33_FAIL_1_1_1_NOT = 57;

		public const int COM_ACNT_NEWBIT_STATUS_MAKE_SYMBOL = 58;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MATCHSUCC = 59;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_AUTOCANCEL = 60;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL = 61;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MATCHSUCC_END = 62;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_AUTOCANCEL_START = 63;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_AUTOCANCEL_END = 64;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_11MATCH_ENTER = 65;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_11MATCH_START = 66;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_11MATCH_END = 67;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33MATCH_ENTER = 68;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33MATCH_START = 69;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33MATCH_END = 70;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33AI_ENTER = 71;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33AI_START = 72;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33AI_END = 73;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33AISINGLE_START = 74;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_33AISINGLE_END = 75;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_11AISINGLE_START = 76;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_11AISINGLE_END = 77;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_1_1_START = 78;

		public const int COM_ACNT_NEWBIT_STATUS_TEAM33AI_MANUELCANCEL_1_1_END = 79;

		public const int COM_ACNT_NEWBIT_STATUS_COIN_DRAW_FIVE = 80;

		public const int COM_ACNT_NEWBIT_STATUS_SINGLE_1V1_END = 81;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_TRAINING_GUIDE = 82;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_CORONA_GUIDE = 83;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_JUNGLE_GUIDE = 84;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_3V3_GUIDE = 85;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_FIRST_1V1_WARM_BATTLE_START = 86;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_FIRST_1V1_WARM_BATTLE_END = 87;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_FIRST_5V5_WARM_BATTLE_START = 88;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_FIRST_5V5_WARM_BATTLE_WIN = 89;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_FIRST_5V5_WARM_BATTLE_LOSE = 90;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_CORONA_GUIDE_START = 91;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_SECOND_5V5_WARM_BATTLE_START = 92;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_SECOND_5V5_WARM_BATTLE_WIN = 93;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_SECOND_5V5_WARM_BATTLE_LOSE = 94;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_REAL_5V5_START = 95;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_REAL_5V5_WIN = 96;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_REAL_5V5_LOSE = 97;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_5V5_GUIDE = 98;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_GAMEVIP_GIFT = 99;

		public const int COM_ACNT_NEWBIT_STATUS_DIAMOND_DRAW = 100;

		public const int COM_ACNT_NEWBIT_STATUS_FIRST_ENTERTAINMENT_START = 101;

		public const int COM_ACNT_NEWBIT_STATUS_FIRST_ENTERTAINMENT_END = 102;

		public const int COM_ACNT_NEWBIT_STATUS_FIRST_MATCH_PKAI_START = 103;

		public const int COM_ACNT_NEWBIT_STATUS_FIRST_MATCH_PKAI_END = 104;

		public const int COM_ACNT_NEWBIT_STATUS_OLDPLAYER = 105;

		public const int COM_ACNT_NEWBIE_STATUS_TYPE_MAX = 106;

		public const int COM_ACNT_CLIENT_BITS_TYPE_KILL_SOLDIER = 0;

		public const int COM_ACNT_CLIENT_BITS_TYPE_DESTORY_ARROWTOWER = 1;

		public const int COM_ACNT_CLIENT_BITS_TYPE_DESTORY_BASETOWER = 2;

		public const int COM_ACNT_CLIENT_BITS_TYPE_DESTORY_MONSTERHOME = 3;

		public const int COM_ACNT_CLIENT_BITS_TYPE_KILL_LITTLEDRAGON = 4;

		public const int COM_ACNT_CLIENT_BITS_TYPE_LEARN_TALENT = 5;

		public const int COM_ACNT_CLIENT_BITS_TYPE_UPGRADE_EQUIP = 6;

		public const int COM_ACNT_CLIENT_BITS_TYPE_GET_HERO = 7;

		public const int COM_ACNT_CLIENT_BITS_TYPE_KILL_HERO = 8;

		public const int COM_ACNT_CLIENT_BITS_TYPE_COMBAT_3V3_ENTER = 9;

		public const int COM_ACNT_CLIENT_BITS_TYPE_COMBAT_3V3_LOADOK = 10;

		public const int COM_ACNT_CLIENT_BITS_TYPE_SINGLE_3V3_LOADOK = 11;

		public const int COM_ACNT_CLIENT_BITS_TYPE_1_1_1_ENTER = 12;

		public const int COM_ACNT_CLIENT_BITS_TYPE_1_1_1_LOADOK = 13;

		public const int COM_ACNT_CLIENT_BITS_TYPE_SYMBOL_ENTER = 14;

		public const int COM_ACNT_CLIENT_BITS_TYPE_ARENA_ENTER = 15;

		public const int COM_ACNT_CLIENT_BITS_TYPE_BURNING_ENTER = 16;

		public const int COM_ACNT_CLIENT_BITS_TYPE_OLDPLAYERGUILDED = 17;

		public const int COM_ACNT_CLIENT_BITS_TYPE_MAX = 18;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_NEWBITGIFT_SHOW = 0;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_PROPITEM_ADD = 1;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_GIFTITEM_ADD = 2;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_SHOW_HERO_TRICK_TIPS = 3;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_ShOW_LBS_GUIDE = 4;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_Show_MoreHero_Guide = 5;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_SHOW_ACTIVE_EQUIP_GUIDE = 6;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_SHOW_GUILD_PLATFORM_GROUP_GUIDE = 7;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_CLTUSE_START = 100;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_CLTUSE_END = 200;

		public const int COM_ACNT_NEWCLIENT_BITS_TYPE_MAX = 201;

		public const int COM_ACNT_NEWBIE_TYPE_NULL = 0;

		public const int COM_ACNT_NEWBIE_TYPE_NEWBIE = 1;

		public const int COM_ACNT_NEWBIE_TYPE_OLDBIRD = 2;

		public const int COM_ACNT_NEWBIE_TYPE_MASTER = 3;

		public const int COM_ACNT_NEWBIE_TYPE_MAX = 4;

		public const int COM_ACNT_SET_HERO = 0;

		public const int COM_ACNT_REMOVE_HERO = 1;

		public const int COM_ACNT_SWAP_HERO = 2;

		public const int COM_ROOM_MEMBER_TNULL = 0;

		public const int COM_ROOM_MEMBER_TACNT = 1;

		public const int COM_ROOM_MEMBER_TNPC = 2;

		public const int COM_ROOM_NPC_LEVELOF_EASY = 0;

		public const int COM_ROOM_NPC_LEVELOF_HARD = 1;

		public const int COM_SHOPDRAW_COIN = 0;

		public const int COM_SHOPDRAW_HERO = 1;

		public const int COM_SHOPDRAW_SKIN = 2;

		public const int COM_SHOPDRAW_SYMBOLCOMMON = 3;

		public const int COM_SHOPDRAW_SYMBOLSENIOR = 4;

		public const int COM_SHOPDRAW_MAX = 5;

		public const int COM_SHOPDRAW_SUB_FREE = 1;

		public const int COM_SHOPDRAW_SUB_ONE = 2;

		public const int COM_SHOPDRAW_SUB_FIVE = 3;

		public const int COM_SHOPDRAW_SUB_TEN = 4;

		public const int COM_SHOPDRAW_SUB_MAX = 5;

		public const int COM_SHOPBUY_LIMIT_COIN = 0;

		public const int COM_SHOPBUY_LIMIT_AP = 1;

		public const int COM_SHOPBUY_LIMIT_SP = 2;

		public const int COM_SHOPBUY_LIMIT_MAX = 3;

		public const int COM_SINGLE_GAME_OF_ADVENTURE = 0;

		public const int COM_SINGLE_GAME_OF_COMBAT = 1;

		public const int COM_SINGLE_GAME_OF_GUIDE = 2;

		public const int COM_SINGLE_GAME_OF_ACTIVITY = 3;

		public const int COM_MULTI_GAME_OF_LADDER = 4;

		public const int COM_MULTI_GAME_OF_PVP_MATCH = 5;

		public const int COM_MULTI_GAME_OF_PVP_ROOM = 6;

		public const int COM_SINGLE_GAME_OF_BURNING = 7;

		public const int COM_SINGLE_GAME_OF_ARENA = 8;

		public const int COM_MULTI_GAME_OF_ENTERTAINMENT = 9;

		public const int COM_MULTI_GAME_OF_REWARDMATCH = 10;

		public const int COM_MULTI_GAME_OF_GUILDMATCH = 11;

		public const int COM_GAME_TYPE_MAX = 12;

		public const int COM_INVITE_JOIN_NULL = 0;

		public const int COM_INVITE_JOIN_ROOM = 1;

		public const int COM_INVITE_JOIN_TEAM = 2;

		public const int COM_INVITE_JOIN_MAX = 3;

		public const int COM_INVITE_RELATION_FRIEND = 0;

		public const int COM_INVITE_RELATION_GUILDMEMBER = 1;

		public const int COM_INVITE_RELATION_LBS = 2;

		public const int COM_INVITE_RELATION_MAX = 3;

		public const int COM_TEAMCHG_PLAYERADD = 0;

		public const int COM_TEAMCHG_PLAYERLEAVE = 1;

		public const int COM_TEAMCHG_MASTERCHG = 2;

		public const int COM_TEAMOPER_NULL = 0;

		public const int COM_TEAMOPER_START_MULTI_MATCH = 1;

		public const int COM_TEAMOPER_START_KICKOUT_MEMBER = 2;

		public const int COM_ACNT_LEAVE_TEAM_LOGOUT = 1;

		public const int COM_ACNT_LEAVE_TEAM_REQLEAVE = 2;

		public const int COM_RANK_STATE_OF_NULL = 0;

		public const int COM_RANK_STATE_OF_SEASONPROCESS = 1;

		public const int COM_RANK_STATE_OF_SEASONOVER = 2;

		public const int COM_RANK_SUBSTATE_OF_NULL = 0;

		public const int COM_RANK_SUBSTATE_OF_NORMAL = 1;

		public const int COM_RANK_SUBSTATE_OF_GRADEUP = 2;

		public const int COM_RANK_SUBSTATE_OF_GRADEDOWN = 3;

		public const int COM_RANK_GRADEUP_SUCC = 1;

		public const int COM_RANK_GRADEUP_FAIL = 2;

		public const int COM_RANK_GRADEUP_UNDEFINE = 3;

		public const int COM_ACTIVITY_OPEN_WEEKMASK = 0;

		public const int COM_ACTIVITY_OPEN_RANDOMLOOP = 1;

		public const int COM_ADD_FRIEND_NULL = 0;

		public const int COM_ADD_FRIEND_PVP = 1;

		public const int COM_ADD_FRIEND_LBS = 2;

		public const int COM_RECRUITMENT_NULL = 0;

		public const int COM_RECRUITMENT_ACTIVE = 1;

		public const int COM_RECRUITMENT_PASSIVE = 2;

		public const int COM_CHG_RECRUITMENT_TYPE_ADD = 1;

		public const int COM_CHG_RECRUITMENT_TYPE_DEL = 2;

		public const int COM_FRIEND_CONFIRM_NORMAL = 0;

		public const int COM_FRIEND_CONFIRM_INTIMACY = 1;

		public const int COM_FRIEND_CONFIRM_RECRUITMENT = 2;

		public const int COM_FRIEND_RELATION_RECRUITMENT = 1;

		public const int COM_FRIEND_RELATION_RECRUITED = 2;

		public const int COM_CHGFRIEND_EVENT_DEL = 1;

		public const int COM_CHGFRIEND_EVENT_ADD = 2;

		public const int COM_CHGFRIEND_EVENT_ADDBYINTIMACY = 3;

		public const int COM_CHGFRIEND_EVENT_ADDBYRECRUIT = 4;

		public const int COM_CHGFRIEND_EVENT_DELBYASSIT = 5;

		public const int COM_CHGFRIEND_EVENT_DELBYBLACK = 6;

		public const int COM_CHGFRIEND_EVENT_ADDBYASSIT = 7;

		public const int COM_TRANSACTION_CONTEXT_RANK = 1;

		public const int COM_TRANSACTION_CONTEXT_ONLINECHK = 2;

		public const int COM_TRANSACTION_CONTEXT_JOIN_GUILD = 3;

		public const int COM_TRANSACTION_CONTEXT_APPROVE_JOIN_GUILD = 4;

		public const int COM_TRANSACTION_CONTEXT_QUIT_GUILD = 5;

		public const int COM_TRANSACTION_CONTEXT_GUILD_INVITE = 6;

		public const int COM_TRANSACTION_CONTEXT_SEARCH_GUILD = 7;

		public const int COM_TRANSACTION_CONTEXT_DEAL_GUILD_INVITE = 8;

		public const int COM_TRANSACTION_CONTEXT_GUILD_RECOMMEND = 9;

		public const int COM_TRANSACTION_CONTEXT_GUILD_GETNAME = 10;

		public const int COM_TRANSACTION_CONTEXT_GETARENA_TARGETDATA = 11;

		public const int COM_TRANSACTION_CONTEXT_APPOINT_POSITION = 12;

		public const int COM_TRANSACTION_CONTEXT_FIRE_MEMBER = 13;

		public const int COM_TRANSACTION_CONTEXT_ADD_ARENAFIGHT_HISTORY = 14;

		public const int COM_TRANSACTION_CONTEXT_DELETEBURNINGENEMY = 15;

		public const int COM_TRANSACTION_CONTEXT_TRANS_BASE_INFO = 16;

		public const int COM_TRANSACTION_CONTEXT_ADD_RANKCURSEASONDATA = 17;

		public const int COM_TRANSACTION_CONTEXT_ADD_RANKPASTSEASONDATA = 18;

		public const int COM_TRANSACTION_CONTEXT_LUCKYDRAW = 19;

		public const int COM_TRANSACTION_CONTEXT_CHANGE_PLAYER_NAME = 20;

		public const int COM_TRANSACTION_CONTEXT_CHANGE_GUILD_NAME = 21;

		public const int COM_TRANSACTION_CONTEXT_ADD_FIGHTHISTORY = 22;

		public const int COM_TRANSACTION_CONTEXT_TRANS_REGISTER_INFO = 23;

		public const int COM_TRANSACTION_CONTEXT_GETACNTMOBAINFO = 24;

		public const int COM_TRANSACTION_CONTEXT_APPLYMASTERREQ = 25;

		public const int COM_TRANSACTION_CONTEXT_CONFIRMMASTERREQ = 26;

		public const int COM_TRANSACTION_CONTEXT_APPLYSTUDENTREQ = 27;

		public const int COM_TRANSACTION_CONTEXT_CONFIRMSTUDENTREQ = 28;

		public const int COM_TRANSACTION_CONTEXT_REMOVEMASTERREQ = 29;

		public const int COM_TRANSACTION_CONTEXT_GETPROCESS_STUDENTINFO = 30;

		public const int COM_TRANSACTION_CONTEXT_GETGRADUATE_STUDENTINFO = 31;

		public const int COM_TRANSACTION_CONTEXT_GRADUATE_FROM_MASTER = 32;

		public const int COM_TRANSACTION_CONTEXT_GET_FIGHT_HISTORY = 33;

		public const int COM_TRANSACTION_CONTEXT_ADD_FRIEND_CONFIRM = 34;

		public const int COM_TRANSACTION_CONTEXT_DEL_FRIEND = 35;

		public const int COM_TRANSACTION_CONTEXT_SEARCH_PALYER = 36;

		public const int COM_TRANSACTION_CONTEXT_ADD_FRIEND = 37;

		public const int COM_TRANSACTION_CONTEXT_ADD_FRIEND_DENY = 38;

		public const int COM_TRANSACTION_CONTEXT_GIFTUSE = 39;

		public const int COM_TRANSACTION_ACTION_SUCC = 1;

		public const int COM_TRANSACTION_ACTION_PROCESS = 2;

		public const int COM_TRANSACTION_ACTION_FAIL = 3;

		public const int COM_TRANSACTION_TCAP_INSERTRANKDETAIL = 1;

		public const int COM_TRANSACTION_TCAP_DELETERANKDETAIL = 2;

		public const int COM_TRANSACTION_TCAP_UPDATERANKCLASSID = 3;

		public const int COM_TRANSACTION_TCAP_GETONLINEINFO = 4;

		public const int COM_TRANSACTION_TCAP_INSERTONLINEINFO = 5;

		public const int COM_TRANSACTION_TCAP_UPDONLINEINFO = 6;

		public const int COM_TRANSACTION_TCAP_INSERTGUILDMEMBER = 7;

		public const int COM_TRANSACTION_TCAP_UPDACNTGUILDINFO = 8;

		public const int COM_TRANSACTION_TCAP_GETACNTINFO = 9;

		public const int COM_TRANSACTION_TCAP_DELJOINGUILDREQ = 10;

		public const int COM_TRANSACTION_TCAP_DELGUILDMEMBER = 11;

		public const int COM_TRANSACTION_TCAP_GETGUILDINVITE = 12;

		public const int COM_TRANSACTION_TCAP_UPDGUILDINVITETIME = 13;

		public const int COM_TRANSACTION_TCAP_INSERTGUILDINVITE = 14;

		public const int COM_TRANSACTION_TCAP_GETGUILDID = 15;

		public const int COM_TRANSACTION_TCAP_GETGUILDMAIN = 16;

		public const int COM_TRANSACTION_TCAP_DELGUILDINVITE = 17;

		public const int COM_TRANSACTION_TCAP_INSERTGUILDRECOMMEND = 18;

		public const int COM_TRANSACTION_TCAP_GETGLOBALREWARD = 19;

		public const int COM_TRANSACTION_TCAP_GETARENA_TARGETHERO = 20;

		public const int COM_TRANSACTION_TCAP_GETARENA_TARGETITEM = 21;

		public const int COM_TRANSACTION_TCAP_ADDARENA_FIGHTHISTORY = 22;

		public const int COM_TRANSACTION_TCAP_DELETE_BURNINGENEMY = 23;

		public const int COM_TRANSACTION_TCAP_INSERT_BURNINGENEMY = 24;

		public const int COM_TRANSACTION_TCAP_UPDVISITORSVRTRANSFLAG = 25;

		public const int COM_TRANSACTION_TCAP_GETACNTINFOFROMVISITORSVR = 26;

		public const int COM_TRANSACTION_TCAP_GETHEROINFOFROMVISITORSVR = 27;

		public const int COM_TRANSACTION_TCAP_GETITEMINFOFROMVISITORSVR = 28;

		public const int COM_TRANSACTION_TCAP_INSERTREGISTERINFO = 29;

		public const int COM_TRANSACTION_TCAP_INSERTACNTINFO = 30;

		public const int COM_TRANSACTION_TCAP_INSERTHEROINFO = 31;

		public const int COM_TRANSACTION_TCAP_INSERTITEMINFO = 32;

		public const int COM_TRANSACTION_TCAP_ADDRANKCURSEASONRECORD = 33;

		public const int COM_TRANSACTION_TCAP_ADDRANKPASTSEASONRECORD = 34;

		public const int COM_TRANSACTION_TCAP_INSERTREGISTER = 35;

		public const int COM_TRANSACTION_TCAP_UPDACNTNAME = 36;

		public const int COM_TRANSACTION_TCAP_DELETEREGISTER = 37;

		public const int COM_TRANSACTION_TCAP_UPDGUILDNAME = 38;

		public const int COM_TRANSACTION_TCAP_ADDFIGHTHISTORYRECORD = 39;

		public const int COM_TRANSACTION_TCAP_GETDESTACNTDATA = 40;

		public const int COM_TRANSACTION_TCAP_GET_MASTERREQNUM = 41;

		public const int COM_TRANSACTION_TCAP_INSERT_MASTERREQ = 42;

		public const int COM_TRANSACTION_TCAP_DEL_MASTERREQ = 43;

		public const int COM_TRANSACTION_TCAP_INSERT_TBMASTER = 44;

		public const int COM_TRANSACTION_TCAP_INSERT_TBSTUDENT = 45;

		public const int COM_TRANSACTION_TCAP_GET_ACNTPROFILE = 46;

		public const int COM_TRANSACTION_TCAP_DEL_TBMASTER = 47;

		public const int COM_TRANSACTION_TCAP_DEL_TBSTUDENT = 48;

		public const int COM_TRANSACTION_TCAP_UPD_TBMASTER = 49;

		public const int COM_TRANSACTION_TCAP_INSERT_TBGRADUATESTUDENT = 50;

		public const int COM_TRANSACTION_TCAP_GET_STUDENTNUM = 51;

		public const int COM_TRANSACTION_TCAP_GET_PROCESS_STUDENT_UNIQ = 52;

		public const int COM_TRANSACTION_TCAP_GET_GRADUATE_STUDENT_UNIQ = 53;

		public const int COM_TRANSACTION_TCAP_BATCHGET_ACNTPROFILE = 54;

		public const int COM_TRANSACTION_TCAP_DEL_TBGRADUATESTUDENT = 55;

		public const int COM_TRANSACTION_TCAP_GET_TBMASTER = 56;

		public const int COM_TRANSACTION_TCAP_GET_USER_PRIVACY_BITS = 57;

		public const int COM_TRANSACTION_TCAP_GET_FIGHT_HISTORY = 58;

		public const int COM_TRANSACTION_TCAP_DEL_FRIENDREQ = 59;

		public const int COM_TRANSACTION_TCAP_GET_FRIENDMAIN = 60;

		public const int COM_TRANSACTION_TCAP_GET_FRIENDNUM = 61;

		public const int COM_TRANSACTION_TCAP_INSERT_FRIEND = 62;

		public const int COM_TRANSACTION_TCAP_DEL_FRIEND = 63;

		public const int COM_TRANSACTION_TCAP_GET_REGISTERINFO = 64;

		public const int COM_TRANSACTION_TCAP_GET_FRIENDREQNUM = 65;

		public const int COM_TRANSACTION_TCAP_INSERT_FRIENDREQ = 66;

		public const int COM_TRANSACTION_MSG_GETCLASSIDREQ = 1;

		public const int COM_TRANSACTION_MSG_GETCLASSIDRSP = 2;

		public const int COM_TRANSACTION_MSG_GETGUILDNAMEREQ = 3;

		public const int COM_TRANSACTION_MSG_GETGUILDNAMERSP = 4;

		public const int COM_TRANSACTION_MSG_IDIP_SENDMAILREQ = 5;

		public const int COM_TRANSACTION_MSG_IDIP_SENDMAILRSP = 6;

		public const int COM_TRANSACTION_MSG_IDIP_BANACNTREQ = 7;

		public const int COM_TRANSACTION_MSG_IDIP_BANACNTRSP = 8;

		public const int COM_TRANSACTION_MSG_IDIP_BANTIMESYNREQ = 9;

		public const int COM_TRANSACTION_MSG_IDIP_BANTIMESYNRSP = 10;

		public const int COM_TRANSACTION_MSG_WORLD_REWARDLIMITREQ = 11;

		public const int COM_TRANSACTION_MSG_WORLD_REWARDLIMITRSP = 12;

		public const int COM_TRANSACTION_MSG_IDIP_CHGACNTINFOONLINEREQ = 13;

		public const int COM_TRANSACTION_MSG_IDIP_CHGACNTINFOONLINERSP = 14;

		public const int COM_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ = 15;

		public const int COM_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP = 16;

		public const int COM_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ = 17;

		public const int COM_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP = 18;

		public const int COM_TRANSACTION_MSG_DUPKICK = 19;

		public const int COM_TRANSACTION_MSG_ASSIST_FLAGSET = 20;

		public const int COM_TRANSACTION_MSG_IDIP_WUDAORANKREQ = 21;

		public const int COM_TRANSACTION_MSG_IDIP_WUDAORANKRSP = 22;

		public const int COM_TRANSACTION_MSG_IDIP_CHGGUILDLEADDERREQ = 23;

		public const int COM_TRANSACTION_MSG_IDIP_CHGGUILDLEADDERRSP = 24;

		public const int COM_TRANSACTION_MSG_GETACNTMOBAINFOREQ = 25;

		public const int COM_TRANSACTION_MSG_GETACNTMOBAINFORSP = 26;

		public const int COM_TRANSACTION_MSG_CHECKRECRUITINFOREQ = 27;

		public const int COM_TRANSACTION_MSG_CHECKRECRUITINFORSP = 28;

		public const int COM_TRANSACTION_MSG_ASSIST_FLAGGET = 29;

		public const int COM_TRANSACTION_MSG_IDIP_ADDITEMREQ = 30;

		public const int COM_TRANSACTION_MSG_IDIP_ADDITEMRSP = 31;

		public const int COM_TRANSACTION_MSG_IDIP_QUERYWORLDRANKREQ = 32;

		public const int COM_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP = 33;

		public const int COM_TRANSACTION_MSG_GIFTUSE_LIMITCHKREQ = 34;

		public const int COM_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP = 35;

		public const int COM_HERO_MASK_BITS_PAKAGE = 1;

		public const int COM_HERO_MASK_BITS_PVP = 2;

		public const int COM_HERO_MASK_BITS_FREE = 4;

		public const int COM_HERO_MASK_BITS_EXPERIENCE = 8;

		public const int COM_APOLLO_OPT_QUERY = 1;

		public const int COM_APOLLO_OPT_CANCEL = 2;

		public const int COM_APOLLO_OPT_PRESENT = 3;

		public const int COM_APOLLO_OPT_PAY = 4;

		public const int COM_APOLLO_OPT_MAX = 5;

		public const int COM_APOLLO_PAY_PAYSHOPBUY = 1;

		public const int COM_APOLLO_PAY_PAYITEMBUY = 2;

		public const int COM_APOLLO_PAY_PAYCOINBUY = 3;

		public const int COM_APOLLO_PAY_PAYMANUALREF = 4;

		public const int COM_APOLLO_PAY_PAYHEROBUY = 5;

		public const int COM_APOLLO_PAY_PAYGAMESWEEP = 6;

		public const int COM_APOLLO_PAY_PAYGUILDCRT = 7;

		public const int COM_APOLLO_PAY_PAYSKINBUY = 8;

		public const int COM_APOLLO_PAY_CLRCD = 9;

		public const int COM_APOLLO_PAY_PAYGUILDDONATE = 10;

		public const int COM_APOLLO_PAY_SPECSALE = 11;

		public const int COM_APOLLO_PAY_MONTHFILLIN = 12;

		public const int COM_APOLLO_PAY_DIRECTBUYITEM = 13;

		public const int COM_APOLLO_PAY_DELETE = 14;

		public const int COM_APOLLO_PAY_PVE_REVIVE = 15;

		public const int COM_APOLLO_PAY_PAYGUILDUPD = 16;

		public const int COM_APOLLO_PAY_PAYTALENTBUY = 17;

		public const int COM_APOLLO_PAY_LUCKYDRAW = 18;

		public const int COM_APOLLO_PAY_CHAT = 19;

		public const int COM_APOLLO_PAY_SALERECMD_BUY = 20;

		public const int COM_APOLLO_PAY_RANDDRAW = 21;

		public const int COM_APOLLO_PAY_PRESENTHERO = 22;

		public const int COM_APOLLO_PAY_PRESENTSKIN = 23;

		public const int COM_APOLLO_PAY_MATCHTICKET = 24;

		public const int COM_APOLLO_PAY_IDIPDELETE = 25;

		public const int COM_APOLLO_PAY_ASKFORREQ = 26;

		public const int COM_APOLLO_PAY_MAX = 27;

		public const int COM_APOLLO_TRANK_USERBUFFER_TYPE_UPLOAD_SCORE = 1;

		public const int COM_APOLLO_TRANK_USERBUFFER_TYPE_GET_TOPN = 2;

		public const int COM_APOLLO_TRANK_USERBUFFER_TYPE_GET_USER_INFO = 3;

		public const int COM_APOLLO_TRANK_USERBUFFER_TYPE_CLEAR_SPECIAL_TOPN = 4;

		public const int COM_APOLLO_TRANK_USERBUFFER_TYPE_GET_SPECIAL_SCORE = 5;

		public const int COM_APOLLO_TRANK_USERBUFFER_TYPE_GET_TOPN_LUCKY = 6;

		public const int COM_APOLLO_TRANK_USERBUFFER_TYPE_MAX = 7;

		public const int ACNT_BANNED_REASON_NONE = 0;

		public const int ACNT_BANNED_REASON_IDIP = 1;

		public const int COM_ARENAFIGHTER_HIGH = 0;

		public const int COM_ARENAFIGHTER_MID = 1;

		public const int COM_ARENAFIGHTER_LOW = 2;

		public const int COM_ARENAFIGHTER_MAX = 3;

		public const int COM_ARENAADDMEM_ERR_ALREADYIN = 1;

		public const int COM_ARENAADDMEM_ERR_BATTLELISTISNULL = 2;

		public const int COM_ARENASETBATTLELIST_ERR_FAILED = 3;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_NULL = 0;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_POWER = 1;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_PVP_EXP = 2;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_GUILD_POWER = 3;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_GUILD_RANK_POINT = 4;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_HERO_NUM = 5;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_SKIN_NUM = 6;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT = 7;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_ACHIEVEMENT = 8;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_WIN_GAMENUM = 9;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_CONTINOUS_WIN = 10;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_USE_COUPONS = 11;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_VIP_SCORE = 12;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_MASTERPOINT = 13;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_GUILD_SEASON = 16;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_CUSTOM_EQUIP = 22;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_LOW_DAY = 33;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_MID_DAY = 34;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_HIGH_DAY = 35;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_LOW_DAY = 36;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_MID_DAY = 37;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_HIGH_DAY = 38;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_LOW_DAY = 39;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_MID_DAY = 40;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_HIGH_DAY = 41;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_LOW_SEASON = 42;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_MID_SEASON = 43;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_HIGH_SEASON = 44;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_LOW_SEASON = 45;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_MID_SEASON = 46;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_HIGH_SEASON = 47;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_LOW_SEASON = 48;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_MID_SEASON = 49;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_HIGH_SEASON = 50;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_LOW_GUILD = 51;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_MID_GUILD = 52;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COINMATCH_HIGH_GUILD = 53;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_LOW_GUILD = 54;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_MID_GUILD = 55;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DIAMONDMATCH_HIGH_GUILD = 56;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_LOW_GUILD = 57;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_MID_GUILD = 58;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_COUPONSMATCH_HIGH_GUILD = 59;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_LOW_COIN_WIN = 60;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_HIGH_COIN_WIN = 61;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_LOW_DIAMOND_WIN = 62;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_HIGH_DIAMOND_WIN = 63;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_DAILY_RANKMATCH = 64;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_MASTER_HERO = 65;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_GUILD_MATCH = 66;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_GUILD_MATCH_WEEK = 67;

		public const int COM_APOLLO_TRANK_SCORE_TYPE_MAX = 68;

		public const int COM_WEAL_CHECKIN = 0;

		public const int COM_WEAL_FIXEDTIME = 1;

		public const int COM_WEAL_MULTIPLE = 2;

		public const int COM_WEAL_CONDITION = 3;

		public const int COM_WEAL_EXCHANGE = 4;

		public const int COM_WEAL_PTEXCHANGE = 5;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_NULL = 0;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_GAMETIME = 1;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_HURT = 2;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_FORMULA = 3;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_MOVE_SPEED = 4;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_SKILL_INFO = 5;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_TALENT_INFO = 6;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_INGAME = 7;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_SOULEXP = 8;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_SOLDIERCNT = 9;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_TOWER_ATTACK_DISTANCE = 10;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_POWER = 11;

		public const int COM_ANTI_CHEAT_LOGIC_TYPE_MAX = 12;

		public const int COM_INGAME_CHEAT_TYPE_NULL = 0;

		public const int COM_INGAME_CHEAT_TYPE_SKILLCD = 1;

		public const int COM_INGAME_CHEAT_TYPE_HASHNE = 2;

		public const int COM_INGAME_CHEAT_TYPE_MAX = 3;

		public const int COM_ACNT_OPER_LIMIT_GETARENAFIGHTHISTORY = 0;

		public const int COM_ACNT_OPER_LIMIT_GETFRIENDSTATE = 1;

		public const int COM_ACNT_OPER_LIMIT_GETSTUDENTLIST = 2;

		public const int COM_ACNT_OPER_LIMIT_MAX = 3;

		public const int COM_ACNT_GAME_STATE_IDLE = 0;

		public const int COM_ACNT_GAME_STATE_SINGLEGAME = 1;

		public const int COM_ACNT_GAME_STATE_MULTIGAME = 2;

		public const int COM_ACNT_GAME_STATE_TEAM = 3;

		public const int COM_ACNT_GAME_STATE_WAITMULTIGAME = 4;

		public const int COM_VIDEO_STATE_NULL = 0;

		public const int COM_VIDEO_STATE_VALID = 1;

		public const int COM_IDIP_ACNTATTR_LEVEL = 0;

		public const int COM_IDIP_ACNTATTR_PVPLEVEL = 1;

		public const int COM_IDIP_ACNTATTR_EXP = 2;

		public const int COM_IDIP_ACNTATTR_PVPEXP = 3;

		public const int COM_IDIP_ACNTATTR_COIN = 4;

		public const int COM_IDIP_ACNTATTR_ACTIONPT = 5;

		public const int COM_IDIP_ACNTATTR_PVPCOIN = 6;

		public const int COM_IDIP_ACNTATTR_ARENACOIN = 7;

		public const int COM_IDIP_ACNTATTR_BURNINGCOIN = 8;

		public const int COM_IDIP_ACNTATTR_SKINCOIN = 9;

		public const int COM_IDIP_ACNTATTR_SYMBOLCOIN = 10;

		public const int COM_IDIP_ACNTATTR_DIAMOND = 11;

		public const int COM_IDIP_ACNTATTR_COPUS = 12;

		public const int COM_IDIP_ACNTATTR_VIPSOCRE = 13;

		public const int COM_IDIP_ACNTATTR_DIAMONDLUCKYPOINT = 14;

		public const int COM_IDIP_ACNTATTR_COUPONSLUCKYPOINT = 15;

		public const int COM_IDIP_ACNTATTR_PASSWORD = 16;

		public const int COM_IDIP_ACNTATTR_MAX = 17;

		public const int COM_IDIP_HEROATTR_HERO = 0;

		public const int COM_IDIP_HEROATTR_HEROSKIN = 1;

		public const int COM_IDIP_HEROATTR_MAX = 2;

		public const int COM_REWARD_MULTIPLE_FROM_NULL = 0;

		public const int COM_REWARD_MULTIPLE_FROM_WEAL = 1;

		public const int COM_REWARD_MULTIPLE_FROM_QQVIP = 2;

		public const int COM_REWARD_MULTIPLE_FROM_PROP = 3;

		public const int COM_REWARD_MULTIPLE_FROM_PVPDAILY = 4;

		public const int COM_REWARD_MULTIPLE_FROM_WXGAMECENTERLOGIN = 5;

		public const int COM_REWARD_MULTIPLE_FROM_GUILD = 6;

		public const int COM_REWARD_MULTIPLE_FROM_FIRSTWIN = 7;

		public const int COM_REWARD_MULTIPLE_FROM_GAMEVIP = 8;

		public const int COM_REWARD_MULTIPLE_FROM_QQGAMECENTERLOGIN = 9;

		public const int COM_REWARD_MULTIPLE_FROM_IOSVISITORLOGIN = 10;

		public const int COM_REWARD_MULTIPLE_FROM_HANGUP = 11;

		public const int COM_REWARD_MULTIPLE_FROM_CREDIT = 12;

		public const int COM_REWARD_MULTIPLE_FROM_RECRUITMENT = 13;

		public const int COM_REWARD_MULTIPLE_FROM_MASTER = 14;

		public const int COM_REWARD_MULTIPLE_FROM_MAX = 15;

		public const int COM_FRIEND_TYPE_GAME = 1;

		public const int COM_FRIEND_TYPE_SNS = 2;

		public const int COM_FRIEND_TYPE_LBS = 3;

		public const int COM_FRIEND_TYPE_GUILD = 4;

		public const int COM_FRIEND_TYPE_STUDENT = 5;

		public const int COM_FRIEND_TYPE_MASTER = 6;

		public const int COM_HERO_WAKESTATE_INVALID = 0;

		public const int COM_HERO_WAKESTATE_WAIT = 1;

		public const int COM_HERO_WATESTATE_PROC = 2;

		public const int COM_HERO_WATESTATE_FINISH = 3;

		public const int COM_HERO_WATESTATE_MAX = 4;

		public const int COM_PLATFORM_STATISTIC_TYPE_NULL = 0;

		public const int COM_PLATFORM_STATISTIC_TYPE_GODLIKE = 1;

		public const int COM_PLATFORM_STATISTIC_TYPE_DEAD = 2;

		public const int COM_PLATFORM_STATISTIC_TYPE_KILL = 3;

		public const int COM_PLATFORM_STATISTIC_TYPE_ASSIST = 4;

		public const int COM_PLATFORM_STATISTIC_TYPE_MVP = 5;

		public const int COM_PLATFORM_STATISTIC_TYPE_TRIPLE_KILL = 6;

		public const int COM_PLATFORM_STATISTIC_TYPE_QUATARY_KILL = 7;

		public const int COM_PLATFORM_STATISTIC_TYPE_PENTA_KILL = 8;

		public const int COM_PLATFORM_STATISTIC_TYPE_MAX = 9;

		public const int COM_SHOPBUY_RES_LVLCHALLENGE = 6;

		public const int COM_SHOPBUY_RES_ENTERTAINMENTRANDHERO = 13;

		public const int COM_PRIVILEGE_TYPE_NONE = 0;

		public const int COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN = 1;

		public const int COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN = 2;

		public const int COM_PRIVILEGE_TYPE_IOSVISITOR_LOGIN = 3;

		public const int COM_REFUSE_TYPE_DONOTE_AND_REC = 0;

		public const int COM_REFUSE_TYPE_ADDFRIEND = 1;

		public const int COM_REFUSE_TYPE_LBSSHARE = 2;

		public const int COM_REFUSE_TYPE_MASTERREQ = 3;

		public const int COM_REFUSE_TYPE_RESERVE_MSG = 4;

		public const int COM_RESERVE_MSG_RESULT_NULL = 0;

		public const int COM_RESERVE_MSG_RESULT_REFUSE = 1;

		public const int COM_RESERVE_MSG_RESULT_ACCEPT = 2;

		public const int COM_ITEM_FROM_NULL = 0;

		public const int COM_ITEM_FROM_BUY = 1;

		public const int COM_ITEM_FROM_WEAL = 2;

		public const int COM_ITEM_FROM_LUCKYDRAW = 3;

		public const int COM_ITEM_FROM_EXCHANGE = 4;

		public const int COM_ITEM_FROM_EXPCARD = 5;

		public const int COM_ITEM_FROM_GIFT = 6;

		public const int COM_ITEM_FROM_LADDER = 7;

		public const int COM_MATCHTEAMEERR_LIMIT_PRIVILEGE = 1;

		public const int COM_MATCHTEAMEERR_INVALIDPARAM = 2;

		public const int COM_MATCHTEAMEERR_NOTFOUND_FRIEND = 3;

		public const int COM_MATCHTEAMEERR_FRIEND_OFFLINE = 4;

		public const int COM_MATCHTEAMEERR_FRIEND_BUSY = 5;

		public const int COM_MATCHTEAMEERR_INVITE_DENY = 6;

		public const int COM_MATCHTEAMEERR_DESTROY = 7;

		public const int COM_MATCHTEAMEERR_BEGIN_MATCH = 8;

		public const int COM_MATCHTEAMEERR_MEMBER_FULL = 9;

		public const int COM_MATCHTEAMEERR_GAMEREADY_ERROR = 10;

		public const int COM_MATCHTEAMEERR_LEAVETEAM_FAILED = 11;

		public const int COM_MATCHTEAMEERR_LIMIT_RANK_FUNC = 12;

		public const int COM_MATCHTEAMEERR_BANTIME = 13;

		public const int COM_MATCHTEAMEERR_HIGHVERSION = 14;

		public const int COM_MATCHTEAMEERR_LOWVERSION = 15;

		public const int COM_MATCHTEAMEERR_LIMIT_ENTERTAINMENT_FUNC = 16;

		public const int COM_MATCHTEAMEERR_BEPUNISHED = 17;

		public const int COM_MATCHTEAMEERR_RANK_INVALID_GRADEDIFF = 18;

		public const int COM_MATCHTEAMEERR_RANK_INVALID_GUILDMEM = 19;

		public const int COM_MATCHTEAMEERR_TEAMERLEAVE = 20;

		public const int COM_MATCHTEAMEERR_INVALID_RANKSTATE_OF_REWARD = 21;

		public const int COM_MATCHTEAMEERR_INVALID_TIME = 22;

		public const int COM_MATCHTEAMEERR_INVALID_CREDIT_VALUE = 23;

		public const int COM_MATCHTEAMEERR_OTHERS = 24;

		public const int COM_MATCHTEAMEERR_DUPJOIN = 25;

		public const int COM_MATCHTEAMEERR_PLAT_CHANNEL_CLOSE = 26;

		public const int COM_MATCHTEAMEERR_OBING = 27;

		public const int COM_MATCHTEAMEERR_FORBID_LADDER = 28;

		public const int COM_MATCHTEAMEERR_BANLADDERTIME = 29;

		public const int COM_MATCHTEAMEERR_BANFIVEVSFIVETIME = 30;

		public const int COM_MATCHTEAMEERR_BANTHREEVSTHREETIME = 31;

		public const int COM_MATCHTEAMEERR_BANONEVSONETIME = 32;

		public const int COM_MATCHTEAMEERR_BANENTERMAINTTIME = 33;

		public const int COM_MATCHTEAMEERR_ANTI_DISTURB = 34;

		public const int COM_COIN_GET_PATH_PVP_BATTLE = 0;

		public const int COM_COIN_GET_PATH_TASK_REWARD = 1;

		public const int COM_COIN_GET_PATH_FRIEND = 2;

		public const int COM_COIN_GET_PATH_MAX = 3;

		public const int COM_NORMAL_MMR_GAME_TYPE_NULL = 0;

		public const int COM_NORMAL_MMR_GAME_TYPE_1V1 = 1;

		public const int COM_NORMAL_MMR_GAME_TYPE_3V3 = 2;

		public const int COM_NORMAL_MMR_GAME_TYPE_5V5 = 3;

		public const int COM_NORMAL_MMR_GAME_TYPE_CHAOS = 4;

		public const int COM_NORMAL_MMR_GAME_TYPE_ENTERTAINMENT = 5;

		public const int COM_NORMAL_MMR_GAME_TYPE_GUILDMATCH = 6;

		public const int COM_NORMAL_MMR_GAME_TYPE_MAX = 7;

		public const int COM_CAST_TYPE_CAMP_UNLIMITED = 0;

		public const int COM_CAST_TYPE_CAMP_LIMITED = 1;

		public const int COM_ACNT_SWAP_STATUS_NULL = 0;

		public const int COM_ACNT_SWAP_STATUS_ACTIVE = 1;

		public const int COM_ACNT_SWAP_STATUS_PASSIVE = 2;

		public const int COM_OBSERVE_SUCC = 0;

		public const int COM_OBSERVE_SYSBUSY = 1;

		public const int COM_OBSERVE_MATCHEND = 2;

		public const int COM_OBSERVE_VERSION_LOW = 3;

		public const int COM_OBSERVE_VERSION_HIGH = 4;

		public const int COM_CHGROOMPOS_EMPTY = 1;

		public const int COM_CHGROOMPOS_PLAYER = 2;

		public const int COM_CHGROOMPOS_BEGIN = 1;

		public const int COM_CHGROOMPOS_BUSY = 2;

		public const int COM_CHGROOMPOS_NPC = 3;

		public const int COM_CHGROOMPOS_TIMEOUT = 4;

		public const int COM_CHGROOMPOS_CANCEL = 5;

		public const int COM_CHGROOMPOS_REFUSE = 6;

		public const int COM_PUNISH_NOTCONFIRM = 1;

		public const int COM_PUNISH_HANGUP = 2;

		public const int COM_PUNISH_CREDIT = 3;

		public const int COM_BATTLE_RESULT_MIX = 0;

		public const int COM_BATTLE_RESULT_WIN = 1;

		public const int COM_BATTLE_RESULT_LOSE = 2;

		public const int COM_ACNTREGISTER_FROM_LOGIN = 1;

		public const int COM_ACNTREGISTER_FROM_TRANSDATA = 2;

		public const int COM_ACNT_PASSWDSTATE_CLOSE = 0;

		public const int COM_ACNT_PASSWDSTATE_OPEN = 1;

		public const int COM_OBNUM_TIPS_SELF = 1;

		public const int COM_OBNUM_TIPS_TEAMMATE = 2;

		public const int COM_OBNUM_TIPS_OPPONENT = 4;

		public const int COM_GUILD_EVENT_TYPE_JOIN = 1;

		public const int COM_GUILD_EVENT_TYPE_QUIT = 2;

		public const int COM_GUILD_EVENT_TYPE_BEKICK = 3;

		public const int COM_GUILD_EVENT_TYPE_PINGJI_UP = 4;

		public const int COM_GUILD_EVENT_TYPE_MEMBERNUM_UP = 5;

		public const int COM_GUILD_EVENT_TYPE_NEWMATCH_START = 6;

		public const int COM_GUILD_EVENT_TYPE_MEMBERUPTO_WANGZHE = 7;

		public const int COM_ACNT_OLD_TYPE_TBD = 0;

		public const int COM_ACNT_OLD_TYPE_LEVEL_LIMIT = 1;

		public const int COM_ACNT_OLD_TYPE_TO_BE_SELECTED = 2;

		public const int COM_ACNT_OLD_TYPE_SELECT_YES = 3;

		public const int COM_ACNT_OLD_TYPE_SELECT_NO = 4;

		public const int COM_ACNT_OLD_TYPE_OLD_VERSION = 5;

		public const int COM_ACNT_MOBA_LEVEL_TYPE_NULL = 0;

		public const int COM_ACNT_MOBA_LEVEL_TYPE_MAX = 4;

		public const int COM_ACNT_MOBA_HERO_TYPE_NULL = -1;

		public const int COM_ACNT_MOBA_HERO_TYPE_MAX = 7;

		public const int COM_CHECKBATTLE_SETTLE = 1;

		public const int COM_CHECKBATTLE_COMPLAINT = 2;

		public const int COM_CHGCREDIT_NONE = 0;

		public const int COM_CHGCREDIT_COMPLAINT_GUAJI = 1;

		public const int COM_CHGCREDIT_COMPLAINT_SONG = 2;

		public const int COM_CHGCREDIT_COMPLAINT_XIAOJI = 3;

		public const int COM_CHGCREDIT_COMPLAINT_MAREN = 4;

		public const int COM_CHGCREDIT_COMPLAINT_YANYUAN = 5;

		public const int COM_CHGCREDIT_COMPLAINT_GUA = 6;

		public const int COM_CHGCREDIT_HANGUP = 7;

		public const int COM_CHGCREDIT_NOTLOGIN = 8;

		public const int COM_CHGCREDIT_UNCONFIRM = 9;

		public const int COM_CHGCREDIT_SETTLE_DEL = 10;

		public const int COM_CHGCREDIT_SETTLE_ADD = 11;

		public const int COM_CHGCREDIT_OUTGAME = 12;

		public const int COM_CHGCREDIT_MAX = 13;

		public const int COM_CLIENT_PLAY_NULL = 0;

		public const int COM_CLIENT_PLAY_ADVENTURE = 2;

		public const int COM_CLIENT_PLAY_BURNING = 4;

		public const int COM_CLIENT_PLAY_ARENA = 8;

		public const int COM_CLIENT_PLAY_LADDER = 16;

		public const int COM_CLIENT_PLAY_REWARDMATCH = 32;

		public const int COM_CLIENT_PLAY_MATCH = 64;

		public const int COM_CLIENT_PLAY_ENTERTAINMENT = 128;

		public const int COM_CLIENT_PLAY_PKAI = 256;

		public const int COM_CLIENT_PLAY_ROOM = 512;

		public const int COM_EXTERN_ASSIST_REQPKG = 1;

		public const int COM_EXTERN_ASSIST_RSPPKG = 2;

		public const int COM_EXTERN_ASSIST_STATE_OFFLINE = 0;

		public const int COM_EXTERN_ASSIST_STATE_LOGIN = 1;

		public const int COM_EXTERN_ASSIST_STATE_ONLINE = 2;

		public const int COM_RECENT_USED_HERO_NULL = 0;

		public const int COM_RECENT_USED_HERO_HIDE = 1;

		public const int COM_RECENT_USED_HERO_SELFSET = 2;

		public const int COM_COIN_LIMIT_NULL = 0;

		public const int COM_COIN_LIMIT_CREDIT_ADD = 1;

		public const int COM_COIN_LIMIT_CREDIT_REDUCE = 2;

		public const int COM_COIN_LIMIT_ACTIVE_ADD = 4;

		public const int COM_COIN_LIMIT_LADDER_ADD = 8;

		public const int COM_PROFIT_LIMIT_NULL = 0;

		public const int COM_PROFIT_LIMIT_EXP = 1;

		public const int COM_PROFIT_LIMIT_COIN = 2;

		public const int COM_PROFIT_LIMIT_MAX = 3;

		public const int COM_SETTLE_TIPS_NULL = 0;

		public const int COM_SETTLE_TIPS_LADDER_MAXCOIN = 1;

		public const int COM_STUDENT_NULL = 0;

		public const int COM_STUDENT_PROCESS = 1;

		public const int COM_STUDENT_GRADUATE = 2;

		public const int COM_INTIMACY_STATE_NULL = 0;

		public const int COM_INTIMACY_STATE_GAY = 1;

		public const int COM_INTIMACY_STATE_LOVER = 2;

		public const int COM_INTIMACY_STATE_SIDEKICK = 3;

		public const int COM_INTIMACY_STATE_BESTIE = 4;

		public const int COM_INTIMACY_STATE_MAX = 5;

		public const int COM_INTIMACY_STATE_GAY_CONFIRM = 20;

		public const int COM_INTIMACY_STATE_GAY_DENY = 21;

		public const int COM_INTIMACY_STATE_LOVER_CONFIRM = 22;

		public const int COM_INTIMACY_STATE_LOVER_DENY = 23;

		public const int COM_INTIMACY_STATE_SIDEKICK_CONFIRM = 24;

		public const int COM_INTIMACY_STATE_SIDEKICK_DENY = 25;

		public const int COM_INTIMACY_STATE_BESTIE_CONFIRM = 26;

		public const int COM_INTIMACY_STATE_BESTIE_DENY = 27;

		public const int COM_INTIMACY_STATE_VALUE_FULL = 28;

		public const int COM_INTIMACY_RELATION_NULL = 0;

		public const int COM_INTIMACY_RELATION_ADD = 1;

		public const int COM_INTIMACY_RELATION_DEL = 2;

		public const int COM_MOBA_CHK_USED_TYPE_NOUSE = 0;

		public const int COM_MOBA_CHK_USED_TYPE_NORMAL = 1;

		public const int COM_MOBA_CHK_USED_TYPE_OLD_PLAYER = 2;

		public const int COM_MASTERCOMFIRM_AGREE = 1;

		public const int COM_MASTERCOMFIRM_DENY = 2;

		public const int COM_SNS_NULL = 0;

		public const int COM_SNS_FRIEND = 1;

		public const int COM_SNS_MASTER = 2;

		public const int COM_SNS_STUDENT = 3;

		public const int COM_GRADUATE_MINE = 1;

		public const int COM_GRADUATE_STUDENT = 2;

		public const int COM_EQUIPEVAL_GOODTYPE = 0;

		public const int COM_EQUIPEVAL_BADTYPE = 1;

		public const int COM_OPERATOR_ADD = 0;

		public const int COM_OPERATOR_MULTIPLY = 1;

		public const int COM_ACNTSTATE_CHG_NULL = 0;

		public const int COM_ACNTSTATE_CHG_INIDLE = 1;

		public const int COM_ACNTSTATE_CHG_INGAME = 2;

		public const int COM_ACNTSTATE_CHG_MAX = 3;

		public const int COM_FRIENDADD_RES_DENY = 0;

		public const int COM_FRIENDADD_RES_CONFIRM = 1;

		public const int COM_USER_PRIVACY_MASK_FIGHT_HISTORY = 0;

		public const int COM_USER_PRIVACY_MASK_VIDEO = 1;

		public const int COM_USER_PRIVACY_MASK_VIP_LEVEL = 2;

		public const int COM_USER_PRIVACY_MASK_FRIEND_CARD = 3;

		public const int COM_USER_PRIVACY_MASK_MAX = 4;

		public const int COM_ADD_ACNT_TO_ROOM_CAMP1 = 0;

		public const int COM_ADD_ACNT_TO_ROOM_CAMP2 = 1;

		public const int COM_ADD_ACNT_TO_ROOM_MID = 2;

		public const int COM_ADD_ACNT_TO_ROOM_SEQ = 3;

		public const int COM_WORLDRANK_ERRCODE_LOGICWORLDID = 1;

		public const int COM_WORLDRANK_ERRCODE_NOTPREPARED = 2;

		public const int COM_WORLDRANK_ERRCODE_RANKTYPE = 3;

		public const int COM_WORLDRANK_ERRCODE_OTHER = 4;

		public const int COM_MULTILADDER_FLAG_FORBID_3 = 0;

		public const int COM_MULTILADDER_FLAG_PERMIT_3 = 1;

		public const int COM_MULTILADDER_FLAG_PERMIT_4 = 2;

		public const int COM_MATCH_1P = 1;

		public const int COM_MATCH_2P = 2;

		public const int COM_MATCH_3P = 3;

		public const int COM_MATCH_4P = 4;

		public const int COM_MATCH_5P = 5;

		public const int COM_OTHERSTATE_BIT_MASK_DND = 0;

		public const int COM_OTHERSTATE_BIT_MASK_TVOB = 1;

		public const int COM_OTHERSTATE_BIT_MASK_MAX = 2;

		public const int COM_ASKFOR_MSG_ID = 0;

		public const int COM_ASKFOR_MSG_STR = 1;

		public const int COM_ASKFOR_OPENSWTICH_FRIEND = 1;

		public const int COM_ASKFOR_OPENSWTICH_SNS = 2;

		public const int COM_ASKFOR_OPENSWTICH_MASTER = 4;

		public const int COM_ASKFOR_DELOPT_DELETE = 0;

		public const int COM_ASKFOR_DELOPT_REFUSE = 1;

		public const int COM_ASKFOR_DELOPT_CONFIRM = 2;

		public const int COM_BURNING_ENEMY_TYPE_REAL_MAN = 1;

		public const int COM_BURNING_ENEMY_TYPE_ROBOT = 2;

		public const int COM_LEVEL_STATUS_LOCKED = 0;

		public const int COM_LEVEL_STATUS_UNLOCKED = 1;

		public const int COM_LEVEL_STATUS_FINISHED = 2;

		public const int COM_PLAYER_GUILD_STATE_NULL = 0;

		public const int COM_PLAYER_GUILD_STATE_PREPARE_CREATE = 1;

		public const int COM_PLAYER_GUILD_STATE_PREPARE_JOIN = 2;

		public const int COM_PLAYER_GUILD_STATE_CHAIRMAN = 3;

		public const int COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN = 4;

		public const int COM_PLAYER_GUILD_STATE_ELDER = 5;

		public const int COM_PLAYER_GUILD_STATE_MEMBER = 6;

		public const int COM_PLAYER_GUILD_STATE_WAIT_RESULT = 7;

		public const int COM_GUILD_SUCCESS = 0;

		public const int COM_GUILD_ERR_NAME_DUP = 1;

		public const int COM_GUILD_ERR_DB = 2;

		public const int COM_GUILD_ERR_CREATE_QUEUE_FULL = 3;

		public const int COM_GUILD_ERR_NO_PREPARE_GUILD = 4;

		public const int COM_GUILD_ERR_MEMBER_FULL = 5;

		public const int COM_GUILD_ERR_HAS_INVITED = 6;

		public const int COM_GUILD_ERR_HAS_RECOMMEND = 7;

		public const int COM_GUILD_ERR_HAS_GUILD = 8;

		public const int COM_GUILD_ERR_GUILD_NOT_EXIST = 9;

		public const int COM_GUILD_ERR_LEVEL_LIMIT = 10;

		public const int COM_GUILD_ERR_INVITED_EXPIRE = 11;

		public const int COM_GUILD_ERR_UPGRADE = 12;

		public const int COM_GUILD_ERR_FIRE_CNT_LIMIT = 13;

		public const int COM_GUILD_ERR_DEAL_SELF_RECOM_FAIL = 14;

		public const int COM_GUILD_ERR_DONATE_CNT_LIMIT = 15;

		public const int COM_GUILD_ERR_DUPLICATE_GET_DIVIDEND = 16;

		public const int COM_GUILD_ERR_NAME_ILLEGAL = 17;

		public const int COM_GUILD_ERR_NOTICE_ILLEGAL = 18;

		public const int COM_GUILD_ERR_FACTORY_LEVEL_LIMIT = 19;

		public const int COM_GUILD_ERR_CONSTRUCT_NOT_ENOUGH = 20;

		public const int COM_GUILD_ERR_HIS_CONSTRUCT_NOT_ENOUGH = 21;

		public const int COM_GUILD_ERR_UPGRADE_GUILD_FAIL = 22;

		public const int COM_GUILD_ERR_HAS_SIGNIN = 23;

		public const int COM_GUILD_ERR_NOT_SAME_LOGICWORLD = 24;

		public const int COM_GUILD_ERR_MATCH_OTHER = 25;

		public const int COM_GUILD_ERR_MATCH_MEMBER_CNT = 26;

		public const int COM_GUILD_ERR_MATCH_MEMBER_NOT_READY = 27;

		public const int COM_GUILD_ERR_MATCH_MEMBER_VERSION = 28;

		public const int COM_GUILD_ERR_MATCH_CNT_LIMIT = 29;

		public const int COM_GUILD_ERR_MATCH_REJECT_INVITE = 30;

		public const int COM_GUILD_ERR_MATCH_TEAM_FULL = 31;

		public const int COM_GUILD_ERR_GRADE_LIMIT = 32;

		public const int COM_GUILD_ERR_GUILD_MATCHING = 33;

		public const int COM_GUILD_ERR_JOIN_TIME_LIMIT = 34;

		public const int COM_GUILD_ERR_TEAM_LEADER_FULL = 35;

		public const int COM_GUILD_ERR_SEND_GUILD_RECRUIT_CD = 36;

		public const int COM_GUILD_ERR_APPLY_EXIST = 37;

		public const int COM_GUILD_ERR_NO_LEADDER = 38;

		public const int COM_GUILD_ERR_NOT_INGUILD = 39;

		public const int COM_GUILD_ERR_YOUARE_LEADDER = 40;

		public const int COM_GUILD_MATCH_SIGNUP_EXIST = 41;

		public const int COM_GUILD_ERR_MATCH_SELFREJECT_INVITE = 42;

		public const int COM_GUILD_ERR_MATCH_NOTINSIGNUP_TIME = 43;

		public const int COM_GUILD_ERR_SIGNUPBEIZHU_ILLEGAL = 44;

		public const int COM_GUILD_ERR_HAVEINHISTEAM = 45;

		public const int COM_GUILD_ERR_HAS_SAME_UID_MEMBER = 46;

		public const int COM_GUILD_SETTING_NEED_APPROVE = 1;

		public const int COM_GUILD_SYMBOL_OPER_TYPE_GET = 1;

		public const int COM_GUILD_SYMBOL_OPER_TYPE_UPGRADE = 2;

		public const int COM_SEARCH_GUILD_TYPE_NOMARL = 0;

		public const int COM_SEARCH_GUILD_TYPE_HYPERLINK = 1;

		public const int COM_HYPERLINK_TYPE_GUILD_INVITE = 1;

		public const int COM_HYPERLINK_TYPE_PREGUILD_INVITE = 2;

		public const int COM_HYPERLINK_TYPE_FORM_JUMP = 3;

		public const int COM_HYPERLINK_TYPE_URL = 4;

		public const int COM_APPOINT_POSITION_TRANS_TYPE_APPOINT = 0;

		public const int COM_APPOINT_POSITION_TRANS_TYPE_DEAL_SELF_RECOMMEND = 1;

		public const int COM_APPOINT_POSITION_TRANS_TYPE_SELF_RECOMMEND_TIMEOUT = 2;

		public const int COM_APPOINT_POSITION_TRANS_TYPE_IMPEACH_CHAIRMAN = 3;

		public const int COM_APPOINT_POSITION_TRANS_TYPE_IDIPCHG_CHAIRMAN = 4;

		public const int COM_APOLLO_TRANK_USERBUFFER_EXTRA_TYPE_NULL = 0;

		public const int COM_APOLLO_TRANK_USERBUFFER_EXTRA_TYPE_SEARCH_GUILD = 1;

		public const int COM_APOLLO_TRANK_USERBUFFER_EXTRA_TYPE_TOPN = 2;

		public const int COM_APOLLO_TVOIP_USERBUFFER_TYPE_NULL = 0;

		public const int COM_APOLLO_TVOIP_USERBUFFER_TYPE_CREATE_ROOM = 1;

		public const int COM_APOLLO_TVOIP_USERBUFFER_TYPE_JOIN_ROOM = 2;

		public const int COM_ACHIEVEMENT_STATE_UNFIN = 0;

		public const int COM_ACHIEVEMENT_STATE_FIN = 1;

		public const int COM_ACHIEVEMENT_STATE_REWARD = 2;

		public const int COM_TROPHYLVL_STATE_UNFIN = 0;

		public const int COM_TROPHYLVL_STATE_FIN = 1;

		public const int COM_TROPHYLVL_STATE_REWARD = 2;

		public const int COM_REWARDMATCH_STATE_END = 0;

		public const int COM_REWARDMATCH_STATE_START = 1;

		public const int COM_REWARDMATCH_STATE_REWARD = 2;

		public const int FIGHT_HISTORY_ACHIVE_GODLIKE = 0;

		public const int FIGHT_HISTORY_ACHIVE_PENTAKILL = 1;

		public const int FIGHT_HISTORY_ACHIVE_QUATARYKILL = 2;

		public const int FIGHT_HISTORY_ACHIVE_TRIPLEKILL = 3;

		public const int FIGHT_HISTORY_ACHIVE_DOUBLEKILL = 4;

		public const int FIGHT_HISTORY_ACHIVE_KILLMOST = 5;

		public const int FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST = 6;

		public const int FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST = 7;

		public const int FIGHT_HISTORY_ACHIVE_ASSISTMOST = 8;

		public const int FIGHT_HISTORY_ACHIVE_GETMOENYMOST = 9;

		public const int FIGHT_HISTORY_ACHIVE_KILLORGANMOST = 10;

		public const int FIGHT_HISTORY_ACHIVE_RUNAWAY = 11;

		public const int FIGHT_HISTORY_ACHIVE_FIGHTACHIVECOUNT = 12;

		public const int FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND = 13;

		public const int FIGHT_HISTORY_ACHIVE_WINMVP = 14;

		public const int FIGHT_HISTORY_ACHIVE_LOSEMVP = 15;

		public const int FIGHT_HISTORY_ACHIVE_ISCOMPUTER = 16;

		public const int FIGHT_HISTORY_ACHIVE_ISWARMBATTLE = 17;

		public const int FIGHT_HISTORY_ACHIVE_LADDER5V5 = 18;

		public const int FIGHT_HISTORY_ACHIVE_LADDER4 = 19;

		public const int FIGHT_HISTORY_ACHIVE_LADDER3 = 20;

		public const int FIGHT_HISTORY_ACHIVE_LADDER2 = 21;

		public const int FIGHT_HISTORY_ACHIVE_COUNT = 22;

		public const int CS_MAX_PKGDATA_LEN = 256000;

		public const int CS_MAX_RESDATA_LEN = 51200;

		public const int CS_CCSYNC_COMMON_BUFF_LEN = 64;

		public const int CS_ACNTUPD_CNT = 3;

		public const int CS_HEROUPD_CNT = 10;

		public const int CS_MAXSWEEP_CNT = 10;

		public const int CS_HEROUPD_PARAM_CNT = 3;

		public const int CS_RECOVER_FRAP_BUF_LEN = 32768;

		public const int CS_PVP_GAMEDATA_REPORT_LEN = 32768;

		public const int CS_PVP_LOGFILE_NAME_LEN = 128;

		public const int CS_MAX_TIPS_LEN = 128;

		public const int CS_ANTIDATA_MAXLEN = 65535;

		public const int CS_MAX_RANKLING_LIST_NUM = 100;

		public const int CS_MAX_SNS_LIST_NUM = 100;

		public const int CS_MAX_KFRAPBOOTPKG_LEN = 466;

		public const int CS_MAX_SPAREFRAP_NUM = 4;

		public const int CS_MAX_REQUESTSINGLE_NUM = 40;

		public const int CS_MAX_IMEI_LEN = 64;

		public const int CS_MAX_SHARE_ITEM_CNT = 64;

		public const int CS_MAX_SECRETARY_CNT = 64;

		public const int CS_MAX_HORNMSG_CNT = 600;

		public const int CS_MAX_HORNMSG_ONEWORLD_CNT = 50;

		public const int CS_MAX_ONE_CAMP_BAN_NUM = 3;

		public const int CS_MAX_WEAL_CONTENTSHARE_PARAM_NUM = 4;

		public const int CS_MAX_GAMINGUPER_SPARE_NUM = 3;

		public const int CS_MAX_CHAT_MSG_NTF_CNT = 100;

		public const int CS_MAX_SHOPBUY_TIMES = 10;

		public const int CS_MAX_FREC_NUM = 10;

		public const int CS_MAX_SYMBOLCOMP_PARTCNT = 4;

		public const int CS_MAX_OPENKEY_LEN = 512;

		public const int CS_MAX_FUNCTION_SWITCH_ARRAY_CNT = 5;

		public const int CS_MAX_SELFMSG_NUM = 10;

		public const int CS_MAX_TVOIP_ACCESS_URL_COUNT = 16;

		public const int CS_MAX_TVOIP_ROOM_USER_COUNT = 16;

		public const int CS_MAX_CLT_PERFORMANCE_STR_LEN = 128;

		public const int CS_STUDENTLIST_MINE = 1;

		public const int CS_STUDENTLIST_BROTHER = 2;

		public const int CS_ACNTINFO_UPD_NULL = 0;

		public const int CS_ACNTINFO_UPD_LEVEL = 1;

		public const int CS_ACNTINFO_UPD_EXP = 2;

		public const int CS_ACNTINFO_UPD_MAXAP = 3;

		public const int CS_ACNTINFO_UPD_CURAP = 4;

		public const int CS_ACNTINFO_UPD_COIN = 5;

		public const int CS_ACNTINFO_UPD_DIAMOND = 6;

		public const int CS_ACNTINFO_UPD_LEVEL_INFO = 7;

		public const int CS_ACNTINFO_UPD_SKILLPOINT = 8;

		public const int CS_ACNTINFO_UPD_ENCHANTPT = 9;

		public const int CS_ACNTINFO_UPD_PVP_LEVEL = 10;

		public const int CS_ACNTINFO_UPD_PVP_EXP = 11;

		public const int CS_ACNTINFO_UPD_PVP_COIN = 12;

		public const int CS_ACNTINFO_UPD_BURNING_COIN = 13;

		public const int CS_ACNTINFO_UPD_ARENA_COIN = 14;

		public const int CS_ACNTINFO_UPD_HEROPOOLEXP = 15;

		public const int CS_ACNTINFO_UPD_SKIN_COIN = 16;

		public const int CS_ACNTINFO_UPD_SYMBOL_COIN = 17;

		public const int CS_ACNTINFO_UPD_HUOYUEDU = 18;

		public const int CS_ACNTINFO_UPD_TEAMMATELIKE = 19;

		public const int CS_ACNTINFO_UPD_OPPONENTLIKE = 20;

		public const int CS_ACNTINFO_UPD_SCORE = 21;

		public const int CS_FRAPBOOT_ACNTSTATE_NULL = 0;

		public const int CS_FRAPBOOT_ACNTSTATE_DISCONN = 1;

		public const int CS_FRAPBOOT_ACNTSTATE_RECONN = 2;

		public const int CS_FRAPBOOT_ACNTSTATE_RUNAWAY = 3;

		public const int CS_CHEATHUOYUEDU_OPT_ADD = 1;

		public const int CS_CHEATHUOYUEDU_OPT_DEC = 2;

		public const int CS_CHEATHUOYUEDU_OPT_RESET = 3;

		public const int CS_CHEATSUB_ADDCOIN = 1;

		public const int CS_CHEATSUB_BUYCOUPONS = 2;

		public const int CS_CHEATSUB_SETACNTLVL = 3;

		public const int CS_CHEATSUB_ADDACNTEXP = 4;

		public const int CS_CHEATSUB_ADDACNTMAXAP = 5;

		public const int CS_CHEATSUB_ADDACNTCURAP = 6;

		public const int CS_CHEATSUB_ADDITEM = 7;

		public const int CS_CHEATSUB_ADDHERO = 8;

		public const int CS_CHEATSUB_TASKDONE = 9;

		public const int CS_CHEATSUB_SETHEROLVL = 10;

		public const int CS_CHEATSUB_ADDHEROEXP = 11;

		public const int CS_CHEATSUB_UNLOCK_ALL_LEVEL = 12;

		public const int CS_CHEATSUB_SENDMAIL = 13;

		public const int CS_CHEATSUB_CLRSHOPBUYLIMIT = 14;

		public const int CS_CHEATSUB_UPDACNTINFO = 15;

		public const int CS_CHEATSUB_RANDOMREWARD = 16;

		public const int CS_CHEATSUB_SET_MMROFRANK = 17;

		public const int CS_CHEATSUB_SET_GRADEOFRANK = 18;

		public const int CS_CHEATSUB_SET_SCOREOFRANK = 19;

		public const int CS_CHEATSUB_SET_CONWINCNT = 20;

		public const int CS_CHEATSUB_SET_CONLOSECNT = 21;

		public const int CS_CHEATSUB_SET_MMROFNORMAL = 22;

		public const int CS_CHEATSUB_CLRSHOPREFRESH = 23;

		public const int CS_CHEATSUB_SETHEROSTAR = 24;

		public const int CS_CHEATSUB_SETHEROQUALITY = 25;

		public const int CS_CHEATSUB_UNLOCK_ACTIVITY = 26;

		public const int CS_CHEATSUB_CLR_ELITE_LEVEL = 27;

		public const int CS_CHEATSUB_DYE_NEWBIE_BIT = 28;

		public const int CS_CHEATSUB_PASS_SINGLE_GAME = 29;

		public const int CS_CHEATSUB_PASS_MULTI_GAME = 30;

		public const int CS_CHEATSUB_SET_OFFSET_SEC = 31;

		public const int CS_CHEATSUB_SET_FREE_HERO = 32;

		public const int CS_CHEATSUB_UNLOCK_HEROPVPMASK = 33;

		public const int CS_CHEATSUB_APREFRESH_SHOP = 34;

		public const int CS_CHEATSUB_CLR_BURNING_LIMIT = 35;

		public const int CS_CHEATSUB_ADD_PVP_COIN = 36;

		public const int CS_CHEATSUB_ADD_PVP_EXP = 37;

		public const int CS_CHEATSUB_SET_PVP_LVL = 38;

		public const int CS_CHEATSUB_REFRESH_POWER = 39;

		public const int CS_CHEATSUB_SET_GUILD_INFO = 40;

		public const int CS_CHEATSUB_ADD_BURNING_COIN = 41;

		public const int CS_CHEATSUB_ADD_ARENA_COIN = 42;

		public const int CS_CHEATSUB_SET_SKILLLVL_MAX = 43;

		public const int CS_CHEATSUB_ADD_SKINCOIN = 44;

		public const int CS_CHEATSUB_GEARADVALL = 45;

		public const int CS_CHEATSUB_GIVECOUPONS = 46;

		public const int CS_CHEATSUB_CUTPACKAGECNT = 47;

		public const int CS_CHEATSUB_ADD_SYMBOLCOIN = 48;

		public const int CS_CHEATSUB_ADD_HERO_PROFICI = 49;

		public const int CS_CHEATSUB_ADD_DIAMOND = 50;

		public const int CS_CHEATSUB_HEROWAKE = 51;

		public const int CS_CHEATSUB_HEROSLEEP = 52;

		public const int CS_CHEATSUB_SETRANKTOTALFIGHT = 53;

		public const int CS_CHEATSUB_SETRANKTOTALWIN = 54;

		public const int CS_CHEATSUB_SET_MAX_FRIEND_NUM = 55;

		public const int CS_CHEATSUB_ADD_ALLSKIN = 56;

		public const int CS_CHEATSUB_HUOYUEDU = 57;

		public const int CS_CHEATSUB_WARMBATTLE_TODAYCNT = 58;

		public const int CS_CHEATSUB_WARMBATTLE_BATTLECNT = 59;

		public const int CS_CHEATSUB_WARMBATTLE_CONLOSECNT = 60;

		public const int CS_CHEATSUB_WARMBATTLE_KILLNUM = 61;

		public const int CS_CHEATSUB_WARMBATTLE_DEADNUM = 62;

		public const int CS_CHEATSUB_SET_GUILD_MEMBER_INFO = 63;

		public const int CS_CHEATSUB_SET_HERO_CUSTOM_EQUIP = 64;

		public const int CS_CHEATSUB_HEADIMAGE_ADD = 65;

		public const int CS_CHEATSUB_HEADIMAGE_DEL = 66;

		public const int CS_CHG_CREDIT_VALUE = 67;

		public const int CS_CHG_REWARDMATCH_POINT = 68;

		public const int CS_CHG_REWARDMATCH_POOL = 69;

		public const int CS_CHEATSUB_LEVELREWARD = 70;

		public const int CS_CHEATSUB_CHG_NEW_NORMALMMR = 71;

		public const int CS_CHEATSUB_CHG_HONORINFO = 72;

		public const int CS_CHG_REWARDMATCH_INFO = 73;

		public const int CS_CREATE_GUILD = 74;

		public const int CS_CHEATSUB_SET_MASTERHERO = 75;

		public const int CS_CHEATSUB_ADD_FIGHTHISTORY = 76;

		public const int CS_CHEATSUB_ADD_GAME_STATISTIC = 77;

		public const int CS_CHEATSUB_AUTOJOIN_TEAM = 78;

		public const int CS_CHEATSUB_SET_MOBA_INFO = 79;

		public const int CS_CHEATSUB_GET_MOBA_INFO = 80;

		public const int CS_CHEATSUB_CLEAR_CHGNAME_CD = 81;

		public const int CS_CHEATSUB_CLEAR_PROFITLIMIT = 82;

		public const int CS_CHEATSUB_CHG_INTIMACY = 83;

		public const int CS_CHEATSUB_RESET_ACHIEVE = 84;

		public const int CS_CHEATSUB_DONE_ACHIEVE = 85;

		public const int CS_CHEATSUB_DELCREDIT = 86;

		public const int CS_CHEATADD_MASTERPOINT = 87;

		public const int CS_CHEATSUB_INTIMACYRELATION = 88;

		public const int CS_CHEATSUB_RECRUITMENTRELATION = 89;

		public const int CS_CHEATSUB_SET_ADDSTARSCORE = 90;

		public const int CS_CHEATSUB_SET_PVPBANENDTIME = 91;

		public const int CS_CHEATSUB_CLEAR_RECRUITMENTBIT = 92;

		public const int CS_ITEMOPT_GM = 1;

		public const int CS_ITEMOPT_BUY = 2;

		public const int CS_ITEMOPT_SALE = 3;

		public const int CS_ITEMOPT_USE = 4;

		public const int CS_ITEMOPT_COMP = 5;

		public const int CS_ITEMOPT_TASK = 6;

		public const int CS_ITEMOPT_WEAR = 7;

		public const int CS_ITEMOPT_UPGRADESTAR = 8;

		public const int CS_ITEMOPT_REWARD = 9;

		public const int CS_ITEMOPT_EQUIPSMELT = 10;

		public const int CS_ITEMOPT_MAILGETACCESS = 11;

		public const int CS_ITEMOPT_HEROADV = 12;

		public const int CS_ITEMOPT_GEARADV = 13;

		public const int CS_ITEMOPT_GUILDSYMBOL = 14;

		public const int CS_ITEMOPT_MAKE = 15;

		public const int CS_ITEMOPT_BREAK = 16;

		public const int CS_ITEMOPT_HEROWAKE = 17;

		public const int CS_ITEMOPT_ACHIEVEMENT = 18;

		public const int CS_ITEMOPT_CHGNAME = 19;

		public const int CS_ITEMOPT_EXCHANGE = 20;

		public const int CS_ITEMOPT_HUOYUEDUREWARD = 21;

		public const int CS_ITEMOPT_WEAL = 22;

		public const int CS_ITEMOPT_IDIP = 23;

		public const int CS_ITEMOPT_LEVELREWARD = 24;

		public const int CS_ITEMOPT_RECRUITMENTREWARD = 25;

		public const int CS_HEROINFO_UPD_LEVEL = 1;

		public const int CS_HEROINFO_UPD_EXP = 2;

		public const int CS_HEROINFO_UPD_STAR = 3;

		public const int CS_HEROINFO_UPD_QUALITY = 4;

		public const int CS_HEROINFO_UPD_SUBQUALITY = 5;

		public const int CS_HEROINFO_UPD_UNLOCKSKILLSLOT = 6;

		public const int CS_HEROINFO_UPD_PROFICIENCY = 7;

		public const int CS_HEROINFO_UPD_MASKBITS = 8;

		public const int CS_HEROINFO_UPD_LIMITTIME = 9;

		public const int CS_HEROINFO_UPD_MASTERGAMECNT = 10;

		public const int CS_COINBUY_TYPE_ONE = 0;

		public const int CS_COINBUY_TYPE_TEN = 1;

		public const int CS_COUPONS_PAYTYPE_NULL = 0;

		public const int CS_COUPONS_PAYTYPE_FIRST = 1;

		public const int CS_COUPONS_PAYTYPE_RENEW = 2;

		public const int CS_COUPONS_PAYTYPE_UPDATE = 3;

		public const int CS_COUPONS_PAYTYPE_LOGIN = 4;

		public const int CS_COUPONS_PAYTYPE_PAY = 5;

		public const int CS_COUPONS_PAYTYPE_QUERY = 6;

		public const int CS_ACNT_UPDATE_FROMTYPE_NULL = 0;

		public const int CS_ACNT_UPDATE_FROMTYPE_PROP = 1;

		public const int CS_ACNT_UPDATE_FROMTYPE_SWEEP = 2;

		public const int CS_ACNT_UPDATE_FROMTYPE_TASK = 3;

		public const int CS_ACNT_UPDATE_FROMTYPE_MAIL = 4;

		public const int CS_ACNT_UPDATE_FROMTYPE_HUOYUEDU = 5;

		public const int CS_ACNT_UPDATE_FROMTYPE_IDIP = 6;

		public const int QQ_VIP_FLAG_NULL = 0;

		public const int QQ_VIP_FLAG_NORMAL = 1;

		public const int QQ_VIP_FLAG_BLUE = 4;

		public const int QQ_VIP_FLAG_RED = 8;

		public const int QQ_VIP_FLAG_SUPER_VIP = 16;

		public const int QQ_VIP_FLAG_GAME_VIP = 32;

		public const int QQ_VIP_FLAG_GMAER = 64;

		public const int CS_RES_DATA_NULL = 0;

		public const int CS_RES_DATA_WEAL_CHECKIN = 1;

		public const int CS_RES_DATA_WEAL_FILLINPRICE = 2;

		public const int CS_RES_DATA_WEAL_FIXEDTIME = 3;

		public const int CS_RES_DATA_WEAL_MULTIPLE = 4;

		public const int CS_RES_DATA_WEAL_CONDITION = 5;

		public const int CS_RES_DATA_WEAL_TEXT = 6;

		public const int CS_RES_DATA_RANDOM_REWARD = 7;

		public const int CS_RES_DATA_PVP_DAILY_RATIO = 8;

		public const int CS_RES_DATA_SPEC_SALE = 9;

		public const int CS_RES_DATA_HERO_PROMOTION = 10;

		public const int CS_RES_DATA_SKIN_PROMOTION = 11;

		public const int CS_RES_DATA_LUCKYDRAW_PRICE = 12;

		public const int CS_RES_DATA_LUCKYDRAW_REWARD = 13;

		public const int CS_RES_DATA_LUCKYDRAW_EXTERNREWARD = 14;

		public const int CS_RES_DATA_SALE_RECOMMEND = 17;

		public const int CS_RES_DATA_RAND_DRAW = 18;

		public const int CS_RES_DATA_REWARDPOOL = 19;

		public const int CS_RES_DATA_REDDOTTIPINFO = 20;

		public const int CS_RES_DATA_HEROSHOP = 21;

		public const int CS_RES_DATA_HEROSKINSHOP = 22;

		public const int CS_RES_DATA_WEAL_EXCHANGE = 23;

		public const int CS_RES_DATA_PVP_SPECITEM = 24;

		public const int CS_RES_DATA_REWARDMATCH_TIME = 25;

		public const int CS_RES_DATA_HUOYUEDU_REWARD = 26;

		public const int CS_RES_DATA_BOUTIQUE = 27;

		public const int CS_RES_DATA_HEADIMAGE = 28;

		public const int CS_RES_DATA_SRV2CLT_GLOBAL_CONF = 29;

		public const int CS_RES_DATA_BANNERIMAGE = 30;

		public const int CS_RES_DATA_REWARDMATCH = 31;

		public const int CS_RES_DATA_SHOPPROMOTION = 32;

		public const int CS_RES_DATA_PROP = 33;

		public const int CS_RES_DATA_WEAL_PTEXCHANGE = 34;

		public const int CS_RES_DATA_RANK_SEASON = 35;

		public const int CS_RES_DATA_RAREEXCHANGE = 36;

		public const int CS_RES_DATA_WEAL_PARAM = 37;

		public const int CS_RES_DATA_MAX = 38;

		public const int CS_SYMBOLBREAK_SINGLE = 0;

		public const int CS_SYMBOLBREAK_LIST = 1;

		public const int CS_AKALISHOP_ERROR_SHOPNOTOPEN = 1;

		public const int CS_AKALISHOP_ERROR_SHOPCLOSE = 2;

		public const int CS_AKALISHOP_ERROR_OVERTOTALLIMIT = 3;

		public const int CS_AKALISHOP_ERROR_GOODSNOTINSHOP = 4;

		public const int CS_AKALISHOP_ERROR_GOODSALREADYBUY = 5;

		public const int CS_AKALISHOP_ERROR_BUYFAIL = 6;

		public const int CS_AKALISHOP_ERROR_INVALID = 7;

		public const int CS_AKALISHOP_ERROR_COINLIMIT = 8;

		public const int CS_AKALISHOP_ERROR_STATEERR = 9;

		public const int CS_USER_SHARE_ON_GETHERO = 0;

		public const int CS_USER_SHARE_ON_GAMESETTLE = 1;

		public const int CS_USER_SHARE_SUBTYPE_XUANYAOYIXIA = 0;

		public const int CS_USER_SHARE_SUBTYPE_BAOCUNDAOSHOUJI = 1;

		public const int CS_USER_SHARE_SUBTYPE_FENXIANGHAOYOU_QQ = 2;

		public const int CS_USER_SHARE_SUBTYPE_FENXIANGHAOYOU_WX = 3;

		public const int CS_USER_SHARE_SUBTYPE_FENXIANG_QQKONGJIAN = 4;

		public const int CS_USER_SHARE_SUBTYPE_FENXIANG_WXPENGYOUQUAN = 5;

		public const int CS_HONOR_SUCCESS = 0;

		public const int CS_HONOR_NOTHAVE = 1;

		public const int CS_CHAT_COMPLAINT_SUCCESS = 0;

		public const int CS_CHAT_COMPLAINT_LIMIT = 1;

		public const int CSID_CMD_HEARTBEAT = 1000;

		public const int SCID_GAMELOGINDISPATCH = 1001;

		public const int CSID_CMD_GAMELOGINREQ = 1002;

		public const int SCID_CMD_GAMELOGINRSP = 1003;

		public const int CSID_GAMING_UPERMSG = 1004;

		public const int SCID_NTF_ACNT_REGISTER = 1007;

		public const int CSID_ACNT_REGISTER_REQ = 1008;

		public const int SCID_ACNT_REGISTER_RES = 1009;

		public const int SCID_ACNT_INFO_UPD = 1010;

		public const int SCID_NTF_ACNT_LEVELUP = 1011;

		public const int CSID_CMD_CHEATCMD = 1012;

		public const int SCID_CMD_LOGINFINISH_NTF = 1013;

		public const int SCID_CMD_RELOGINNOW = 1014;

		public const int SCID_NTF_ACNT_PVPLEVELUP = 1015;

		public const int CSID_CMD_GAMELOGOUTREQ = 1016;

		public const int SCID_CMD_GAMELOGOUTRSP = 1017;

		public const int SCID_CMD_LOGINSYN_REQ = 1018;

		public const int CSID_CMD_LOGINSYN_RSP = 1019;

		public const int CSID_CREATEULTIGAMEREQ = 1020;

		public const int SCID_JOINMULTIGAMERSP = 1022;

		public const int CSID_QUITMULTIGAMEREQ = 1023;

		public const int SCID_QUITMULTIGAMERSP = 1024;

		public const int SCID_ROOMCHGNTF = 1025;

		public const int SCID_ASK_ACNT_TRANS_VISITORSVRDATA = 1026;

		public const int CSID_RSP_ACNT_TRANS_VISITORSVRDATA = 1027;

		public const int SCID_GAMECONN_REDIRECT = 1030;

		public const int SCID_FRAPBOOT_SINGLE = 1034;

		public const int SCID_FRAPBOOTINFO = 1035;

		public const int CSID_REQUESTFRAPBOOTSINGLE = 1036;

		public const int CSID_REQUESTFRAPBOOTTIMEOUT = 1037;

		public const int SCID_OFFINGRESTART_REQ = 1040;

		public const int CSID_OFFINGRESTART_RSP = 1041;

		public const int SCID_CMD_GAMELOGINLIMIT = 1042;

		public const int SCID_CMD_BANTIME_CHG = 1043;

		public const int SCID_ISACCEPT_AIPLAYER_REQ = 1044;

		public const int CSID_ISACCEPT_AIPLAYER_RSP = 1045;

		public const int SCID_NOTICE_HANGUP = 1046;

		public const int CSID_STARTSINGLEGAMEREQ = 1050;

		public const int SCID_STARTSINGLEGAMERSP = 1051;

		public const int CSID_SINGLEGAMEFINREQ = 1052;

		public const int SCID_SINGLEGAMEFINRSP = 1053;

		public const int CSID_SINGLEGAMESWEEPREQ = 1054;

		public const int SCID_SINGLEGAMESWEEPRSP = 1055;

		public const int CSID_GET_CHAPTER_REWARD_REQ = 1056;

		public const int SCID_GET_CHAPTER_REWARD_RSP = 1057;

		public const int CSID_QUITSINGLEGAMEREQ = 1058;

		public const int SCID_QUITSINGLEGAMERSP = 1059;

		public const int CSID_ASKINMULTGAME_REQ = 1060;

		public const int SCID_ASKINMULTGAME_RSP = 1061;

		public const int CSID_SECURE_INFO_START_REQ = 1062;

		public const int SCID_MULTGAME_BEGINBAN = 1069;

		public const int SCID_MULTGAME_BEGINPICK = 1070;

		public const int SCID_MULTGAME_BEGINADJUST = 1071;

		public const int SCID_MULTGAME_BEGINLOAD = 1075;

		public const int CSID_MULTGAME_LOADFIN = 1076;

		public const int SCID_MULTGAME_BEGINFIGHT = 1077;

		public const int SCID_MULTGAMEREADYNTF = 1078;

		public const int SCID_MULTGAMEABORTNTF = 1079;

		public const int CSID_MULTGAME_GAMEOVER = 1080;

		public const int SCID_MULTGAME_GAMEOVER = 1081;

		public const int SCID_MULTGAME_SETTLEGAIN = 1082;

		public const int CSID_MULTGAME_LOADPROCESS = 1083;

		public const int SCID_MULTGAME_LOADPROCESS = 1084;

		public const int SCID_MULTGAME_NTF_CLT_GAMEOVER = 1085;

		public const int CSID_MULTGAME_RUNAWAY_REQ = 1086;

		public const int SCID_MULTGAME_RUNAWAY_RSP = 1087;

		public const int SCID_MULTGAME_RUNAWAY_NTF = 1088;

		public const int SCID_MULTGAMERECOVERNTF = 1089;

		public const int CSID_RECOVERGAMEFRAP_REQ = 1090;

		public const int SCID_RECOVERGAMEFRAP_RSP = 1091;

		public const int SCID_RECONNGAME_NTF = 1092;

		public const int CSID_RECOVERGAMESUCC = 1093;

		public const int SCID_MULTGAME_DISCONN_NTF = 1094;

		public const int SCID_MULTGAME_RECONN_NTF = 1095;

		public const int CSID_KFRAPLATERCHG_REQ = 1096;

		public const int SCID_KFRAPLATERCHG_NTF = 1097;

		public const int CSID_MULTGAME_DIE_REQ = 1098;

		public const int SCID_HANGUP_NTF = 1099;

		public const int CSID_CMD_ITEMSALE = 1101;

		public const int SCID_CMD_ITEMADD = 1102;

		public const int SCID_CMD_ITEMDEL = 1103;

		public const int CSID_CMD_EQUIPWEAR = 1104;

		public const int SCID_CMD_EQUIPCHG = 1106;

		public const int CSID_CMD_PROPUSE = 1107;

		public const int CSID_CMD_PKGQUERY = 1108;

		public const int SCID_NTF_PKGDETAIL = 1109;

		public const int CSID_CMD_ITEMCOMP = 1110;

		public const int CSID_CMD_HEROADVANCE = 1111;

		public const int SCID_CMD_HEROADVANCE = 1112;

		public const int CSID_CMD_SHOPBUY = 1113;

		public const int SCID_CMD_SHOPBUY = 1114;

		public const int CSID_CMD_COINBUY = 1115;

		public const int SCID_CMD_COINBUY = 1116;

		public const int SCID_NTF_CLRSHOPBUYLIMIT = 1117;

		public const int CSID_CMD_AUTOREFRESH = 1118;

		public const int CSID_CMD_MANUALREFRESH = 1119;

		public const int SCID_CMD_SHOPDETAIL = 1120;

		public const int CSID_CMD_SYMBOLNAMECHG = 1121;

		public const int SCID_CMD_SYMBOLNAMECHG = 1122;

		public const int CSID_CMD_HORNUSE = 1123;

		public const int SCID_CMD_HORNUSE = 1124;

		public const int CSID_CMD_ITEMBUY = 1125;

		public const int SCID_CMD_ITEMBUY = 1126;

		public const int SCID_NTF_SHOPTIMEOUT = 1127;

		public const int SCID_NTF_CLRSHOPREFRESH = 1128;

		public const int CSID_CMD_SYMBOLCOMP = 1130;

		public const int SCID_CMD_SYMBOLCOMP = 1131;

		public const int CSID_CMD_SYMBOLQUERY = 1132;

		public const int SCID_CMD_SYMBOLDETAIL = 1133;

		public const int CSID_CMD_SYMBOLWEAR = 1134;

		public const int CSID_CMD_SYMBOLOFF = 1135;

		public const int SCID_CMD_SYMBOLCHG = 1136;

		public const int CSID_CMD_SYMBOLPAGESEL = 1137;

		public const int SCID_CMD_SYMBOLPAGESEL = 1138;

		public const int SCID_CMD_SYMBOLCHG_LIST = 1139;

		public const int CSID_CMD_EQUIPSMELT = 1140;

		public const int CSID_CMD_EQUIPENCHANT = 1141;

		public const int SCID_CMD_EQUIPENCHANT = 1142;

		public const int SCID_CMD_COINDRAW_RESULT = 1143;

		public const int CSID_CMD_GEAR_LVLUP = 1144;

		public const int CSID_CMD_GEAR_LVLUPALL = 1145;

		public const int SCID_CMD_GEAR_LEVELINFO = 1146;

		public const int CSID_CMD_GEAR_ADVANCE = 1147;

		public const int SCID_CMD_GEAR_ADVANCE = 1148;

		public const int SCID_CMD_PROPUSE_GIFTGET = 1149;

		public const int CSID_CMD_ACNTCOUPONS = 1150;

		public const int SCID_CMD_ACNTCOUPONS = 1151;

		public const int SCID_CMD_SPECIALDETAIL = 1152;

		public const int CSID_CMD_BUY_SPECSALE = 1153;

		public const int SCID_CMD_BUY_SPECSALE = 1154;

		public const int CSID_CMD_SYMBOL_MAKE = 1155;

		public const int CSID_CMD_SYMBOL_BREAK = 1156;

		public const int SCID_CMD_SYMBOL_MAKE = 1157;

		public const int SCID_CMD_SYMBOL_BREAK = 1158;

		public const int CSID_CMD_COUPONS_REWARDGET = 1160;

		public const int SCID_CMD_COUPONS_REWARDINFO = 1161;

		public const int CSID_CMD_SYMBOLPAGE_CLR = 1162;

		public const int SCID_CMD_SYMBOLPAGE_CLR = 1163;

		public const int CSID_CMD_TALENT_BUY = 1164;

		public const int SCID_CMD_TALENT_BUY = 1165;

		public const int CSID_CMD_SKILLUNLOCK_SEL = 1166;

		public const int SCID_CMD_SKILLUNLOCK_SEL = 1167;

		public const int SCID_CMD_HERO_WAKECHG = 1168;

		public const int CSID_CMD_HERO_WAKEOPT = 1169;

		public const int SCID_CMD_HERO_WAKESTEP = 1170;

		public const int SCID_CMD_HEROWAKE_REWARD = 1171;

		public const int SCID_CMD_PROPUSE = 1172;

		public const int CSID_CMD_SALERECMD_BUY = 1175;

		public const int SCID_CMD_SALERECMD_BUY = 1176;

		public const int CSID_CMD_RANDDRAW_REQ = 1177;

		public const int SCID_CMD_RANDDRAW_RSP = 1178;

		public const int SCID_NTF_RANDDRAW_SYNID = 1179;

		public const int CSID_CMD_SYMBOLRCMD_WEAR = 1180;

		public const int SCID_CMD_SYMBOLRCMD_WEAR = 1181;

		public const int CSID_CMD_SYMBOLRCMD_SEL = 1182;

		public const int SCID_CMD_SYMBOLRCMD_SEL = 1183;

		public const int CSID_CMD_RAREEXCHANGE_REQ = 1184;

		public const int SCID_CMD_RAREEXCHANGE_RSP = 1185;

		public const int SCID_NTF_SYMBOLPAGESYN = 1186;

		public const int SCID_NTF_ERRCODE = 1190;

		public const int SCID_NTF_NEWIEBITSYN = 1191;

		public const int SCID_NTF_NEWIEALLBITSYN = 1192;

		public const int CSID_CMD_CHGSIGNATURE = 1193;

		public const int SCID_CMD_CHGSIGNATURE = 1194;

		public const int CSID_CMD_FRIENDLIST = 1200;

		public const int SCID_CMD_FRIENDLIST = 1201;

		public const int CSID_CMD_FRIENDREQLIST = 1202;

		public const int SCID_CMD_FRIENDREQLIST = 1203;

		public const int CSID_CMD_SEARCHPLAYER = 1204;

		public const int SCID_CMD_SEARCHPLAYER = 1205;

		public const int CSID_CMD_FRIENDADD = 1206;

		public const int SCID_CMD_FRIENDADD = 1207;

		public const int CSID_CMD_FRIENDDEL = 1208;

		public const int SCID_CMD_FRIENDDEL = 1209;

		public const int CSID_CMD_FRIENDADDCONFIRM = 1210;

		public const int SCID_CMD_FRIENDADDCONFIRM = 1211;

		public const int CSID_CMD_FRIENDADDDENY = 1212;

		public const int SCID_CMD_FRIENDADDDENY = 1213;

		public const int CSID_CMD_FRIENDINVITEGAME = 1214;

		public const int SCID_CMD_FRIENDINVITEGAME = 1215;

		public const int CSID_CMD_FRIENDRECEIVEACHIEVE = 1216;

		public const int SCID_CMD_FRIENDRECEIVEACHIEVE = 1217;

		public const int CSID_CMD_FRIENDDONATEPOINT = 1218;

		public const int SCID_CMD_FRIENDDONATEPOINT = 1219;

		public const int CSID_CMD_FRIENDDONATEPOINTALL = 1222;

		public const int SCID_CMD_FRIENDDONATEPOINTALL = 1223;

		public const int SCID_CMD_NTF_CHGINTIMACY = 1224;

		public const int CSID_CMD_NOASKFORFLAG_CHG = 1225;

		public const int SCID_CMD_NOASKFORFLAG_CHG = 1226;

		public const int SCID_NTF_FRIEND_NOASKFOR_FLAGCHG = 1227;

		public const int CSID_CMD_FRIENDINVITEINFO = 1228;

		public const int SCID_CMD_FRIENDINVITEINFO = 1229;

		public const int SCID_CMD_NTF_FRIEND_REQUEST = 1230;

		public const int SCID_CMD_NTF_FRIEND_ADD = 1231;

		public const int SCID_CMD_NTF_FRIEND_DEL = 1232;

		public const int SCID_CMD_NTF_FRIEND_LOGIN_STATUS = 1233;

		public const int CSID_CMD_LIST_FREC = 1234;

		public const int SCID_CMD_LIST_FREC = 1235;

		public const int CSID_CMD_FRIENDRECALLPOINT = 1236;

		public const int SCID_CMD_FRIENDRECALLPOINT = 1237;

		public const int SCID_CMD_NTF_RECALL_FRIEND = 1238;

		public const int SCID_CMD_BLACKLIST = 1239;

		public const int CSID_OPER_HERO_REQ = 1240;

		public const int SCID_OPER_HERO_NTF = 1241;

		public const int CSID_CMD_HERO_CONFIRM = 1242;

		public const int SCID_HERO_CONFIRM_NTF = 1243;

		public const int SCID_DEFAULT_HERO_NTF = 1244;

		public const int CSID_HERO_CANCEL_CONFIRM = 1245;

		public const int SCID_HERO_CANCEL_CONFIRM_NTF = 1246;

		public const int CSID_SHOW_HERO_WIN_RATIO = 1247;

		public const int SCID_SHOW_HERO_WIN_RATIO_NTF = 1248;

		public const int CSID_RELAYSVRPING = 1260;

		public const int CSID_GAMESVRPING = 1261;

		public const int CSID_CLRCDLIMIT_REQ = 1262;

		public const int SCID_CLRCDLIMIT_RSP = 1263;

		public const int CSID_RELAYHASHCHECK = 1280;

		public const int SCID_NEXTFIRSTWINSEC_NTF = 1281;

		public const int CSID_COIN_GET_PATH_REQ = 1282;

		public const int SCID_COIN_GET_PATH_RSP = 1283;

		public const int SCID_RELAYHASHCHECK = 1284;

		public const int CSID_CMD_CHAT_REQ = 1300;

		public const int CSID_CMD_GET_CHAT_MSG_REQ = 1301;

		public const int SCID_CMD_CHAT_NTF = 1302;

		public const int CSID_CMD_CHAT_COMPLAINT_REQ = 1303;

		public const int SCID_CMD_CHAT_COMPLAINT_RSP = 1304;

		public const int CSID_CMD_GET_HORNMSG = 1305;

		public const int SCID_CMD_GET_HORNMSG = 1306;

		public const int SCID_OFFLINE_CHAT_NTF = 1307;

		public const int CSID_CLEAN_OFFLINE_CHAT_REQ = 1308;

		public const int SCID_LEAVE_SETTLEUI_NTF = 1309;

		public const int CSID_CMD_GAINCHEST = 1320;

		public const int SCID_CMD_GAINCHEST = 1321;

		public const int CSID_CMD_FRIREFUSERECALL = 1322;

		public const int SCID_CMD_NTF_REFUSERECALL = 1323;

		public const int SCID_CMD_FRIREFUSERECALL = 1324;

		public const int CSID_CMD_DEFRIEND = 1325;

		public const int SCID_CMD_DEFRIEND = 1326;

		public const int CSID_CMD_CANCELDEFRIEND = 1327;

		public const int SCID_CMD_CANCELDEFRIEND = 1328;

		public const int CSID_CMD_LBSREPORT = 1329;

		public const int CSID_CMD_LBSSEARCH = 1330;

		public const int SCID_CMD_LBSSEARCH = 1331;

		public const int CSID_CMD_LBSREMOVE = 1332;

		public const int CSID_CMD_LIKE_REQ = 1333;

		public const int SCID_CMD_NTF_LIKE = 1334;

		public const int CSID_CMD_LICENSE_REQ = 1350;

		public const int SCID_CMD_LICENSE_RSP = 1351;

		public const int CSID_CMD_INTIMACY_RELATION_REQUEST = 1360;

		public const int SCID_CMD_INTIMACY_RELATION_REQUEST = 1361;

		public const int SCID_CMD_NTF_INTIMACY_RELATION_REQUEST = 1362;

		public const int CSID_CMD_CHG_INTIMACYCONFIRM = 1363;

		public const int SCID_CMD_CHG_INTIMACYCONFIRM = 1364;

		public const int SCID_CMD_NTF_CHG_INTIMACYCONFIRM = 1365;

		public const int CSID_CMD_CHG_INTIMACYDENY = 1366;

		public const int SCID_CMD_CHG_INTIMACYDENY = 1367;

		public const int SCID_CMD_NTF_CHG_INTIMACYDENY = 1368;

		public const int CSID_CMD_CHG_INTIMACYPRIORITY = 1369;

		public const int CSID_RECRUITMENT_REWARD_REQ = 1370;

		public const int SCID_RECRUITMENT_REWARD_RSP = 1371;

		public const int SCID_RECRUITMENT_REWARD_ERR_NTF = 1372;

		public const int SCID_RECRUITMENT_ERR_NTF = 1373;

		public const int SCID_CHG_RECRUITMENT_NTF = 1374;

		public const int CSID_CMD_ASKFORREQ_GET = 1379;

		public const int SCID_CMD_ASKFORREQ_GET = 1380;

		public const int SCID_CMD_ASKFORREQ_GETFAIL = 1381;

		public const int CSID_CMD_ASKFORREQ_DEL = 1382;

		public const int SCID_CMD_ASKFORREQ_DEL = 1383;

		public const int CSID_CMD_ASKFORREQ_SEND = 1384;

		public const int SCID_CMD_ASKFORREQ_SEND = 1385;

		public const int CSID_CMD_ASKFORREQ_READ = 1386;

		public const int SCID_CMD_ASKFORREQ_READ = 1387;

		public const int CSID_CMD_ASKFORREQ_REFUSE = 1388;

		public const int SCID_CMD_ASKFORREQ_REFUSE = 1389;

		public const int CSID_CMD_ASKFORREQ_CONFIRM = 1390;

		public const int SCID_CMD_ASKFORREQ_CONFIRM = 1391;

		public const int CSID_CMD_MAILOPT = 1400;

		public const int SCID_CMD_MAILOPT = 1401;

		public const int CSID_CMD_FUNCUNLOCK_REQ = 1402;

		public const int SCID_ACNT_DETAILINFO_RSP = 1403;

		public const int SCID_ACNT_HEAD_URL_CHG_NTF = 1404;

		public const int SCID_ACNT_SELF_MSG_INFO_RSP = 1405;

		public const int SCID_CMD_AKALISHOP_DETAIL = 1406;

		public const int CSID_CMD_AKALISHOP_BUY = 1407;

		public const int SCID_CMD_AKALISHOP_BUY = 1408;

		public const int CSID_CMD_AKALISHOP_FLAG = 1409;

		public const int SCID_CMD_AKALISHOP_FLAG = 1410;

		public const int SCID_CMD_HONORINFOCHG_RSP = 1414;

		public const int SCID_CMD_HONORINFO_RSP = 1415;

		public const int CSID_CMD_USEHONOR_REQ = 1416;

		public const int SCID_CMD_USEHONOR_RSP = 1417;

		public const int CSID_CMD_NOTICE_NEW = 1420;

		public const int SCID_CMD_NOTICE_NEW = 1421;

		public const int CSID_CMD_NOTICE_LIST = 1422;

		public const int SCID_CMD_NOTICE_LIST = 1423;

		public const int CSID_CMD_NOTICE_INFO = 1424;

		public const int SCID_CMD_NOTICE_INFO = 1425;

		public const int CSID_SEND_GUILD_MAIL_REQ = 1430;

		public const int SCID_SEND_GUILD_MAIL_RSP = 1431;

		public const int CSID_CMD_FIGHTHISTORY_REQ = 1440;

		public const int CSID_CMD_FIGHTHISTORYLIST_REQ = 1441;

		public const int SCID_CMD_FIGHTHISTORYLIST_RSP = 1442;

		public const int SCID_ROLLING_MSG_NTF = 1450;

		public const int CSID_CMD_REDDOT_LIST = 1470;

		public const int SCID_CMD_REDDOT_LIST = 1471;

		public const int CSID_CMD_CHKXUNYOU_SERV = 1480;

		public const int CSID_USUALTASK_REQ = 1500;

		public const int SCID_USUALTASK_RES = 1501;

		public const int CSID_TASKSUBMIT_REQ = 1502;

		public const int SCID_TASKSUBMIT_RES = 1503;

		public const int CSID_TASKUPD_NTF = 1504;

		public const int CSID_CLIENTREPORT_TASK_DONE = 1505;

		public const int SCID_USUALTASKDISCARD_RES = 1506;

		public const int SCID_NEWTASKGET_NTF = 1507;

		public const int SCID_DELTASK_NTF = 1508;

		public const int SCID_HUOYUEDUINFO_NTF = 1600;

		public const int CSID_GETHUOYUEDUREWARD_REQ = 1601;

		public const int SCID_GETHUOYUEDUREWARD_RSP = 1602;

		public const int SCID_HUOYUEDUREWARDERR_NTF = 1603;

		public const int CSID_GETPVPLEVELREWARD_REQ = 1614;

		public const int SCID_GETPVPLEVELREWARD_RSP = 1615;

		public const int CSID_ACHIEVEHERO_REQ = 1800;

		public const int SCID_ACHIEVEHERO_RSP = 1801;

		public const int SCID_ACNTHEROINFO_NTY = 1802;

		public const int SCID_CMD_HEROEXP_ADD = 1803;

		public const int SCID_ADDHERO_NTY = 1804;

		public const int CSID_BATTLELIST_REQ = 1805;

		public const int SCID_BATTLELIST_RSP = 1806;

		public const int SCID_BATTLELIST_NTY = 1807;

		public const int CSID_UPGRADESTAR_REQ = 1808;

		public const int SCID_UPGRADESTAR_RSP = 1809;

		public const int SCID_HERO_INFO_UPD = 1810;

		public const int CSID_SKILLUPDATE_REQ = 1811;

		public const int SCID_SKILLUPDATE_RSP = 1812;

		public const int SCID_GMADDHERO_RSP = 1813;

		public const int CSID_FREEHERO_REQ = 1814;

		public const int SCID_FREEHERO_RSP = 1815;

		public const int SCID_GMUNLOCKHEROPVP_RSP = 1816;

		public const int CSID_BUYHERO_REQ = 1817;

		public const int SCID_BUYHERO_RSP = 1818;

		public const int CSID_BUYHEROSKIN_REQ = 1819;

		public const int SCID_BUYHEROSKIN_RSP = 1820;

		public const int CSID_WEARHEROSKIN_REQ = 1821;

		public const int SCID_WEARHEROSKIN_RSP = 1822;

		public const int SCID_HEROSKIN_ADD = 1823;

		public const int CSID_UPHEROLVL_REQ = 1824;

		public const int SCID_UPHEROLVL_RSP = 1825;

		public const int SCID_LIMITSKIN_ADD = 1826;

		public const int SCID_LIMITSKIN_DEL = 1827;

		public const int SCID_USEEXPCARD_NTF = 1828;

		public const int SCID_GM_ADDALLSKIN_RSP = 1829;

		public const int CSID_PRESENTHERO_REQ = 1830;

		public const int SCID_PRESENTHERO_RSP = 1831;

		public const int CSID_PRESENTSKIN_REQ = 1832;

		public const int SCID_PRESENTSKIN_RSP = 1833;

		public const int CSID_ACNT_PSWDINFO_OPEN = 1901;

		public const int SCID_ACNT_PSWDINFO_OPEN = 1902;

		public const int CSID_ACNT_PSWDINFO_CLOSE = 1903;

		public const int SCID_ACNT_PSWDINFO_CLOSE = 1904;

		public const int CSID_ACNT_PSWDINFO_CHG = 1905;

		public const int SCID_ACNT_PSWDINFO_CHG = 1906;

		public const int CSID_ACNT_PSWDINFO_FORCE = 1907;

		public const int SCID_ACNT_PSWDINFO_FORCE = 1908;

		public const int CSID_ACNT_PSWDINFO_FORCECAL = 1909;

		public const int SCID_ACNT_PSWDINFO_FORCECAL = 1910;

		public const int CSID_MATCH_REQ = 2010;

		public const int SCID_MATCH_RSP = 2011;

		public const int SCID_ROOM_STARTSINGLEGAME_NTF = 2012;

		public const int CSID_START_MULTI_GAME_REQ = 2013;

		public const int SCID_START_MULTI_GAME_RSP = 2014;

		public const int CSID_ADD_NPC_REQ = 2015;

		public const int SCID_ADD_NPC_RSP = 2016;

		public const int CSID_INVITE_FRIEND_JOIN_ROOM_REQ = 2017;

		public const int SCID_INVITE_FRIEND_JOIN_ROOM_RSP = 2018;

		public const int CSID_KICKOUT_ROOM_MEMBER_REQ = 2019;

		public const int SCID_INVITE_JOIN_GAME_REQ = 2020;

		public const int CSID_INVITE_JOIN_GAME_RSP = 2021;

		public const int CSID_CREATE_TEAM_REQ = 2022;

		public const int SCID_JOIN_TEAM_RSP = 2023;

		public const int CSID_INVITE_FRIEND_JOIN_TEAM_REQ = 2024;

		public const int SCID_INVITE_FRIEND_JOIN_TEAM_RSP = 2025;

		public const int SCID_TEAM_CHG = 2026;

		public const int CSID_LEAVE_TEAM = 2027;

		public const int CSID_TEAM_OPER_REQ = 2028;

		public const int SCID_SELF_BEKICK_FROM_TEAM = 2029;

		public const int SCID_ACNT_LEAVE_TEAM_RSP = 2030;

		public const int CSID_ROOM_CONFIRM_REQ = 2031;

		public const int SCID_ROOM_CONFIRM_RSP = 2032;

		public const int CSID_ROOM_CHGMEMBERPOS_REQ = 2033;

		public const int CSID_INVITE_GUILD_MEMBER_JOIN_REQ = 2034;

		public const int CSID_GET_GUILD_MEMBER_GAME_STATE_REQ = 2035;

		public const int SCID_GET_GUILD_MEMBER_GAME_STATE_RSP = 2036;

		public const int CSID_ROOM_CHGPOS_CONFIRM_REQ = 2037;

		public const int SCID_ROOM_CHGPOS_NTF = 2038;

		public const int CSID_GET_PREPARE_GUILD_LIST_REQ = 2203;

		public const int SCID_GET_PREPARE_GUILD_LIST_RSP = 2204;

		public const int CSID_GET_GUILD_INFO_REQ = 2205;

		public const int SCID_GET_GUILD_INFO_RSP = 2206;

		public const int CSID_GET_PREPARE_GUILD_INFO_REQ = 2207;

		public const int SCID_GET_PREPARE_GUILD_INFO_RSP = 2208;

		public const int CSID_CREATE_GUILD_REQ = 2209;

		public const int SCID_CREATE_GUILD_RSP = 2210;

		public const int CSID_JOIN_PREPARE_GUILD_REQ = 2211;

		public const int SCID_JOIN_PREPARE_GUILD_RSP = 2212;

		public const int SCID_JOIN_PREPARE_GUILD_NTF = 2213;

		public const int SCID_ADD_GUILD_NTF = 2214;

		public const int SCID_MEMBER_ONLINE_NTF = 2215;

		public const int SCID_PREPARE_GUILD_BREAK_NTF = 2216;

		public const int CSID_MODIFY_GUILD_SETTING_REQ = 2217;

		public const int SCID_MODIFY_GUILD_SETTING_RSP = 2218;

		public const int CSID_GET_GUILD_APPLY_LIST_REQ = 2219;

		public const int SCID_GET_GUILD_APPLY_LIST_RSP = 2220;

		public const int CSID_APPLY_JOIN_GUILD_REQ = 2221;

		public const int SCID_APPLY_JOIN_GUILD_RSP = 2222;

		public const int SCID_JOIN_GUILD_APPLY_NTF = 2223;

		public const int SCID_NEW_MEMBER_JOIN_GULD_NTF = 2224;

		public const int CSID_APPROVE_JOIN_GUILD_APPLY = 2225;

		public const int CSID_QUIT_GUILD_REQ = 2226;

		public const int SCID_QUIT_GUILD_RSP = 2227;

		public const int SCID_QUIT_GUILD_NTF = 2228;

		public const int CSID_GUILD_INVITE_REQ = 2229;

		public const int SCID_GUILD_INVITE_RSP = 2230;

		public const int CSID_SEARCH_GUILD_REQ = 2231;

		public const int SCID_SEARCH_GUILD_RSP = 2232;

		public const int CSID_DEAL_GUILD_INVITE = 2233;

		public const int CSID_GUILD_RECOMMEND_REQ = 2234;

		public const int SCID_GUILD_RECOMMEND_RSP = 2235;

		public const int SCID_GUILD_RECOMMEND_NTF = 2236;

		public const int CSID_GET_GUILD_RECOMMEND_LIST_REQ = 2237;

		public const int SCID_GET_GUILD_RECOMMEND_LIST_RSP = 2238;

		public const int CSID_REJECT_GUILD_RECOMMEND = 2239;

		public const int SCID_DEAL_GUILD_INVITE_RSP = 2240;

		public const int CSID_SEARCH_PREGUILD_REQ = 2241;

		public const int SCID_SEARCH_PREGUILD_RSP = 2242;

		public const int SCID_GUILD_LEVEL_CHANGE_NTF = 2245;

		public const int CSID_APPOINT_POSITION_REQ = 2249;

		public const int SCID_APPOINT_POSITION_RSP = 2250;

		public const int SCID_GUILD_POSITION_CHG_NTF = 2251;

		public const int CSID_FIRE_GUILD_MEMBER_REQ = 2252;

		public const int SCID_FIRE_GUILD_MEMBER_RSP = 2253;

		public const int SCID_FIRE_GUILD_MEMBER_NTF = 2254;

		public const int SCID_GUILD_CROSS_DAY_NTF = 2264;

		public const int SCID_MEMBER_RANK_POINT_NTF = 2267;

		public const int SCID_GUILD_RANK_RESET_NTF = 2268;

		public const int SCID_GUILD_CONSTRUCT_CHG = 2271;

		public const int CSID_CHG_GUILD_HEADID_REQ = 2272;

		public const int SCID_CHG_GUILD_HEADID_RSP = 2273;

		public const int CSID_CHG_GUILD_NOTICE_REQ = 2274;

		public const int SCID_CHG_GUILD_NOTICE_RSP = 2275;

		public const int CSID_UPGRADE_GUILD_BY_COUPONS_REQ = 2276;

		public const int SCID_UPGRADE_GUILD_BY_COUPONS_RSP = 2277;

		public const int CSID_GUILD_SIGNIN_REQ = 2278;

		public const int SCID_GUILD_SIGNIN_RSP = 2279;

		public const int SCID_GUILD_SEASON_RESET_NTF = 2280;

		public const int CSID_GET_GROUP_GUILD_ID_REQ = 2281;

		public const int SCID_GET_GROUP_GUILD_ID_NTF = 2282;

		public const int CSID_SET_GUILD_GROUP_OPENID_REQ = 2283;

		public const int SCID_SET_GUILD_GROUP_OPENID_NTF = 2284;

		public const int CSID_GET_GUILD_EVENT_REQ = 2285;

		public const int SCID_GET_GUILD_EVENT_RSP = 2286;

		public const int CSID_SEND_GUILD_RECRUIT_REQ = 2287;

		public const int SCID_SEND_GUILD_RECRUIT_RSP = 2288;

		public const int CSID_GET_GUILD_RECRUIT_REQ = 2289;

		public const int SCID_GET_GUILD_RECRUIT_RSP = 2290;

		public const int CSID_GUILD_BINDQUNLOG_REQ = 2291;

		public const int CSID_GUILD_UNBINDQUNLOG_REQ = 2292;

		public const int CSID_ACNTACTIVITYINFO_REQ = 2500;

		public const int SCID_ACNTACTIVITYINFO_RSP = 2501;

		public const int SCID_ACTIVITYENDDEPLETION_NTF = 2502;

		public const int CSID_DRAWWEAL_REQ = 2503;

		public const int SCID_DRAWWEAL_RSP = 2504;

		public const int SCID_WEALDETAIL_NTF = 2505;

		public const int SCID_WEAL_CON_DATA_NTF = 2506;

		public const int CSID_WEAL_DATA_REQ = 2507;

		public const int SCID_WEAL_DATA_NTF = 2508;

		public const int SCID_PROP_MULTIPLE_NTF = 2509;

		public const int SCID_RES_DATA_NTF = 2510;

		public const int SCID_WEAL_EXCHANGE_RES = 2511;

		public const int SCID_WEAL_POINTDATA_NTF = 2512;

		public const int SCID_CLASSOFRANKDETAIL_NTF = 2600;

		public const int SCID_UPDRANKINFO_NTF = 2601;

		public const int CSID_GET_RANKING_LIST_REQ = 2602;

		public const int SCID_GET_RANKING_LIST_RSP = 2603;

		public const int CSID_GET_RANKING_ACNT_INFO_REQ = 2604;

		public const int SCID_GET_RANKING_ACNT_INFO_RSP = 2605;

		public const int CSID_GET_ACNT_DETAIL_INFO_REQ = 2606;

		public const int SCID_GET_ACNT_DETAIL_INFO_RSP = 2607;

		public const int CSID_SET_ACNT_NEWBIE_TYPE_REQ = 2608;

		public const int SCID_SET_ACNT_NEWBIE_TYPE_RSP = 2609;

		public const int CSID_GET_ACNT_RANKINFO = 2610;

		public const int SCID_ACNT_RANKINFO_RSP = 2611;

		public const int SCID_MONTH_WEEK_CARD_USE_RSP = 2612;

		public const int CSID_GET_RANKLIST_BY_SPECIAL_SCORE_REQ = 2613;

		public const int SCID_GET_RANKLIST_BY_SPECIAL_SCORE_RSP = 2614;

		public const int CSID_GET_SPECIAL_GUILD_RANK_INFO_REQ = 2615;

		public const int SCID_GET_SPECIAL_GUILD_RANK_INFO_RSP = 2616;

		public const int CSID_GET_INTIMACY_RELATION_REQ = 2617;

		public const int SCID_GET_INTIMACY_RELATION_RSP = 2618;

		public const int CSID_GET_BURNING_PROGRESS_REQ = 2700;

		public const int SCID_GET_BURNING_PROGRESS_RSP = 2701;

		public const int CSID_GET_BURNING_REWARD_REQ = 2702;

		public const int SCID_GET_BURNING_REWARD_RSP = 2703;

		public const int CSID_RESET_BURNING_PROGRESS_REQ = 2704;

		public const int SCID_RESET_BURNING_PROGRESS_RSP = 2705;

		public const int CSID_COMMONINFO_REPORT = 2800;

		public const int CSID_GAMEDATA_REPORT = 2801;

		public const int CSID_GAMELOG_REPORT = 2802;

		public const int SCID_PVPCHK_NTF_CLIENT = 2803;

		public const int CSID_GAMEDATA_REPORTOVER = 2804;

		public const int CSID_GAMELOG_REPORTOVER = 2805;

		public const int CSID_SETBATTLELIST_OF_ARENA_REQ = 2900;

		public const int SCID_SETBATTLELIST_OF_ARENA_RSP = 2901;

		public const int CSID_JOINARENA_REQ = 2903;

		public const int SCID_JOINARENA_RSP = 2904;

		public const int CSID_GETARENADATA_REQ = 2905;

		public const int SCID_GETARENADATA_RSP = 2906;

		public const int CSID_CHGARENAFIGHTER_REQ = 2907;

		public const int SCID_CHGARENAFIGHTER_RSP = 2908;

		public const int CSID_GETTOPFIGHTEROFARENA_REQ = 2909;

		public const int SCID_GETTOPFIGHTEROFARENA_RSP = 2910;

		public const int CSID_GETARENAFIGHTHISTORY_REQ = 2911;

		public const int SCID_GETARENAFIGHTHISTORY_RSP = 2912;

		public const int SCID_RANKCURSEASONHISTORY_NTF = 2913;

		public const int SCID_RANKPASTSEASONHISTORY_NTF = 2914;

		public const int CSID_GETRANKREWARD_REQ = 2915;

		public const int SCID_GETRANKREWARD_RSP = 2916;

		public const int SCID_NTF_ADDCURSEASONRECORD = 2917;

		public const int SCID_NTF_ADDPASTSEASONRECORD = 2918;

		public const int CSID_ANTIDATA_REQ = 3000;

		public const int SCID_ANTIDATA_SYN = 3001;

		public const int SCID_CREATE_TVOIP_ROOM_NTF = 3100;

		public const int SCID_JOIN_TVOIP_ROOM_NTF = 3101;

		public const int CSID_CHANGE_NAME_REQ = 3200;

		public const int SCID_CHANGE_NAME_RSP = 3201;

		public const int SCID_GUILD_NAME_CHG_NTF = 3202;

		public const int CSID_UPDATECLIENTBITS_NTF = 4000;

		public const int CSID_SERVERTIME_REQ = 4001;

		public const int SCID_SERVERTIME_RSP = 4002;

		public const int CSID_UPDNEWCLIENTBITS_NTF = 4003;

		public const int SCID_CMD_NTF_FRIEND_GAME_STATE = 4100;

		public const int SCID_NTF_SNS_FRIEND = 4101;

		public const int SCID_SNS_FRIEND_CHG_PROFILE = 4102;

		public const int SCID_NTF_ACNT_SNSNAME = 4103;

		public const int SCID_NTF_SNS_NICKNAME = 4104;

		public const int SCID_FUNCTION_SWITCH_NTF = 4110;

		public const int CSID_QQVIPINFO_REQ = 4200;

		public const int SCID_QQVIPINFO_RSP = 4201;

		public const int CSID_DIRECT_BUY_ITEM_REQ = 4301;

		public const int SCID_DIRECT_BUY_ITEM_RSP = 4302;

		public const int CSID_PVE_REVIVE_REQ = 4303;

		public const int SCID_PVE_REVIVE_RSP = 4304;

		public const int CSID_USER_COMPLAINT_REQ = 4305;

		public const int SCID_USER_COMPLAINT_RSP = 4306;

		public const int CSID_SHARE_TLOG_REQ = 4307;

		public const int SCID_SHARE_TLOG_RSP = 4308;

		public const int CSID_DYE_INBATTLE_NEWBIEBIT_REQ = 4309;

		public const int SCID_DYE_INBATTLE_NEWBIEBIT_RSP = 4310;

		public const int SCID_ACHIEVEMENT_INFO_NTF = 4401;

		public const int SCID_ACHIEVEMENT_STATE_CHG_NTF = 4402;

		public const int SCID_ACHIEVEMENT_DONE_DATA_CHG_NTF = 4403;

		public const int CSID_GET_ACHIEVEMENT_REWARD_REQ = 4404;

		public const int SCID_GET_ACHIEVEMENT_REWARD_RSP = 4405;

		public const int CSID_GET_TROPHY_REWARD_REQ = 4410;

		public const int SCID_GET_TROPHY_REWARD_RSP = 4411;

		public const int SCID_NTF_TROPHYRELVLP = 4412;

		public const int CSID_REQ_CHGSHOW = 4413;

		public const int SCID_RSP_CHGSHOW = 4414;

		public const int SCID_DAILY_CHECK_DATA_NTF = 4500;

		public const int SCID_GAME_VIP_NTF = 4600;

		public const int CSID_HEADIMG_CHG_REQ = 4601;

		public const int SCID_HEADIMG_CHG_RSP = 4602;

		public const int CSID_HEADIMG_FLAGCLR_REQ = 4603;

		public const int SCID_HEADIMG_FLAGCLR_RSP = 4604;

		public const int SCID_NTF_HEADIMG_CHG = 4605;

		public const int SCID_HEADIMG_LIST_SYNC = 4606;

		public const int SCID_NTF_HEADIMG_ADD = 4607;

		public const int SCID_NTF_HEADIMG_DEL = 4608;

		public const int SCID_LUCKYDRAW_DATA_NTF = 4800;

		public const int CSID_LUCKYDRAW_REQ = 4801;

		public const int SCID_LUCKYDRAW_RSP = 4802;

		public const int CSID_LUCKYDRAW_EXTERN_REQ = 4803;

		public const int SCID_LUCKYDRAW_EXTERN_RSP = 4804;

		public const int CSID_ACNT_VOICESTATE = 4850;

		public const int SCID_NTF_VOICESTATE = 4851;

		public const int CSID_SURRENDER_REQ = 4900;

		public const int SCID_SURRENDER_RSP = 4901;

		public const int SCID_SURRENDER_NTF = 4902;

		public const int CSID_CLT_PERFORMANCE = 5000;

		public const int CSID_CLT_ACTION_STATISTICS = 5002;

		public const int SCID_ENTERTAINMENT_SYN_RAND_HERO_CNT = 5003;

		public const int CSID_GETAWARDPOOL_REQ = 5101;

		public const int SCID_GETAWARDPOOL_RSP = 5102;

		public const int SCID_MATCHPOINT_NTF = 5103;

		public const int CSID_BUY_MATCHTICKET_REQ = 5104;

		public const int SCID_BUY_MATCHTICKET_RSP = 5105;

		public const int CSID_GET_MATCHINFO_REQ = 5106;

		public const int SCID_GET_MATCHINFO_RSP = 5107;

		public const int CSID_GET_REWARDMATCH_INFO_REQ = 5108;

		public const int SCID_GET_REWARDMATCH_INFO_RSP = 5109;

		public const int CSID_REWARDMATCH_STATE_CHG_REQ = 5110;

		public const int SCID_REWARDMATCH_INFO_CHG_NTF = 5111;

		public const int CSID_SELFDEFINE_HEROEQUIP_CHG_REQ = 5201;

		public const int SCID_SELFDEFINE_HEROEQUIP_CHG_RSP = 5202;

		public const int CSID_RECOVER_SYSTEMEQUIP_REQ = 5203;

		public const int SCID_RECOVER_SYSTEMEQUIP_RSP = 5204;

		public const int SCID_MATCHTEAM_DESTROY_NTF = 5205;

		public const int CSID_GET_ACNT_CREDIT_VALUE = 5206;

		public const int SCID_NTF_ACNT_CREDIT_VALUE = 5207;

		public const int CSID_JOINMULTIGAMEREQ = 5208;

		public const int CSID_JOIN_TEAM_REQ = 5210;

		public const int SCID_NTF_CUR_BAN_PICK_INFO = 5211;

		public const int CSID_BAN_HERO_REQ = 5212;

		public const int SCID_BAN_HERO_RSP = 5213;

		public const int CSID_SWAP_HERO_REQ = 5214;

		public const int SCID_NTF_SWAP_HERO = 5215;

		public const int CSID_CONFIRM_SWAP_HERO_REQ = 5216;

		public const int SCID_NTF_CONFIRM_SWAP_HERO = 5217;

		public const int CSID_OBSERVE_FRIEND_REQ = 5218;

		public const int SCID_OBSERVE_FRIEND_RSP = 5219;

		public const int CSID_OBSERVE_GREAT_REQ = 5220;

		public const int SCID_OBSERVE_GREAT_RSP = 5221;

		public const int CSID_GET_GREATMATCH_REQ = 5222;

		public const int SCID_GET_GREATMATCH_RSP = 5223;

		public const int CSID_SELFDEFINE_CHATINFO_CHG_REQ = 5224;

		public const int SCID_SELFDEFINE_CHATINFO_CHG_RSP = 5225;

		public const int CSID_CANCEL_SWAP_HERO_REQ = 5226;

		public const int SCID_CANCEL_SWAP_HERO_RSP = 5227;

		public const int CSID_GET_VIDEOFRAPS_REQ = 5228;

		public const int SCID_GET_VIDEOFRAPS_RSP = 5229;

		public const int CSID_QUITOBSERVE_REQ = 5230;

		public const int SCID_QUITOBSERVE_RSP = 5231;

		public const int CSID_ACNT_QUITSETTLEUI_REQ = 5232;

		public const int CSID_GETFRIEND_GAMESTATE_REQ = 5233;

		public const int CSID_WEAL_CONTENT_SHARE_DONE = 5234;

		public const int SCID_TRANSDATA_RENAME_NTF = 5235;

		public const int CSID_TRANSDATA_RENAME_REQ = 5236;

		public const int SCID_TRANSDATA_RENAME_RES = 5237;

		public const int SCID_UPLOADCLTLOG_REQ = 5238;

		public const int CSID_UPLOADCLTLOG_NTF = 5239;

		public const int SCID_OBTIPS_NTF = 5240;

		public const int SCID_ACNT_OLD_TYPE_NTF = 5241;

		public const int CSID_ACNT_SET_OLD_TYPE = 5242;

		public const int CSID_PAUSE_REQ = 5243;

		public const int SCID_PAUSE_RSP = 5244;

		public const int CSID_SELECT_NEWBIE_HERO_REQ = 5245;

		public const int SCID_ACNT_MOBA_INFO = 5246;

		public const int SCID_GUILD_MATCH_SELFSIGNUPINFO_RSP = 5292;

		public const int CSID_GET_GUILD_MATCH_SIGNUPLIST_REQ = 5293;

		public const int SCID_GET_GUILD_MATCH_SIGNUPLIST_RSP = 5294;

		public const int CSID_GUILD_MATCH_SIGNUP_REQ = 5295;

		public const int SCID_GUILD_MATCH_SIGNUP_RSP = 5296;

		public const int CSID_GUILD_MATCH_MODSIGNUP_REQ = 5297;

		public const int SCID_GUILD_MATCH_MODSIGNUP_RSP = 5298;

		public const int CSID_GUILD_MATCH_GETINVITE_REQ = 5299;

		public const int SCID_GUILD_MATCH_GETINVITE_RSP = 5300;

		public const int CSID_CHG_GUILD_MATCH_LEADER_REQ = 5301;

		public const int SCID_CHG_GUILD_MATCH_LEADER_NTF = 5302;

		public const int CSID_INVITE_GUILD_MATCH_MEMBER_REQ = 5303;

		public const int SCID_INVITE_GUILD_MATCH_MEMBER_NTF = 5304;

		public const int CSID_DEAL_GUILD_MATCH_MEMBER_INVITE = 5305;

		public const int SCID_GUILD_MATCH_MEMBER_INVITE_RSP = 5306;

		public const int CSID_KICK_GUILD_MATCH_MEMBER_REQ = 5307;

		public const int CSID_LEAVE_GUILD_MATCH_TEAM_REQ = 5308;

		public const int SCID_GUILD_MATCH_MEMBER_CHG_NTF = 5309;

		public const int CSID_START_GUILD_MATCH_REQ = 5310;

		public const int CSID_SET_GUILD_MATCH_READY_REQ = 5311;

		public const int SCID_SET_GUILD_MATCH_READY_RSP = 5312;

		public const int SCID_SET_GUILD_MATCH_READY_NTF = 5313;

		public const int SCID_GUILD_MATCH_SCORE_CHG_NTF = 5314;

		public const int SCID_START_GUILD_MATCH_RSP = 5315;

		public const int SCID_GUILD_MATCH_OB_INFO_CHG = 5316;

		public const int CSID_OB_GUILD_MATCH_REQ = 5317;

		public const int SCID_OB_GUILD_MATCH_RSP = 5318;

		public const int CSID_GET_GUILD_MATCH_HISTORY_REQ = 5319;

		public const int SCID_GET_GUILD_MATCH_HISTORY_RSP = 5320;

		public const int SCID_CHG_GUILD_MATCH_LEADER_RSP = 5321;

		public const int CSID_GUILD_MATCH_REMIND_REQ = 5322;

		public const int SCID_GUILD_MATCH_REMIND_NTF = 5323;

		public const int CSID_GET_GUILD_MATCH_OB_CNT_REQ = 5324;

		public const int SCID_GET_GUILD_MATCH_OB_CNT_RSP = 5325;

		public const int SCID_SWITCHOFF_NTF = 5326;

		public const int CSID_EQUIPEVAL_REQ = 5327;

		public const int SCID_EQUIPEVAL_RSP = 5328;

		public const int CSID_GET_EQUIPEVAL_REQ = 5329;

		public const int SCID_GET_EQUIPEVAL_RSP = 5330;

		public const int SCID_GUILD_MATCH_WEEK_RESET_NTF = 5331;

		public const int CSID_GET_LADDER_REWARD_SKIN_REQ = 5332;

		public const int SCID_SERVER_PING_REQ = 5333;

		public const int CSID_SERVER_PING_RSP = 5334;

		public const int CSID_APPLY_MASTER_REQ = 5402;

		public const int SCID_APPLY_MASTER_RSP = 5403;

		public const int CSID_CONFIRM_MASTER_REQ = 5404;

		public const int SCID_CONFIRM_MASTER_RSP = 5405;

		public const int CSID_REMOVE_MASTER_REQ = 5406;

		public const int SCID_REMOVE_MASTER_RSP = 5407;

		public const int SCID_CMD_MASTERREQLIST = 5408;

		public const int SCID_CMD_MASTERREQ_NTF = 5409;

		public const int SCID_CMD_MASTERSTUDENT_INFO = 5410;

		public const int SCID_CMD_MASTERSTUDENT_ADD = 5411;

		public const int SCID_CMD_MASTERSTUDENT_DEL = 5412;

		public const int SCID_CMD_GRADUATE_NTF = 5413;

		public const int SCID_CMD_MASTERACNTDATA_NTF = 5414;

		public const int CSID_CMD_GETSTUDENTLIST_REQ = 5415;

		public const int SCID_CMD_GETSTUDENTLIST_RSP = 5416;

		public const int SCID_MASTERSTUDENT_LOGINSTATUS_NTF = 5417;

		public const int SCID_MASTERSTUDENT_NOASKFOR_NTF = 5418;

		public const int CSID_SHOWRECENTUSEDHERO_REQ = 5500;

		public const int SCID_PROFITLIMIT_NTF = 5501;

		public const int SCID_CHGNAMECD_NTF = 5502;

		public const int SCID_PVPBAN_NTF = 5503;

		public const int CSID_CMD_CHG_USEDHEROEQUIP_REQ = 5600;

		public const int SCID_CMD_CHG_USEDHEROEQUIP_RSP = 5601;

		public const int CSID_CMD_CHG_HEROEQUIPNAME_REQ = 5602;

		public const int SCID_CMD_CHG_HEROEQUIPNAME_RSP = 5603;

		public const int CSID_OPERATE_USER_PRIVACY_BIT_REQ = 5604;

		public const int SCID_OPERATE_USER_PRIVACY_BIT_RSP = 5605;

		public const int SCID_CLTUPLOADDATA_REQ = 5606;

		public const int CSID_CLTUPLOADDATA_RSP = 5607;

		public const int SCID_NTF_SVRRESTART = 5608;

		public const int CSID_CHG_FRIEND_CARD_REQ = 5609;

		public const int SCID_CHG_FRIEND_CARD_RSP = 5610;

		public const int CSID_CHG_OTHERSTATE_BIT_REQ = 5611;

		public const int SCID_CHG_OTHERSTATE_BIT_RSP = 5612;

		public const int CSID_RESERVE_MSG_REQ = 5613;

		public const int SCID_RESERVE_MSG_RSP = 5614;

		public const int SCID_RESERVE_MSG_REQ = 5615;

		public const int CSID_RESERVE_MSG_RSP = 5616;

		public const int CS_SUCCESS = 0;

		public const int CS_FAIL = 1;

		public const int CS_ERR_STARTSINGLEGAME_FAIL = 2;

		public const int CS_ERR_FINSINGLEGAME_FAIL = 4;

		public const int CS_ERR_QUITMULTGAME_FAIL = 5;

		public const int CS_ERR_REGISTER_NAME_DUP_FAIL = 6;

		public const int CS_ERR_SHOULD_REFRESH_TASK = 7;

		public const int CS_ERR_COMMIT_ERR = 8;

		public const int CS_ERR_GET_CHAPTER_REWARD_ERR = 9;

		public const int CS_ERR_QUITSINGLEGAME_FAIL = 10;

		public const int CS_ERR_GET_BURNING_PROGRESS_ERR = 11;

		public const int CS_ERR_GET_BURNING_REWARD_ERR = 12;

		public const int CS_ERR_RESET_BURNING_PROGRESS_ERR = 13;

		public const int CS_ERR_TRANK_INBANTIME = 15;

		public const int CS_ERR_RECONNLOGICWORLDIDINVALID = 16;

		public const int CS_ERR_VERSIONUPDKICK = 17;

		public const int CS_ERR_UPDCLT = 18;

		public const int CS_ERR_SVROAM = 19;

		public const int CS_ERR_PROTOCOLERR = 20;

		public const int CS_ERR_DUPLICATELOGIN = 21;

		public const int CS_ERR_GAMESVRSHUTDOWN = 22;

		public const int CS_ERR_NOTINWHITELIST = 23;

		public const int CS_ERR_INBLACKLIST = 24;

		public const int CS_ERR_ERRCLTVERSION = 25;

		public const int CS_ERR_REGISTERNAME = 30;

		public const int CS_ERR_ROOMNAME = 31;

		public const int CS_ERR_REGISTERLIMITOFPERDAY = 32;

		public const int CS_ERR_BANACNT = 33;

		public const int CS_ERR_ONLINECHKERRRELOGIN = 34;

		public const int CS_ERR_LOGINLIMIT = 35;

		public const int CS_ERR_REGISTERLIMITOFTOTAL = 36;

		public const int CS_ERR_RECRUITER_LEVELLIMIT = 37;

		public const int CS_ERR_GETVIDEOFRAPERROR = 40;

		public const int CS_ERR_HASHCHKINVALID = 41;

		public const int CS_ERR_RECRUIT_INVITECODE = 42;

		public const int CS_ERR_RECRUIT_LEVELLIMIT = 43;

		public const int CS_ERR_RECRUIT_NOTONEPLAT = 44;

		public const int CS_ERR_PASSIVE_RECRUIT_NUMLIMIT = 45;

		public const int CS_ERR_ACTIVE_RECRUIT_NUMLIMIT = 46;

		public const int CS_ERR_RECRUIT_OTHER = 47;

		public const int CS_ERR_RECRUIT_SELF = 48;

		public const int CS_ERR_RECRUIT_CLOSING = 49;

		public const int CS_ERR_REQBOOTSINGLEERROR_TOVIDEO = 50;

		public const int CS_ERR_CONSUME_BAN = 51;

		public const int CS_ERR_APOLLOPAY_FAST = 52;

		public const int CS_ERR_FRIEND_TCAPLUS_ERR = 101;

		public const int CS_ERR_FRIEND_RECORD_NOT_EXSIST = 102;

		public const int CS_ERR_FRIEND_NUM_EXCEED = 103;

		public const int CS_ERR_PEER_FRIEND_NUM_EXCEED = 104;

		public const int CS_ERR_FRIEND_DONATE_AP_EXCEED = 105;

		public const int CS_ERR_FRIEND_RECV_AP_EXCEED = 106;

		public const int CS_ERR_FRIEND_ADD_FRIEND_DENY = 107;

		public const int CS_ERR_FRIEND_ADD_FRIEND_SELF = 108;

		public const int CS_ERR_FRIEND_ADD_FRIEND_EXSIST = 109;

		public const int CS_ERR_FRIEND_REQ_REPEATED = 110;

		public const int CS_ERR_FRIEND_NOT_EXSIST = 111;

		public const int CS_ERR_FRIEND_SEND_MAIL = 112;

		public const int CS_ERR_FRIEND_DONATE_REPEATED = 113;

		public const int CS_ERR_FRIEND_AP_FULL = 114;

		public const int CS_ERR_FRIEND_LOADING = 115;

		public const int CS_ERR_FRIEND_ADD_FRIEND_ZONE = 116;

		public const int CS_ERR_FRIEND_OTHER = 117;

		public const int CS_ERR_GET_RANKING_LIST_INVALID_NUMBER_TYPE = 118;

		public const int CS_ERR_GET_ACNT_DETAIL_INFO_ERR = 119;

		public const int CS_ERR_SPECSALE_OTHER = 120;

		public const int CS_ERR_SPECSALE_OUTDATE = 121;

		public const int CS_ERR_SPECSALE_EXCEED = 122;

		public const int CS_ERR_SPECSALE_VIPLEVEL = 123;

		public const int CS_ERR_SPECSALE_BALANCE = 124;

		public const int CS_ERR_SYMBOLPAGE_NAME_ILLGEAL = 125;

		public const int CS_ERR_CHANGE_NAME_TYPE_INVALID = 126;

		public const int CS_ERR_CHANGE_NAME_ITEM_NOT_ENOUGH = 127;

		public const int CS_ERR_DB = 128;

		public const int CS_ERR_CHANGE_GUILD_NAME_AUTHORITY = 129;

		public const int CS_ERR_SHARE_TLOG_BEYOND_TIMELIMIT = 130;

		public const int CS_ERR_SURRENDER_NOT_START = 131;

		public const int CS_ERR_SURRENDER_UNVALID_PLAYER = 132;

		public const int CS_ERR_SURRENDER_CD = 133;

		public const int CS_ERR_CHAT_DENY = 134;

		public const int CS_ERR_CHAT_UNLOCK = 135;

		public const int CS_ERR_CHATPAY = 136;

		public const int CS_ERR_CHAT_CD = 137;

		public const int CS_ERR_CHAT_REPEAT = 138;

		public const int CS_ERR_CHAT_NOTFRIEND = 139;

		public const int CS_ERR_CHAT_NOTINROOM = 140;

		public const int CS_ERR_CHAT_NOTINGUILD = 141;

		public const int CS_ERR_MONTH_WEEK_CARD_EXPIRED = 142;

		public const int CS_ERR_MONTH_WEEK_CARD_REWARD_GOT = 143;

		public const int CS_ERR_FRIEND_RECALL_REPEATED = 144;

		public const int CS_ERR_FRIEND_RECALL_EXCEED = 145;

		public const int CS_ERR_FRIEND_RECALL_TIME_LIMIT = 146;

		public const int CS_ERR_CHAT_NOTINTEAM = 147;

		public const int CS_ERR_CHAT_STATE = 148;

		public const int CS_ERR_CHAT_SUBTYPE = 149;

		public const int CS_ERR_CHAT_CONTENT = 150;

		public const int CS_ERR_REFUSE_RECALL_REPEATED = 151;

		public const int CS_ERR_REFUSE_ADDFRIEND = 152;

		public const int CS_ERR_CREDIT_BAN_LADDER = 153;

		public const int CS_ERR_VERIFICATION_ILLEGAL = 154;

		public const int CS_ERR_CHAT_INBATTLE_INPUT_OFF = 155;

		public const int CS_ERR_ACNT_IN_SWAP_HERO_ACTIVE_STATUS = 156;

		public const int CS_ERR_ACNT_IN_SWAP_HERO_OPER_STATUS = 157;

		public const int CS_ERR_CHEST_HAVESHARED = 158;

		public const int CS_ERR_CHEST_CONDITION = 159;

		public const int CS_ERR_DEFRIEND_REPEATED = 160;

		public const int CS_ERR_BLACKLIST_NOT_EXSIST = 161;

		public const int CS_ERR_BLACKLIST_EXCEED = 162;

		public const int CS_ERR_FRIEND_INVALID_PLAT = 163;

		public const int CS_ERR_ACNT_NOT_IN_ACTIVE_STATUS = 164;

		public const int CS_ERR_SEND_GUILD_MAIL_AUTH = 165;

		public const int CS_ERR_SEND_GUILD_MAIL_LIMIT = 166;

		public const int CS_ERR_SEND_GUILD_MAIL_SUBJECT = 167;

		public const int CS_ERR_SEND_GUILD_MAIL_CONTENT = 168;

		public const int CS_ERR_CHAT_NOTINSETTLEUI = 169;

		public const int CS_ERR_INTIMACY_FULL = 170;

		public const int CS_ERR_FRIEND_IN_BLACK = 171;

		public const int CS_ERR_SIGNATURE_ILLEGAL = 172;

		public const int CS_ERR_ACNTFIGHTHISTORYLIST_NOTEXIST = 173;

		public const int CS_ERR_LBS_LIMIT = 174;

		public const int CS_ERR_LBSSERECH_TIMELIMIT = 175;

		public const int CS_ERR_ONLINENUM_ERR = 176;

		public const int CS_ERR_MAX_ONLINENUM = 177;

		public const int CS_ERR_ACNTPSWD_CHECK = 178;

		public const int CS_ERR_INVALID_OLD_TYPE = 179;

		public const int CS_ERR_FRIEND_ADD_LOCK = 180;

		public const int CS_ERR_FRIEND_ADD_TOO_OFTEN = 181;

		public const int CS_ERR_SWITCHOFF = 182;

		public const int CS_ERR_REDIRECT_LOGICWORLDID = 183;

		public const int CS_ERR_PASSWORD_CHECK = 184;

		public const int CS_ERR_CHGNAME_CD = 185;

		public const int CS_ERR_REQUEST_LIST_NUM_EXCEED = 186;

		public const int CS_ERR_INTIMACY_REQUEST_SELF = 187;

		public const int CS_ERR_INTIMACY_REQUEST_TIME_LIMIT = 188;

		public const int CS_ERR_INTIMACY_VALUE_NOTENOUGH = 189;

		public const int CS_ERR_INTIMACY_REQUEST_REPEATED = 190;

		public const int CS_ERR_INTIMACY_RELATION_NUM_EXCEED = 191;

		public const int CS_ERR_PEER_INTIMACY_RELATION_NUM_EXCEED = 192;

		public const int CS_ERR_INTIMACY_RELATION_EXSIST = 193;

		public const int CS_ERR_INTIMACY_RELATION_NOTEXIST = 194;

		public const int CS_ERR_INTIMACY_RELATION_OTHER = 195;

		public const int CS_ERR_INTIMACY_RELATION_OFTEN = 196;

		public const int CS_ERR_MASTERDATA = 200;

		public const int CS_ERR_MASTER_INVALID_PLAT = 201;

		public const int CS_ERR_HAVE_MASTER = 202;

		public const int CS_ERR_REFUSE_ADDMASTER = 203;

		public const int CS_ERR_APPLYREQ_TOOMUCH = 204;

		public const int CS_ERR_APPLYREQ_REPEATED = 205;

		public const int CS_ERR_STUDENT_FULL = 206;

		public const int CS_ERR_IS_MASTERORSTUDENT = 207;

		public const int CS_ERR_CANNOT_BEMASTER = 208;

		public const int CS_ERR_APPLYREQ_NOT_EXIST = 209;

		public const int CS_ERR_ISNOT_MASTER = 210;

		public const int CS_ERR_IN_24HOUR = 211;

		public const int CS_ERR_LVL_LOW = 212;

		public const int CS_ERR_LVL_HIGH = 213;

		public const int CS_ERR_GRADE_LOW = 214;

		public const int CS_ERR_TRANSACTION = 215;

		public const int CS_ERR_ISNOT_STUDENT = 216;

		public const int CS_ERR_MASTERAPPLY_SELF = 217;

		public const int CS_ERR_MASTER_BANTIME = 218;

		public const int CS_ERR_OTHER_LVL_LOW = 219;

		public const int CS_ERR_OTHER_LVL_HIGH = 220;

		public const int CS_ERR_OHTER_GRADE_LOW = 221;

		public const int CS_ERR_REQ_NOTVALID = 222;

		public const int CS_ERR_OTHER_ISSTUDENT = 223;

		public const int CS_ERR_OPERATE_USER_PRIVACY_BIT = 224;

		public const int CS_ERR_ACNT_USER_PRIVACY_FORBIDDEN = 225;

		public const int CS_ERR_EQUIPNAME = 226;

		public const int CS_ERR_DATINGDECLARATION_ILLEGAL = 227;

		public const int CS_ERR_FRIENDCARD_TAG = 228;

		public const int CS_ERR_FRIENDCARD_OTHER = 229;

		public const int CS_ERR_CHG_OTHERSTATE_BIT = 230;

		public const int CS_ERR_GET_INTIMACY_RELATION_OFTEN = 231;

		public const int CS_ERR_STUDENT_GRADE_HIGH = 232;

		public const int CS_ERR_MASTER_GRADE_LOW = 233;

		public const int CS_RESERVEMSG_SUCCESS = 0;

		public const int CS_RESERVEMSG_TEMPCLOSE = 1;

		public const int CS_RESERVEMSG_USRCLOSE = 2;

		public const int CS_RESERVEMSG_SYSTEMCLOSE = 3;

		public const int CS_RESERVEMSG_CONDITION_NOTREACH = 4;

		public const int CS_RESERVEMSG_NOTONLINE = 5;

		public const int CS_RESERVEMSG_NOTFRIEND = 6;

		public const int CS_RESERVEMSG_NOTDISTURB = 7;

		public const int CS_RESERVEMSG_CDLIMIT = 8;

		public const int CS_RESERVEMSG_OTHERERR = 9;

		public const int CS_ROOMERR_RELAYSVR_LIMIT = 1;

		public const int CS_ROOMERR_GAMEABORT = 2;

		public const int CS_ROOMERR_TIMEOUT = 3;

		public const int CS_ROOMERR_ACNTLEAVE = 4;

		public const int CS_ROOMERR_ROOM_NOT_FOUND = 5;

		public const int CS_ROOMERR_ROOM_ALREADY_START = 6;

		public const int CS_ROOMERR_ROOM_MEMBERFULL = 7;

		public const int CS_ROOMERR_ROOM_OTHERS = 8;

		public const int CS_ROOMERR_ROOM_LIMIT_PRIVILEGE = 9;

		public const int CS_ROOMERR_ROOM_INVALIDPARAM = 10;

		public const int CS_ROOMERR_ROOM_NOTFOUND_FRIEND = 11;

		public const int CS_ROOMERR_ROOM_FRIEND_OFFLINE = 12;

		public const int CS_ROOMERR_ROOM_FRIEND_BUSY = 13;

		public const int CS_ROOMERR_ROOM_INVITE_DENY = 14;

		public const int CS_ROOMERR_ROOM_UNCONFIRM = 15;

		public const int CS_ROOMERR_ROOM_TEAMERUNCONFIRM = 16;

		public const int CS_ROOMERR_ROOM_TEAMERLEAVE = 17;

		public const int CS_ROOMERR_ROOM_OTHERSUNCONFIRM = 18;

		public const int CS_ROOMERR_ROOM_OTHERSLEAVE = 19;

		public const int CS_ROOMERR_ROOM_BANTIME = 20;

		public const int CS_ROOMERR_ROOM_VERSION_LIMIT = 21;

		public const int CS_ROOMERR_ROOM_HIGHVERSION = 22;

		public const int CS_ROOMERR_ROOM_LOWVERSION = 23;

		public const int CS_ROOMERR_ROOM_LIMIT_ENTERTAINMENT_FUNC = 24;

		public const int CS_ROOMERR_ROOM_LIMIT_REWARDMATCH_FUNC = 25;

		public const int CS_ROOMERR_ROOM_INBANTIME = 26;

		public const int CS_ROOMERR_ROOM_LIMIT_FUNC = 27;

		public const int CS_ROOMERR_ROOM_BAN_PICK_LIMIT = 28;

		public const int CS_ROOMERR_PLAT_CHANNEL_CLOSE = 29;

		public const int CS_ROOMERR_ROOM_BAN_PICK_HERO_LIMIT = 30;

		public const int CS_ROOMERR_ROOM_ANTI_DISTURB = 31;

		public const int CS_HEROOPER_ERROR_CODE_OTHER = 1;

		public const int CS_HEROOPER_ERROR_CODE_INVALID_POS = 2;

		public const int CS_HEROOPER_ERROR_CODE_INVALID_HEROID = 3;

		public const int CS_HEROOPER_ERROR_CODE_DUP = 4;

		public const int CS_HEROOPER_ERROR_CODE_UNEFFECT = 5;

		public const int CS_CONFIRMHERO_ERROR_FAILED = 1;

		public const int CS_SKILLUPDATE_SUCC = 0;

		public const int CS_SKILLUPDATE_INVALIDSLOT = 1;

		public const int CS_SKILLUPDATE_UNLOCK = 2;

		public const int CS_SKILLUPDATE_LVLFULL = 3;

		public const int CS_SKILLUPDATE_NOGOLD = 4;

		public const int CS_SKILLUPDATE_NOPOINT = 5;

		public const int CS_SKILLUPDATE_OTHER = 6;

		public const int CS_HUOYUEDUREWARD_SUCC = 0;

		public const int CS_HUOYUEDUREWARD_ACNTNULL = 1;

		public const int CS_HUOYUEDUREWARD_INFONULL = 2;

		public const int CS_HUOYUEDUREWARD_NOTINTABLE = 3;

		public const int CS_HUOYUEDUREWARD_NOTENOUGH = 4;

		public const int CS_HUOYUEDUREWARD_GETED = 5;

		public const int CS_LEVELREWARD_SUCC = 0;

		public const int CS_LEVELREWARD_LEVELINVALID = 1;

		public const int CS_LEVELREWARD_HAVEGET = 2;

		public const int CS_LEVELREWARD_NOTINTABLE = 3;

		public const int CS_RECRUITMENTWARD_SUCC = 0;

		public const int CS_RECRUITMENTWARD_NOTINTABLE = 1;

		public const int CS_RECRUITMENTWARD_LEVEL_NOTENOUGH = 2;

		public const int CS_RECRUITMENTWARD_NOTRECRUITMENT = 3;

		public const int CS_RECRUITMENTWARD_GETED = 4;

		public const int CS_RECRUITMENTWARD_OTHER = 5;

		public const int CS_RECRUITMENTWARD_SUPER = 6;

		public const int CS_ACTIVITY_SUCC = 0;

		public const int CS_ACTIVITY_LOCK = 1;

		public const int CS_ACTIVITY_NOAP = 2;

		public const int CS_ACTIVITY_FULLTIMES = 3;

		public const int CS_ACTIVITY_CDLIMIT = 4;

		public const int CS_ACTIVITY_ACNTLVLLIMIT = 5;

		public const int CS_ACTIVITY_NOTFIND = 6;

		public const int CS_ACTIVITY_MEMNOTFIND = 7;

		public const int CS_ACTIVITY_OTHER = 8;

		public const int CS_SWEEP_STAR = 1;

		public const int CS_SWEEP_VIP = 2;

		public const int CS_SWEEP_AP = 3;

		public const int CS_SWEEP_TICKET = 4;

		public const int CS_SWEEP_COUPONS = 5;

		public const int CS_SWEEP_CNTLIMIT = 6;

		public const int CS_SWEEP_OTHER = 7;

		public const int CS_SWEEP_NOTLOCK = 8;

		public const int CS_ADDHEROSKIN_SKININVALID = 1;

		public const int CS_ADDHEROSKIN_PROMOTION = 2;

		public const int CS_ADDHEROSKIN_BUYFAIL = 3;

		public const int CS_ADDHEROSKIN_NOHERO = 4;

		public const int CS_ADDHEROSKIN_SKINHAS = 5;

		public const int CS_ADDHEROSKIN_COINLIMIT = 6;

		public const int CS_ADDHEROSKIN_COUPONS = 7;

		public const int CS_ADDHEROSKIN_OTHER = 8;

		public const int CS_ADDHEROSKIN_RANKGRADE = 9;

		public const int CS_ADDHERO_ACNTOWNED = 1;

		public const int CS_ADDHERO_BUYFAIL = 2;

		public const int CS_ADDHERO_HEROINVALID = 3;

		public const int CS_ADDHERO_TIME_ERROR = 4;

		public const int CS_ADDHERO_PROMOTION = 5;

		public const int CS_ADDHERO_COINLIMIT = 6;

		public const int CS_ADDHERO_PKGFULL = 7;

		public const int CS_ADDHERO_INITHERO = 8;

		public const int CS_ADDHERO_OTHER = 9;

		public const int CS_WEARHEROSKIN_NOOWNEDHERO = 1;

		public const int CS_WEARHEROSKIN_NOOWNEDSKIN = 2;

		public const int CS_WEARHEROSKIN_OTHER = 3;

		public const int CS_PRESENTHEROSKIN_SYS = 1;

		public const int CS_PRESENTHEROSKIN_LOCK = 2;

		public const int CS_PRESENTHEROSKIN_NOALLOW = 3;

		public const int CS_PRESENTHEROSKIN_UNFRIEND = 4;

		public const int CS_PRESENTHEROSKIN_COINLIMIT = 5;

		public const int CS_PRESENTHEROSKIN_MAILFAIL = 6;

		public const int CS_PRESENTHEROSKIN_PROMOTION = 7;

		public const int CS_PRESENTHEROSKIN_HEROINVALID = 8;

		public const int CS_PRESENTHEROSKIN_SKININVALID = 9;

		public const int CS_PRESENTHEROSKIN_LIMITMAX = 10;

		public const int CS_PRESENTHEROSKIN_FRIENDTIME = 11;

		public const int CS_PRESENTHEROSKIN_INVALID_PLAT = 12;

		public const int CS_PRESENTHEROSKIN_SELF = 13;

		public const int CS_PRESENTHEROSKIN_PSWDCHK = 14;

		public const int CS_PRESENTHEROSKIN_MSGDIRTY = 15;

		public const int CS_PRESENTHEROSKIN_HASHERO = 16;

		public const int CS_PRESENTHEROSKIN_HASSKIN = 17;

		public const int CS_COINDRAW_ERR_LIMIT_RESOURCE = 1;

		public const int CS_COINDRAW_ERR_PACKAGEFULL = 2;

		public const int CS_COINDRAW_ERR_MAXSTEP = 3;

		public const int CS_COINDRAW_ERR_OTHERS = 4;

		public const int CS_SINGLEGAME_ERR_FAIL = 1;

		public const int CS_SINGLEGAMEOFARENA_ERR_SELFLOCK = 2;

		public const int CS_SINGLEGAMEOFARENA_ERR_TARGETLOCK = 3;

		public const int CS_SINGLEGAMEOFARENA_ERR_TARGETCHG = 4;

		public const int CS_SINGLEGAMEOFARENA_ERR_NOTFINDTARGET = 5;

		public const int CS_SINGLEGAMEOFARENA_ERR_OTHERS = 6;

		public const int CS_SINGLEGAMEOFARENA_ERR_LIMIT_CNT = 7;

		public const int CS_SINGLEGAMEOFARENA_ERR_LIMIT_CD = 8;

		public const int CS_SINGLEGAMEOFARENA_ERR_REWARD_STATE = 9;

		public const int CS_SINGLEGAME_ERR_BANTIME = 10;

		public const int CS_SINGLEGAME_ERR_FREEHERO = 11;

		public const int CS_SINGLEGAME_ERR_EXPSKIN = 12;

		public const int CS_SINGLEGAMEOFARENA_ERR_SETTLE_CDTIME = 13;

		public const int CS_WEALDRAW_ERR_INVALIDTIME = 1;

		public const int CS_WEALDRAW_ERR_INVALIDPERIODTIME = 2;

		public const int CS_WEALDRAW_ERR_INCOMPLETE = 3;

		public const int CS_WEALDRAW_ERR_DRAWED = 4;

		public const int CS_WEALDRAW_ERR_NORES = 5;

		public const int CS_WEALDRAW_ERR_FILLINPRICE = 6;

		public const int CS_WEALDRAW_ERR_LACKCOUPONS = 7;

		public const int CS_WEALDRAW_ERR_PAY = 8;

		public const int CS_WEALDRAW_ERR_PAYTIMEOUT = 9;

		public const int CS_WEALDRAW_ERR_NOWWEALCON = 10;

		public const int CS_WEALDRAW_ERR_PERIOD = 11;

		public const int CS_WEALDRAW_ERR_NOREWARDDATA = 12;

		public const int CS_WEALDRAW_ERR_REWARDACNT = 13;

		public const int CS_WEALDRAW_ERR_OTHERS = 14;

		public const int CS_WEALDRAW_ERR_LACKITEM = 15;

		public const int CS_WEALDRAW_ERR_RECORDFULL = 16;

		public const int CS_WEALDRAW_ERR_LACKPOINT = 17;

		public const int CS_WEALDRAW_ERR_DRAWCNT = 18;

		public const int CS_WEALDRAW_EXTRA_LIMITCON_REWARD = 1;

		public const int CS_CONSUMETYPE_LIMIT = 1;

		public const int CS_UPHEROLVL_NOTREADY = 1;

		public const int CS_UPHEROLVL_NOHERO = 2;

		public const int CS_UPHEROLVL_LACKEXPORFULLLVL = 3;

		public const int CS_UPHEROLVL_OTHERS = 4;

		public const int CS_SYMBOLOPT_STATEERR = 1;

		public const int CS_SYMBOLOPT_IDINVALID = 2;

		public const int CS_SYMBOLOPT_CONSUMELIMIT = 3;

		public const int CS_SYMBOLOPT_MAKELIMIT = 4;

		public const int CS_SYMBOLOPT_MAKEERR = 5;

		public const int CS_SYMBOLOPT_BELONGHERO = 6;

		public const int CS_GETRANKREWARD_ERR_STATE_INVALID = 1;

		public const int CS_GETRANKREWARD_ERR_ALREADY_GET = 2;

		public const int CS_GETRANKREWARD_ERR_OTHERS = 3;

		public const int CS_TALENTBUY_DATAERR = 1;

		public const int CS_TALENTBUY_CONSUMELIMIT = 2;

		public const int CS_TALENTBUY_ORDERERR = 3;

		public const int CS_TALENTBUY_DUPBUY = 4;

		public const int CS_TALENTBUY_FINISH = 5;

		public const int CS_LUCKYDRAW_LACKMONEY = 1;

		public const int CS_LUCKYDRAW_MONEYTYPE = 2;

		public const int CS_LUCKYDRAW_DRAWCNT = 3;

		public const int CS_LUCKYDRAW_APOLLO = 4;

		public const int CS_LUCKYDRAW_COMSUME = 5;

		public const int CS_LUCKYDRAW_PAYTIMEOUT = 6;

		public const int CS_LUCKYDRAW_REWARDLIMIT = 7;

		public const int CS_LUCKYDRAW_OTHER = 8;

		public const int CS_LUCKYDRAW_EXTERN_MONEYTYPE = 1;

		public const int CS_LUCKYDRAW_EXTERN_PERIODINDEX = 2;

		public const int CS_LUCKYDRAW_EXTERN_NOREACH = 3;

		public const int CS_LUCKYDRAW_EXTERN_DRAWED = 4;

		public const int CS_LUCKYDRAW_EXTERN_REWARDID = 5;

		public const int CS_LUCKYDRAW_EXTERN_OTHER = 6;

		public const int CS_HEADIMAGE_DEAL_OTHER = 1;

		public const int CS_HEADIMAGE_DEAL_LOCKED = 2;

		public const int CS_SALERECOMMEND_ERR_SYS = 1;

		public const int CS_SALERECOMMEND_ERR_ID = 2;

		public const int CS_SALERECOMMEND_ERR_OUTDATE = 3;

		public const int CS_SALERECOMMEND_ERR_DUP = 4;

		public const int CS_SALERECOMMEND_ERR_PAY = 5;

		public const int CS_SALERECOMMEND_ERR_MONEY = 6;

		public const int CS_SALERECOMMEND_ERR_ITEM = 7;

		public const int CS_SALERECOMMEND_ERR_ITEM_INVALID = 8;

		public const int CS_SALERECOMMEND_ERR_BUYFAIL = 9;

		public const int CS_SALERECOMMEND_ERR_INVALDITEM = 10;

		public const int CS_RAREEXCHANGE_ERR_SYS = 1;

		public const int CS_RAREEXCHANGE_ERR_ID = 2;

		public const int CS_RAREEXCHANGE_ERR_OUTDATE = 3;

		public const int CS_RAREEXCHANGE_ERR_DUP = 4;

		public const int CS_RAREEXCHANGE_ERR_LIMIT = 5;

		public const int CS_RAREEXCHANGE_ERR_EXCHANGE = 6;

		public const int CS_RAREEXCHANGE_ERR_STATE = 7;

		public const int CS_RANDDRAW_ERR_SYS = 1;

		public const int CS_RANDDRAW_ERR_ID = 2;

		public const int CS_RANDDRAW_ERR_OUTDATE = 3;

		public const int CS_RANDDRAW_ERR_MONEY = 4;

		public const int CS_RANDDRAW_ERR_NODATA = 5;

		public const int CS_LICENSEGET_ERR_SYS = 1;

		public const int CS_LICENSEGET_ERR_ID = 2;

		public const int CS_LICENSEGET_ERR_ALREADY = 3;

		public const int CS_LICENSEGET_ERR_CONDLACK = 4;

		public const int CS_LICENSEGET_ERR_FULL = 5;

		public const int CS_BUYMATCHTICKET_SUCC = 0;

		public const int CS_BUYMATCHTICKET_ERR_FULL = 1;

		public const int CS_BUYMATCHTICKET_ERR_MONEY = 2;

		public const int CS_HORNMSG_ADD_OTHER = 1;

		public const int CS_HORNMSG_ADD_FULL = 2;

		public const int CS_HORNMSG_ADD_INVALID = 3;

		public const int CS_HORNMSG_ADD_DIRTY = 4;

		public const int CS_SYMBOLRCMD_WEAR_STATE = 1;

		public const int CS_SYMBOLRCMD_WEAR_UNSEL = 2;

		public const int CS_SYMBOLRCMD_WEAR_PAGEIDX = 3;

		public const int CS_SYMBOLRCMD_WEAR_INVALID = 4;

		public const int CS_ACNT_PSWDOPT_STATEERR = 1;

		public const int CS_ACNT_PSWDOPT_INVALID = 2;

		public const int CS_ACNT_PSWDOPT_PSWDCHECK = 3;

		public const int CS_ACNT_PSWDOPT_FORCEFAIL = 4;

		public const int CS_ACNT_ASKFORREQ_ERR_SYS = 1;

		public const int CS_ACNT_ASKFORREQ_ERR_INDEX = 2;

		public const int CS_ACNT_ASKFORREQ_ERR_UNDEL = 3;

		public const int CS_ACNT_ASKFORREQ_ERR_VERSION = 4;

		public const int CS_ACNT_ASKFORREQ_ERR_NODATA = 5;

		public const int CS_ACNT_ASKFORREQ_ERR_STATE = 6;

		public const int CS_ACNT_ASKFORREQ_ERR_MSGDIRTY = 7;

		public const int CS_ACNT_ASKFORREQ_ERR_LOCK = 8;

		public const int CS_ACNT_ASKFORREQ_ERR_SELF = 9;

		public const int CS_ACNT_ASKFORREQ_ERR_MAXCNT = 10;

		public const int CS_ACNT_ASKFORREQ_ERR_PLAT = 11;

		public const int CS_ACNT_ASKFORREQ_ERR_ITEM = 12;

		public const int CS_ACNT_ASKFORREQ_ERR_NOFRIEND = 13;

		public const int CS_ACNT_ASKFORREQ_ERR_NOALLOW = 14;

		public const int CS_ACNT_ASKFORREQ_ERR_READWAIT = 15;

		public const int CS_ACNT_ASKFORREQ_ERR_MSGTYPE = 16;

		public const int CS_ACNT_ASKFORREQ_ERR_MSGID = 17;

		public const int CS_ACNT_ASKFORREQ_ERR_PSWDCHK = 18;

		public const int CS_ACNT_ASKFORREQ_ERR_OFFSHELF = 19;

		public const int CS_ACNT_ASKFORREQ_ERR_PAY = 20;

		public const int CS_ACNT_ASKFORREQ_ERR_COINLIMIT = 21;

		public const int CS_ACNT_ASKFORREQ_ERR_MAILFAIL = 22;

		public const int CS_ACNT_ASKFORREQ_ERR_NOOPEN = 23;

		public const int CS_ACNT_ASKFORREQ_ERR_FRIENDTIME = 24;

		public const int CS_ACNT_ASKFORREQ_ERR_ITEMHAVE = 25;

		public const int CS_ACNT_ASKFORREQ_ERR_ITEMHAVE_SELF = 26;

		public const int CS_TASKUPD_PREREQUISTIE = 0;

		public const int CS_TASKUPD_COMMIT = 1;

		public const int USUALTASK_RES_NEEDUPD = 0;

		public const int USUALTASK_RES_NOTNESS = 1;

		public const int MAIL_OPT_GETLIST = 1;

		public const int MAIL_OPT_SEND = 2;

		public const int MAIL_OPT_READ = 3;

		public const int MAIL_OPT_DEL = 4;

		public const int MAIL_OPT_GETACCESS = 5;

		public const int MAIL_OPT_UNREADINFO = 6;

		public const int MAIL_OPT_MAILLISTREFRESH = 1;

		public const int MAIL_OPT_SYSMAILLISTREFRESH = 2;

		public const int MAIL_OPT_FRIENDMAILLISTREFRESH = 3;

		public const int MAIL_OPT_MAILLISTSYS = 4;

		public const int MAIL_OPT_MAILLISTFRIEND = 5;

		public const int LIST_FRIEND_RSP_PACKET_TYPE_NORMAL = 1;

		public const int LIST_FRIEND_RSP_PACKET_TYPE_LAST = 2;

		public const int LIST_FRIEND_RSP_PACKET_TYPE_CLT_ASK = 3;

		public const int RECONN_RELAY_STATE_PICK_TYPE_OTHER = 0;

		public const int RECONN_RELAY_STATE_PICK_TYPE_BANPICK = 1;

		public const int RECONN_RELAY_STATE_BAN = 1;

		public const int RECONN_RELAY_STATE_PICK = 2;

		public const int RECONN_RELAY_STATE_ADJUST = 3;

		public const int RECONN_RELAY_STATE_LOADING = 4;

		public const int RECONN_RELAY_STATE_GAMEING = 5;

		public const int RECONN_RELAY_STATE_GAMEOVER = 6;

		public const int BUY_HERO_TYPE_COUPONS = 0;

		public const int BUY_HERO_TYPE_COIN = 1;

		public const int BUY_HERO_TYPE_DIAMOND = 2;

		public const int BUY_HERO_TYPE_MIXPAY = 3;

		public const int BUY_HERO_TYPE_MAX = 4;

		public const int CS_PVP_NTF_CLIENT_TYPE_REPORT_GAMEDATA = 1;

		public const int CS_PVP_NTF_CLIENT_TYPE_REPORT_GAMELOG = 2;

		public const int CS_PVP_NTF_CLIENT_TYPE_RESTART_GAME = 3;

		public const int CS_PVP_LOG_FLAG_NEW = 1;

		public const int CS_PVP_LOG_FLAG_DATA = 2;

		public const int BUY_HEROSKIN_TYPE_COUPONS = 1;

		public const int BUY_HEROSKIN_TYPE_SKINCOIN = 2;

		public const int BUY_HEROSKIN_TYPE_DIAMOND = 3;

		public const int BUY_HEROSKIN_TYPE_MIXPAY = 4;

		public const int CS_GAMELOGOUT_ALL = 0;

		public const int CS_GAMELOGOUT_PICKWORLD = 1;

		public const int CS_UPHEROLVL_TYPE_MAX = 0;

		public const int CS_UPHEROLVL_TYPE_ONE = 1;

		public const int CS_GAMEVIP_NTF_NORMAL = 0;

		public const int CS_GAMEVIP_NTF_OPEN = 1;

		public const int CS_GAMEVIP_NTF_LOSE = 2;

		public const int CS_GAMEVIP_NTF_REGAIN = 3;

		public const int CS_GAMEVIP_NTF_UPGRADE = 4;

		public const int CS_GAMEVIP_NTF_DEGRADE = 5;

		public const int CS_HEROWAKE_OPT_START = 0;

		public const int CS_HEROWAKE_OPT_NEXT = 1;

		public const int CS_EXPCARD_HEROOWN = 2;

		public const int CS_EXPCARD_INSERTHERO = 3;

		public const int CS_EXPCARD_SKINOWN = 4;

		public const int CS_EXPCARD_INSERTSKIN = 5;

		public const int CS_EXPCARD_NOUSEHERO = 6;

		public const int CS_EXPCARD_OTHER = 7;

		public const int CS_SALERECMD_BUY_COUPONS = 1;

		public const int CS_SALERECMD_BUY_DIAMOND = 2;

		public const int CS_SALERECMD_BUY_EXCHANGE = 3;

		public const int CS_SALERECMD_BUY_COIN = 4;

		public const int CS_APOLLOFLAG_REALENV = 0;

		public const int CS_APOLLOFLAG_CHARGE = 1;

		public const int CS_HORNTYPE_SMALL = 0;

		public const int CS_HORNTYPE_BIGER = 1;

		public const int CS_HORNTYPE_MAX = 2;

		public const int CS_VOICESTATE_NONE = 0;

		public const int CS_VOICESTATE_PART = 1;

		public const int CS_VOICESTATE_FULL = 2;

		public const int BOOTFRAP_TYPE_NULL = 0;

		public const int BOOTFRAP_TYPE_CC = 1;

		public const int BOOTFRAP_TYPE_CS = 2;

		public const int BOOTFRAP_TYPE_ACNTSTATECHG = 3;

		public const int BOOTFRAP_TYPE_ASSISTSTATECHG = 4;

		public const int BOOTFRAP_TYPE_AISTATECHG = 5;

		public const int BOOTFRAP_TYPE_GAMEOVERNTF = 6;

		public const int BOOTFRAP_TYPE_PAUSE = 7;

		public const int BOOTFRAP_TYPE_MAX = 8;

		public const int CS_PAUSE_APPLY = 1;

		public const int CS_PAUSE_CANCEL = 2;

		public const int CS_MULTGAME_DIE = 1;

		public const int CS_MULTGAME_RELIVE = 2;

		public const int CSSYNC_CMD_NULL = 0;

		public const int CSSYNC_CMD_USEOBJECTIVESKILL = 128;

		public const int CSSYNC_CMD_USEDIRECTIONALSKILL = 129;

		public const int CSSYNC_CMD_USEPOSITIONSKILL = 130;

		public const int CSSYNC_CMD_MOVE = 131;

		public const int CSSYNC_CMD_BASEATTACK = 132;

		public const int CCSYNC_TYPE_COMMON = 1;

		public const int CS_ASSISI_START = 1;

		public const int CS_ASSISI_END = 2;

		public const int CS_AI_START = 1;

		public const int CS_AI_END = 2;

		public const int CS_AI_STAY_HOME = 3;

		public const int ACCEPT_AIPLAYER_NO = 0;

		public const int ACCEPT_AIPLAEYR_YES = 1;

		public const int HANGUP_START = 1;

		public const int HANGUP_END = 2;

		public const int CS_CLT_ACTION_TYPE_SECRETARY = 1;

		public const int FRAME_CMD_INVALID = 0;

		public const int FRAME_CMD_PLAYERMOVE = 1;

		public const int FRAME_CMD_PLAYERSTOPMOVE = 3;

		public const int FRAME_CMD_ATTACKPOSITION = 4;

		public const int FRAME_CMD_ATTACKACTOR = 5;

		public const int FRAME_CMD_LEARNSKILL = 6;

		public const int FRAME_CMD_USECURVETRACKSKILL = 9;

		public const int FRAME_CMD_USECOMMONATTACK = 10;

		public const int FRAME_CMD_SWITCHAOUTAI = 11;

		public const int FRAME_CMD_SWITCHCAPTAIN = 12;

		public const int FRAME_CMD_SWITCHSUPERKILLER = 13;

		public const int FRAME_CMD_SWITCHGODMODE = 14;

		public const int FRAME_CMD_LEARNTALENT = 15;

		public const int FRAME_CMD_TESTCOMMANDDELAY = 16;

		public const int FRAME_CMD_PLAYATTACKTARGETMODE = 20;

		public const int FRAME_CMD_SVRNTFCHGKFRAMELATER = 21;

		public const int FRAME_CMD_PLAYER_BUY_EQUIP = 24;

		public const int FRAME_CMD_PLAYER_SELL_EQUIP = 25;

		public const int FRAME_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE = 26;

		public const int FRAME_CMD_SET_SKILL_LEVEL = 27;

		public const int FRAME_CMD_PLAYCOMMONATTACKTMODE = 28;

		public const int FRAME_CMD_LOCKATTACKTARGET = 29;

		public const int FRAME_CMD_Signal_Btn_Position = 30;

		public const int FRAME_CMD_Signal_MiniMap_Position = 31;

		public const int FRAME_CMD_Signal_MiniMap_Target = 32;

		public const int FRAME_CMD_BUY_HORIZON_EQUIP = 34;

		public const int FRAME_CMD_PLAYER_IN_OUT_EQUIPSHOP = 35;

		public const int FRAME_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP = 36;

		public const int FRAME_CMD_PLAYLASTHITMODE = 37;

		public const int FRAME_CMD_PLAYER_CHOOSE_EQUIPSKILL = 38;

		public const int FRAME_CMD_PLAYER_CHEAT = 39;

		public const int FRAME_CMD_PLAYERATTACKORGANMODE = 40;

		public const int SC_FRAME_CMD_INVALID = 0;

		public const int SC_FRAME_CMD_PLAYERRUNAWAY = 192;

		public const int SC_FRAME_CMD_PLAYERDISCONNECT = 193;

		public const int SC_FRAME_CMD_PLAYERRECONNECT = 194;

		public const int SC_FRAME_CMD_ASSISTSTATECHG = 195;

		public const int SC_FRAME_CMD_CHGAUTOAI = 196;

		public const int SC_FRAME_CMD_SVRNTF_GAMEOVER = 197;

		public const int SC_FRAME_CMD_PAUSE_RESUME_GAME = 198;
	}
}
