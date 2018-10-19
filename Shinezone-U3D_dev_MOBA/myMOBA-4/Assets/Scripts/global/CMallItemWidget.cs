using System;
using UnityEngine;

public class CMallItemWidget : MonoBehaviour
{
	public enum NamePosition
	{
		Top,
		Bottom
	}

	public enum BuyContainer
	{
		Coin,
		Dianmond
	}

	public enum ItemType
	{
		Hero,
		Skin
	}

	public GameObject m_item;

	public GameObject m_icon;

	public GameObject m_skinLabel;

	public GameObject m_topNameContainer;

	public GameObject m_topNameLeftText;

	public GameObject m_topNameRightText;

	public GameObject m_bottomNameContainer;

	public GameObject m_bottomNameLeftText;

	public GameObject m_bottomNameRightText;

	public GameObject m_priceContainer;

	public GameObject m_orTextContainer;

	public GameObject m_middleOrText;

	public GameObject m_bottomOrText;

	public GameObject m_experienceMask;

	public GameObject m_tagContainer;

	public GameObject m_tagText;

	public GameObject m_btnGroup;

	public GameObject m_buyBtn;

	public GameObject m_buyBtnOwnedText;

	public GameObject m_buyBtnText;

	public GameObject m_linkBtn;

	public GameObject m_linkBtnText;

	public GameObject m_specialOwnedText;

	public GameObject m_askForBtn;

	public GameObject m_askForBtnText;
}
