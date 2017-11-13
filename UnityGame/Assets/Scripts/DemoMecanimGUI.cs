using Photon;
using System;
using UnityEngine;

public class DemoMecanimGUI : PunBehaviour
{
	public GUISkin Skin;

	private PhotonAnimatorView m_AnimatorView;

	private Animator m_RemoteAnimator;

	private float m_SlideIn;

	private float m_FoundPlayerSlideIn;

	private bool m_IsOpen;

	public void Awake()
	{
	}

	public void Update()
	{
		this.FindRemoteAnimator();
		this.m_SlideIn = Mathf.Lerp(this.m_SlideIn, (!this.m_IsOpen) ? 0f : 1f, Time.deltaTime * 9f);
		this.m_FoundPlayerSlideIn = Mathf.Lerp(this.m_FoundPlayerSlideIn, (!(this.m_AnimatorView == null)) ? 1f : 0f, Time.deltaTime * 5f);
	}

	public void FindRemoteAnimator()
	{
		if (this.m_RemoteAnimator != null)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PhotonView component = array[i].GetComponent<PhotonView>();
			if (component != null && !component.isMine)
			{
				this.m_RemoteAnimator = array[i].GetComponent<Animator>();
			}
		}
	}

	public void OnGUI()
	{
		GUI.skin = this.Skin;
		string[] array = new string[]
		{
			"Disabled",
			"Discrete",
			"Continuous"
		};
		GUILayout.BeginArea(new Rect((float)Screen.width - 200f * this.m_FoundPlayerSlideIn - 400f * this.m_SlideIn, 0f, 600f, (float)Screen.height), GUI.skin.box);
		GUILayout.Label("Mecanim Demo", GUI.skin.customStyles[0], new GUILayoutOption[0]);
		GUI.color = Color.white;
		string text = "Settings";
		if (this.m_IsOpen)
		{
			text = "Close";
		}
		if (GUILayout.Button(text, new GUILayoutOption[]
		{
			GUILayout.Width(110f)
		}))
		{
			this.m_IsOpen = !this.m_IsOpen;
		}
		string text2 = string.Empty;
		if (this.m_AnimatorView != null)
		{
			text2 += "Send Values:\n";
			for (int i = 0; i < this.m_AnimatorView.GetSynchronizedParameters().get_Count(); i++)
			{
				PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_AnimatorView.GetSynchronizedParameters().get_Item(i);
				try
				{
					switch (synchronizedParameter.Type)
					{
					case PhotonAnimatorView.ParameterType.Float:
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							synchronizedParameter.Name,
							" (",
							this.m_AnimatorView.GetComponent<Animator>().GetFloat(synchronizedParameter.Name).ToString("0.00"),
							")\n"
						});
						break;
					}
					case PhotonAnimatorView.ParameterType.Int:
					{
						string text3 = text2;
						text2 = string.Concat(new object[]
						{
							text3,
							synchronizedParameter.Name,
							" (",
							this.m_AnimatorView.GetComponent<Animator>().GetInteger(synchronizedParameter.Name),
							")\n"
						});
						break;
					}
					case PhotonAnimatorView.ParameterType.Bool:
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							synchronizedParameter.Name,
							" (",
							(!this.m_AnimatorView.GetComponent<Animator>().GetBool(synchronizedParameter.Name)) ? "False" : "True",
							")\n"
						});
						break;
					}
					}
				}
				catch
				{
					Debug.Log("derrrr for " + synchronizedParameter.Name);
				}
			}
		}
		if (this.m_RemoteAnimator != null)
		{
			text2 += "\nReceived Values:\n";
			for (int j = 0; j < this.m_AnimatorView.GetSynchronizedParameters().get_Count(); j++)
			{
				PhotonAnimatorView.SynchronizedParameter synchronizedParameter2 = this.m_AnimatorView.GetSynchronizedParameters().get_Item(j);
				try
				{
					switch (synchronizedParameter2.Type)
					{
					case PhotonAnimatorView.ParameterType.Float:
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							synchronizedParameter2.Name,
							" (",
							this.m_RemoteAnimator.GetFloat(synchronizedParameter2.Name).ToString("0.00"),
							")\n"
						});
						break;
					}
					case PhotonAnimatorView.ParameterType.Int:
					{
						string text3 = text2;
						text2 = string.Concat(new object[]
						{
							text3,
							synchronizedParameter2.Name,
							" (",
							this.m_RemoteAnimator.GetInteger(synchronizedParameter2.Name),
							")\n"
						});
						break;
					}
					case PhotonAnimatorView.ParameterType.Bool:
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							synchronizedParameter2.Name,
							" (",
							(!this.m_RemoteAnimator.GetBool(synchronizedParameter2.Name)) ? "False" : "True",
							")\n"
						});
						break;
					}
					}
				}
				catch
				{
					Debug.Log("derrrr for " + synchronizedParameter2.Name);
				}
			}
		}
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.alignment = TextAnchor.UpperLeft;
		GUI.color = new Color(1f, 1f, 1f, 1f - this.m_SlideIn);
		GUI.Label(new Rect(10f, 100f, 600f, (float)Screen.height), text2, gUIStyle);
		if (this.m_AnimatorView != null)
		{
			GUI.color = new Color(1f, 1f, 1f, this.m_SlideIn);
			GUILayout.Space(20f);
			GUILayout.Label("Synchronize Parameters", new GUILayoutOption[0]);
			for (int k = 0; k < this.m_AnimatorView.GetSynchronizedParameters().get_Count(); k++)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				PhotonAnimatorView.SynchronizedParameter synchronizedParameter3 = this.m_AnimatorView.GetSynchronizedParameters().get_Item(k);
				GUILayout.Label(synchronizedParameter3.Name, new GUILayoutOption[]
				{
					GUILayout.Width(100f),
					GUILayout.Height(36f)
				});
				int synchronizeType = (int)synchronizedParameter3.SynchronizeType;
				int num = GUILayout.Toolbar(synchronizeType, array, new GUILayoutOption[0]);
				if (num != synchronizeType)
				{
					this.m_AnimatorView.SetParameterSynchronized(synchronizedParameter3.Name, synchronizedParameter3.Type, (PhotonAnimatorView.SynchronizeType)num);
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndArea();
	}

	public override void OnJoinedRoom()
	{
		this.CreatePlayerObject();
	}

	private void CreatePlayerObject()
	{
		Vector3 position = new Vector3(-2f, 0f, 0f);
		position.x += Random.Range(-3f, 3f);
		position.z += Random.Range(-4f, 4f);
		GameObject gameObject = PhotonNetwork.Instantiate("Robot Kyle Mecanim", position, Quaternion.identity, 0);
		this.m_AnimatorView = gameObject.GetComponent<PhotonAnimatorView>();
	}
}
