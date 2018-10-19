using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CampaignFormView : ActivityView
	{
		public enum WidgetDefine
		{
			EMPTY,
			Introduction,
			Banner,
			Progress,
			Rewards,
			MultiGain = 11,
			CheckIn,
			Notice,
			Exchange,
			PointsExchange,
			MAX
		}

		private const float WIDGET_SPACING_Y = 5f;

		public CampaignFormView(GameObject node, ActivityForm actvForm, Activity actv) : base(node, actvForm, actv)
		{
			this.SetActivity(actv);
		}

		public override void Clear()
		{
			base.Clear();
			for (int i = 0; i < 16; i++)
			{
				Utility.FindChildSafe(base.root, ((CampaignFormView.WidgetDefine)i).ToString()).CustomSetActive(false);
			}
		}

		public void SetActivity(Activity actv)
		{
			this.Clear();
			base.activity = actv;
			if (actv == null)
			{
				return;
			}
			char[] separator = new char[]
			{
				' ',
				','
			};
			string[] array = actv.Wigets.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			float num = 0f;
			int[] array2 = new int[16];
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				int num3 = 0;
				if (!int.TryParse(array[i], out num3) || num3 <= 0 || num3 >= 16)
				{
					DebugHelper.Assert(false, "[CampaignFormView][Activity:{0}] widgets config error!", new object[]
					{
						actv.ID
					});
				}
				else
				{
					CampaignFormView.WidgetDefine widgetDefine = (CampaignFormView.WidgetDefine)num3;
					GameObject gameObject = Utility.FindChildSafe(base.root, widgetDefine.ToString());
					if (null != gameObject)
					{
						array2[num2++] = num3;
						gameObject.SetActive(true);
						ActivityWidget activityWidget = null;
						switch (widgetDefine)
						{
						case CampaignFormView.WidgetDefine.Introduction:
							activityWidget = new IntrodWidget(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.Banner:
							activityWidget = new BannerWidget(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.Progress:
							activityWidget = new ProgressWidget(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.Rewards:
							activityWidget = new RewardWidget(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.MultiGain:
							activityWidget = new MultiGainWgt(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.CheckIn:
							activityWidget = new CheckInWidget(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.Notice:
							activityWidget = new NoticeWidget(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.Exchange:
							activityWidget = new ExchangeWgt(gameObject, this);
							break;
						case CampaignFormView.WidgetDefine.PointsExchange:
							activityWidget = new PointsExchangeWgt(gameObject, this);
							break;
						}
						IL_1AE:
						if (activityWidget != null)
						{
							activityWidget.SetPosY(num);
							num -= activityWidget.Height + 5f;
							base.WidgetList.Add(activityWidget);
							goto IL_1DA;
						}
						goto IL_1DA;
						goto IL_1AE;
					}
				}
				IL_1DA:;
			}
			for (int j = 1; j < 16; j++)
			{
				bool flag = false;
				for (int k = 0; k < num2; k++)
				{
					if (array2[k] == j)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Utility.FindChildSafe(base.root, ((CampaignFormView.WidgetDefine)j).ToString()).CustomSetActive(false);
				}
			}
			base.root.GetComponent<RectTransform>().sizeDelta = new Vector2(base.root.GetComponent<RectTransform>().sizeDelta.x, -num);
			for (int l = 0; l < base.WidgetList.Count; l++)
			{
				base.WidgetList[l].OnShow();
			}
		}
	}
}
