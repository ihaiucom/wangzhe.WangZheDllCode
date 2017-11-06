using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class CTongCaiSys : MonoSingleton<CTongCaiSys>
{
	public bool isOpenTongcai = true;

	public bool isChecked;

	public bool isTongCaiValid;

	public bool isCanUseTongCai = true;

	public readonly string Channel = "1100_WzryApp";

	public readonly string CheckUrlFormat = "http://chong.qq.com/tws/flowpackage/QueryOrderRelation?OutUid={0}&OutUidType={1}&Timestamp={2}&Channel=1100_WzryApp&Token={3}";

	public string ScanH5UrlFormat = "http://chong.qq.com/mobile/special_traffic_king.shtml?OutUid={0}&OutUidType={1}&Token={2}&Timestamp={3}&AccessToken={4}&Channel=1100_WzryApp";

	public readonly string TongcaiUrl = "ft.smoba.qq.com";

	public readonly string[] TongcaiIps = new string[]
	{
		string.Empty,
		"101.226.76.200",
		"140.206.160.117",
		"117.135.172.223",
		"182.254.11.206"
	};

	private float timeOut = 3f;

	public IspType supplierType;

	protected override void Init()
	{
		this.isChecked = false;
		this.isCanUseTongCai = false;
	}

	public void StartCheck(ApolloAccountInfo info)
	{
		base.StartCoroutine(this.CheckCanUseTongCai(info));
	}

	[DebuggerHidden]
	public IEnumerator CheckCanUseTongCai(ApolloAccountInfo info)
	{
		CTongCaiSys.<CheckCanUseTongCai>c__Iterator37 <CheckCanUseTongCai>c__Iterator = new CTongCaiSys.<CheckCanUseTongCai>c__Iterator37();
		<CheckCanUseTongCai>c__Iterator.info = info;
		<CheckCanUseTongCai>c__Iterator.<$>info = info;
		<CheckCanUseTongCai>c__Iterator.<>f__this = this;
		return <CheckCanUseTongCai>c__Iterator;
	}

	public void CheckDataString(string dataString)
	{
		dataString = dataString.Replace("\"", string.Empty);
		dataString = dataString.Replace("\t", string.Empty);
		dataString = dataString.Replace("\r", string.Empty);
		dataString = dataString.Replace("\n", string.Empty);
		string[] array = dataString.Split(new char[]
		{
			','
		});
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IndexOf("retCode") != -1)
			{
				string[] array2 = array[i].Split(new char[]
				{
					':'
				});
				if (array2.Length == 2)
				{
					string text = array2[1].Trim();
					if (text.Equals("0"))
					{
						flag = true;
						break;
					}
				}
			}
		}
		if (flag)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].IndexOf("state") != -1)
				{
					string[] array3 = array[j].Split(new char[]
					{
						':'
					});
					if (array3.Length == 2)
					{
						string text2 = array3[1].Trim();
						if (text2.Equals("3") || text2.Equals("4") || text2.Equals("5"))
						{
							this.isTongCaiValid = true;
						}
						break;
					}
				}
			}
		}
		else
		{
			this.isTongCaiValid = false;
		}
		this.supplierType = IspType.Null;
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k].IndexOf("supplierType") != -1)
			{
				string[] array4 = array[k].Split(new char[]
				{
					':'
				});
				if (array4.Length == 2)
				{
					string text3 = array4[1].Trim();
					if (text3.IndexOf("1") != -1)
					{
						this.supplierType = IspType.Yidong;
						break;
					}
					if (text3.IndexOf("2") != -1)
					{
						this.supplierType = IspType.Liantong;
						break;
					}
					if (text3.IndexOf("3") != -1)
					{
						this.supplierType = IspType.Dianxing;
						break;
					}
				}
			}
		}
	}

	public bool IsCanUseTongCai()
	{
		if (TdirConfig.curServerType == TdirServerType.Normal || TdirConfig.curServerType == TdirServerType.Mid)
		{
			return this.isCanUseTongCai && this.isTongCaiValid && CUICommonSystem.GetNetWorkType() == enNetWorkType.enNet;
		}
		return ApolloConfig.loginOnlyVPort >= 60000 && this.isCanUseTongCai && this.isTongCaiValid && CUICommonSystem.GetNetWorkType() == enNetWorkType.enNet;
	}

	public bool IsTongCaiUserAndCanUse()
	{
		if (TdirConfig.curServerType == TdirServerType.Normal || TdirConfig.curServerType == TdirServerType.Mid)
		{
			return this.isCanUseTongCai && this.isTongCaiValid;
		}
		return ApolloConfig.loginOnlyVPort >= 60000 && this.isCanUseTongCai && this.isTongCaiValid;
	}

	public string GetCheckUrl(ApolloAccountInfo info)
	{
		if (info.OpenId.Equals("$OPID$"))
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("openid", info.OpenId));
			string text = "OpenIdIsError";
			text += "_ANDROID";
			if (info.Platform == ApolloPlatform.Wechat)
			{
				text += "_Wechat";
			}
			else if (info.Platform == ApolloPlatform.QQ)
			{
				text += "_QQ";
			}
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent(text, list, true);
		}
		string text2 = (DateTime.get_Now().get_Ticks() / 10000000L).ToString();
		string input = string.Concat(new object[]
		{
			info.OpenId,
			(int)info.Platform,
			text2,
			this.Channel,
			CTongCaiSys.DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey),
			CTongCaiSys.DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey1)
		});
		string text3 = Utility.CreateMD5Hash(input, Utility.MD5_STRING_CASE.UPPER).ToLower();
		return string.Format(this.CheckUrlFormat, new object[]
		{
			info.OpenId,
			(int)info.Platform,
			text2,
			text3
		});
	}

	public void OpenTongCaiH5(ApolloAccountInfo info)
	{
		if (info == null || string.IsNullOrEmpty(info.OpenId))
		{
			return;
		}
		string text = string.Empty;
		if (ApolloConfig.platform == ApolloPlatform.QQ)
		{
			for (int i = 0; i < info.TokenList.Count; i++)
			{
				ApolloToken apolloToken = info.TokenList[i];
				if (apolloToken != null && apolloToken.Type == ApolloTokenType.Access)
				{
					text = apolloToken.Value;
				}
			}
		}
		string text2 = Singleton<CTextManager>.instance.GetText("TongCaiBookUrl");
		if (!string.IsNullOrEmpty(text2))
		{
			this.ScanH5UrlFormat = text2;
		}
		string text3 = (DateTime.get_Now().get_Ticks() / 10000000L).ToString();
		string input = string.Concat(new object[]
		{
			info.OpenId,
			(int)info.Platform,
			text3,
			this.Channel,
			CTongCaiSys.DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey),
			CTongCaiSys.DealStr(MonoSingleton<GameFramework>.instance.tongCaiKey1)
		});
		string text4 = Utility.CreateMD5Hash(input, Utility.MD5_STRING_CASE.UPPER).ToLower();
		string strUrl = string.Format(this.ScanH5UrlFormat, new object[]
		{
			info.OpenId,
			(int)info.Platform,
			text4,
			text3,
			text
		});
		CUICommonSystem.OpenUrl(strUrl, true, 0);
	}

	public bool IsUsingTongcaiIp()
	{
		bool result = false;
		if (string.IsNullOrEmpty(ApolloConfig.loginOnlyIpTongCai))
		{
			return result;
		}
		return ApolloConfig.loginOnlyIpTongCai.Equals(this.TongcaiIps[(int)this.supplierType]) || ApolloConfig.loginOnlyIpTongCai.Equals(this.TongcaiIps[4]);
	}

	public static string DealStr(string text)
	{
		char[] array = text.ToCharArray();
		string text2 = string.Empty;
		for (int i = array.Length - 1; i > -1; i--)
		{
			text2 += array[i];
		}
		return text2;
	}

	public static bool IsShowBuyTongCaiBtn()
	{
		uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_SHOW_BUYTONGCAI_BTN);
		return globeValue == 1u || MonoSingleton<TdirMgr>.instance.ShowBuyTongCaiBtn();
	}
}
