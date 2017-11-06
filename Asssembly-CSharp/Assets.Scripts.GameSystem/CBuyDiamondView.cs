using Assets.Scripts.UI;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CBuyDiamondView
	{
		public enum enBuyDiamondViewWidget
		{
			Charge_Items
		}

		private CUIFormScript m_cuiForm;

		public CUIFormScript Form
		{
			set
			{
				this.m_cuiForm = value;
				if (this.m_cuiForm != null)
				{
					this.Draw();
				}
			}
		}

		public void Draw()
		{
			CUIListScript component = this.m_cuiForm.GetWidget(0).GetComponent<CUIListScript>();
			int[] array = new int[]
			{
				100,
				200,
				300,
				400,
				500,
				600
			};
			component.SetElementAmount(array.Length);
			byte b = 0;
			while ((int)b < array.Length)
			{
				CUIListElementScript elemenet = component.GetElemenet((int)b);
				CUIEventScript cUIEventScript = elemenet.gameObject.GetComponent<CUIEventScript>();
				if (cUIEventScript == null)
				{
					cUIEventScript = elemenet.gameObject.AddComponent<CUIEventScript>();
					cUIEventScript.Initialize(elemenet.m_belongedFormScript);
				}
				stUIEventParams eventParams = default(stUIEventParams);
				int num = array[(int)b] / 10;
				eventParams.tag = array[(int)b];
				cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Purchase_BuyDiamond, eventParams);
				Text component2 = elemenet.gameObject.transform.Find("pnlPrice/txtQuantity").GetComponent<Text>();
				Text component3 = elemenet.gameObject.transform.Find("pnlPrice/txtPrice").GetComponent<Text>();
				component2.set_text(array[(int)b].ToString());
				component3.set_text(num.ToString());
				b += 1;
			}
		}
	}
}
