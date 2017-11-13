using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class DropItem
	{
		protected string Prefab;

		protected IPickupEffect PickupEffect;

		protected IDropDownEffect DropDownEffect;

		protected GameObject ItemObject;

		protected Transform CachedItemTransform;

		public bool isMoving
		{
			get
			{
				return this.DropDownEffect != null && !this.DropDownEffect.isFinished;
			}
		}

		public IPickupEffect pickupEffect
		{
			get
			{
				return this.PickupEffect;
			}
		}

		public IDropDownEffect dropDownEffect
		{
			get
			{
				return this.DropDownEffect;
			}
		}

		public DropItem(string InPrefab, IDropDownEffect InDropdownEffect, IPickupEffect InPickupEffect)
		{
			this.Prefab = InPrefab;
			this.DropDownEffect = InDropdownEffect;
			this.PickupEffect = InPickupEffect;
			if (!string.IsNullOrEmpty(this.Prefab))
			{
				GameObject gameObject = Singleton<DropItemMgr>.instance.FindPrefabObject(this.Prefab);
				if (gameObject != null)
				{
					this.ItemObject = (GameObject)Object.Instantiate(gameObject);
					this.CachedItemTransform = ((this.ItemObject != null) ? this.ItemObject.transform : null);
				}
			}
			if (InDropdownEffect != null)
			{
				InDropdownEffect.Bind(this);
			}
			if (InPickupEffect != null)
			{
				InPickupEffect.Bind(this);
			}
		}

		public void SetLocation(VInt3 Pos)
		{
			if (this.CachedItemTransform != null)
			{
				this.CachedItemTransform.position = (Vector3)Pos;
			}
		}

		public void UpdateLogic(int delta)
		{
			if (this.dropDownEffect != null && !this.dropDownEffect.isFinished)
			{
				this.dropDownEffect.OnUpdate(delta);
			}
			if (this.DropDownEffect != null && this.DropDownEffect.isFinished)
			{
				this.CheckTouch();
			}
		}

		public void Destroy()
		{
			if (this.ItemObject != null)
			{
				Object.DestroyObject(this.ItemObject);
				this.ItemObject = null;
			}
		}

		private void CheckTouch()
		{
			int num = MonoSingleton<GlobalConfig>.instance.PickupRange * MonoSingleton<GlobalConfig>.instance.PickupRange;
			DebugHelper.Assert(this.dropDownEffect != null);
			DebugHelper.Assert(this.pickupEffect != null);
			VInt3 location = this.dropDownEffect.location;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int count = heroActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				VInt3 location2 = heroActors.get_Item(i).handle.location;
				if ((location2 - location).sqrMagnitude <= (double)num && this.pickupEffect.CanPickup(heroActors.get_Item(i)))
				{
					this.pickupEffect.OnPickup(heroActors.get_Item(i));
					break;
				}
			}
		}
	}
}
