using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class PetComponent : LogicComponent
	{
		private BlueBaBa blueBaBa = new BlueBaBa();

		private RedBaBa redBaBa = new RedBaBa();

		public override void OnUse()
		{
			base.OnUse();
			if (this.blueBaBa != null)
			{
				this.blueBaBa.OnUse();
			}
			if (this.redBaBa != null)
			{
				this.redBaBa.OnUse();
			}
		}

		public override void Init()
		{
			base.Init();
			if (this.blueBaBa != null)
			{
				this.blueBaBa.Init(ref this.actorPtr);
			}
			if (this.redBaBa != null)
			{
				this.redBaBa.Init(ref this.actorPtr);
			}
		}

		public override void Uninit()
		{
			base.Uninit();
		}

		public BasePet GetPet(PetType _type)
		{
			if (_type == PetType.BlueBaBaType)
			{
				return this.blueBaBa;
			}
			if (_type == PetType.RedBaBaType)
			{
				return this.redBaBa;
			}
			return null;
		}

		public void CreatePet(PetType _type, string _prefabName, Vector3 _offset)
		{
			BasePet pet = this.GetPet(_type);
			if (pet != null)
			{
				pet.Create(_prefabName, _offset);
			}
		}

		public void DestoryPet(PetType _type)
		{
			BasePet pet = this.GetPet(_type);
			if (pet != null)
			{
				pet.Destory();
			}
		}

		public override void LateUpdate(int nDelta)
		{
			if (this.blueBaBa != null)
			{
				this.blueBaBa.LateUpdate(nDelta);
			}
			if (this.redBaBa != null)
			{
				this.redBaBa.LateUpdate(nDelta);
			}
		}
	}
}
