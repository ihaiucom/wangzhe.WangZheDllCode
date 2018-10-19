using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CUIContainerScript : CUIComponent
	{
		private const int c_elementMaxAmount = 200;

		public int m_prepareElementAmount;

		private GameObject m_elementTemplate;

		private string m_elementName;

		private int m_usedElementAmount;

		private GameObject[] m_usedElements = new GameObject[200];

		private List<GameObject> m_unusedElements = new List<GameObject>();

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			base.Initialize(formScript);
			for (int i = 0; i < base.gameObject.transform.childCount; i++)
			{
				GameObject gameObject = base.gameObject.transform.GetChild(i).gameObject;
				if (this.m_elementTemplate == null)
				{
					this.m_elementTemplate = gameObject;
					this.m_elementName = gameObject.name;
					this.m_elementTemplate.name = this.m_elementName + "_Template";
					if (this.m_elementTemplate.activeSelf)
					{
						this.m_elementTemplate.SetActive(false);
					}
				}
				gameObject.SetActive(false);
			}
			if (this.m_prepareElementAmount > 0)
			{
				for (int j = 0; j < this.m_prepareElementAmount; j++)
				{
					GameObject gameObject2 = base.Instantiate(this.m_elementTemplate);
					gameObject2.gameObject.name = this.m_elementName;
					base.InitializeComponent(gameObject2.gameObject);
					if (gameObject2.activeSelf)
					{
						gameObject2.SetActive(false);
					}
					if (gameObject2.transform.parent != base.gameObject.transform)
					{
						gameObject2.transform.SetParent(base.gameObject.transform, true);
						gameObject2.transform.localScale = Vector3.one;
					}
					this.m_unusedElements.Add(gameObject2);
				}
			}
		}

		protected override void OnDestroy()
		{
			this.m_elementTemplate = null;
			this.m_usedElements = null;
			this.m_unusedElements.Clear();
			this.m_unusedElements = null;
			base.OnDestroy();
		}

		public int GetElement()
		{
			if (this.m_elementTemplate == null || this.m_usedElementAmount >= 200)
			{
				return -1;
			}
			GameObject gameObject;
			if (this.m_unusedElements.Count > 0)
			{
				gameObject = this.m_unusedElements[0];
				this.m_unusedElements.RemoveAt(0);
			}
			else
			{
				gameObject = base.Instantiate(this.m_elementTemplate);
				gameObject.name = this.m_elementName;
				base.InitializeComponent(gameObject.gameObject);
			}
			gameObject.SetActive(true);
			for (int i = 0; i < 200; i++)
			{
				if (this.m_usedElements[i] == null)
				{
					this.m_usedElements[i] = gameObject;
					this.m_usedElementAmount++;
					return i;
				}
			}
			return -1;
		}

		public GameObject GetElement(int sequence)
		{
			if (sequence < 0 || sequence >= 200)
			{
				return null;
			}
			return (!(this.m_usedElements[sequence] == null)) ? this.m_usedElements[sequence].gameObject : null;
		}

		public void RecycleElement(int sequence)
		{
			if (this.m_elementTemplate == null || sequence < 0 || sequence >= 200)
			{
				return;
			}
			GameObject gameObject = this.m_usedElements[sequence];
			this.m_usedElements[sequence] = null;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
				if (gameObject.transform.parent != base.gameObject.transform)
				{
					gameObject.transform.SetParent(base.gameObject.transform, true);
					gameObject.transform.localScale = Vector3.one;
				}
				this.m_unusedElements.Add(gameObject);
				this.m_usedElementAmount--;
			}
		}

		public void RecycleElement(GameObject elementObject)
		{
			if (this.m_elementTemplate == null || elementObject == null)
			{
				return;
			}
			for (int i = 0; i < 200; i++)
			{
				if (this.m_usedElements[i] == elementObject)
				{
					this.m_usedElements[i] = null;
					this.m_usedElementAmount--;
					break;
				}
			}
			elementObject.SetActive(false);
			if (elementObject.transform.parent != base.gameObject.transform)
			{
				elementObject.transform.SetParent(base.gameObject.transform, true);
				elementObject.transform.localScale = Vector3.one;
			}
			this.m_unusedElements.Add(elementObject);
		}

		public void RecycleAllElement()
		{
			if (this.m_elementTemplate == null || this.m_usedElementAmount <= 0)
			{
				return;
			}
			for (int i = 0; i < 200; i++)
			{
				if (this.m_usedElements[i] != null)
				{
					this.m_usedElements[i].SetActive(false);
					if (this.m_usedElements[i].transform.parent != base.gameObject.transform)
					{
						this.m_usedElements[i].transform.SetParent(base.gameObject.transform, true);
						this.m_usedElements[i].transform.localScale = Vector3.one;
					}
					this.m_unusedElements.Add(this.m_usedElements[i]);
					this.m_usedElements[i] = null;
					this.m_usedElementAmount--;
				}
			}
		}
	}
}
