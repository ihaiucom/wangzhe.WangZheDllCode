using Photon;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ExitGames.Demos.DemoAnimator
{
	public class PlayerManager : PunBehaviour, IPunObservable
	{
		[Tooltip("The Player's UI GameObject Prefab")]
		public GameObject PlayerUiPrefab;

		[Tooltip("The Beams GameObject to control")]
		public GameObject Beams;

		[Tooltip("The current Health of our player")]
		public float Health = 1f;

		[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
		public static GameObject LocalPlayerInstance;

		private bool IsFiring;

		public void Awake()
		{
			if (this.Beams == null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
			}
			else
			{
				this.Beams.SetActive(false);
			}
			if (base.photonView.isMine)
			{
				PlayerManager.LocalPlayerInstance = base.gameObject;
			}
			Object.DontDestroyOnLoad(base.gameObject);
		}

		public void Start()
		{
			CameraWork component = base.gameObject.GetComponent<CameraWork>();
			if (component != null)
			{
				if (base.photonView.isMine)
				{
					component.OnStartFollowing();
				}
			}
			else
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
			}
			if (this.PlayerUiPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(this.PlayerUiPrefab) as GameObject;
				gameObject.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
			}
			else
			{
				Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
			}
		}

		public void OnDisable()
		{
		}

		public void Update()
		{
			if (base.photonView.isMine)
			{
				this.ProcessInputs();
				if (this.Health <= 0f)
				{
					GameManager.Instance.LeaveRoom();
				}
			}
			if (this.Beams != null && this.IsFiring != this.Beams.GetActive())
			{
				this.Beams.SetActive(this.IsFiring);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!base.photonView.isMine)
			{
				return;
			}
			if (!other.name.Contains("Beam"))
			{
				return;
			}
			this.Health -= 0.1f;
		}

		public void OnTriggerStay(Collider other)
		{
			if (!base.photonView.isMine)
			{
				return;
			}
			if (!other.name.Contains("Beam"))
			{
				return;
			}
			this.Health -= 0.1f * Time.deltaTime;
		}

		private void OnLevelWasLoaded(int level)
		{
			this.CalledOnLevelWasLoaded(level);
		}

		private void CalledOnLevelWasLoaded(int level)
		{
			if (!Physics.Raycast(base.transform.position, -Vector3.up, 5f))
			{
				base.transform.position = new Vector3(0f, 5f, 0f);
			}
			GameObject gameObject = Object.Instantiate(this.PlayerUiPrefab) as GameObject;
			gameObject.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}

		private void ProcessInputs()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				if (EventSystem.get_current().IsPointerOverGameObject())
				{
				}
				if (!this.IsFiring)
				{
					this.IsFiring = true;
				}
			}
			if (Input.GetButtonUp("Fire1") && this.IsFiring)
			{
				this.IsFiring = false;
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				stream.SendNext(this.IsFiring);
				stream.SendNext(this.Health);
			}
			else
			{
				this.IsFiring = (bool)stream.ReceiveNext();
				this.Health = (float)stream.ReceiveNext();
			}
		}
	}
}
