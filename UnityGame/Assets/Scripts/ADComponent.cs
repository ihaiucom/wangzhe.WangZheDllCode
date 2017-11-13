using System;
using UnityEngine;
using UnityEngine.UI;

public class ADComponent : MonoBehaviour
{
	private enum eADState
	{
		eFade,
		eStable
	}

	public Texture[] m_adTextures;

	public Image m_targetImage;

	public float m_fadeTime = 1f;

	public float m_stableTime = 5f;

	private ADComponent.eADState m_state = ADComponent.eADState.eStable;

	private float m_timer;

	private int m_index;

	private Material m_material;

	public void Awake()
	{
		if (this.m_adTextures.Length == 0)
		{
			return;
		}
		this.m_targetImage.gameObject.SetActive(true);
		Shader shader = Shader.Find("UI/ImageSwitch");
		this.m_material = new Material(shader);
		this.m_targetImage.set_material(this.m_material);
		this.m_material.SetTexture("_Tex1", this.m_adTextures[0]);
		this.m_material.SetFloat("_Percent", 0f);
	}

	private void Update()
	{
		if (this.m_adTextures.Length == 0)
		{
			return;
		}
		this.m_timer += Time.deltaTime;
		if (this.m_state == ADComponent.eADState.eStable)
		{
			if (this.m_timer >= this.m_stableTime)
			{
				this.m_timer = 0f;
				this.m_state = ADComponent.eADState.eFade;
				int index = this.m_index;
				this.m_index = (this.m_index + 1) % this.m_adTextures.Length;
				this.m_material.SetTexture("_Tex2", this.m_adTextures[index]);
				this.m_material.SetTexture("_Tex1", this.m_adTextures[this.m_index]);
				this.m_material.SetFloat("_Percent", 1f);
			}
		}
		else if (this.m_timer >= this.m_fadeTime)
		{
			this.m_timer = 0f;
			this.m_state = ADComponent.eADState.eStable;
			this.m_material.SetFloat("_Percent", 0f);
		}
		else
		{
			this.m_material.SetFloat("_Percent", 1f - this.m_timer / this.m_fadeTime);
		}
	}
}
