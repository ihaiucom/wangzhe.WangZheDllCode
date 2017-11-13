using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class GravityMovement
	{
		public const int MAX_MOTION_COUNT = 3;

		private PlayerMovement Movement;

		private ListView<SpecialMotionControler> motionControlers = new ListView<SpecialMotionControler>();

		private AccelerateMotionControler gravityControler = new AccelerateMotionControler();

		public GravityMovement(PlayerMovement _movement)
		{
			this.Movement = _movement;
			this.gravityControler.InitMotionControler(0, -98);
		}

		public void Init()
		{
			this.Movement.isFlying = false;
			this.Movement.isLerpFlying = false;
			this.motionControlers.Clear();
		}

		public void Reset()
		{
			this.Init();
			this.gravityControler.Reset();
		}

		public int GetMotionControlerCount()
		{
			return this.motionControlers.Count;
		}

		public void AddMotionControler(SpecialMotionControler _controler)
		{
			if (this.motionControlers.Count <= 3)
			{
				this.motionControlers.Add(_controler);
			}
		}

		public void RemoveMotionControler(SpecialMotionControler _controler)
		{
			this.motionControlers.Remove(_controler);
		}

		public void GravityMoveLerp(int _deltaTime, bool bReset)
		{
			int num = 0;
			Vector3 position = Vector3.zero;
			if (this.Movement == null || this.Movement.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Freeze))
			{
				return;
			}
			if (this.motionControlers.Count != 0)
			{
				this.Movement.isLerpFlying = true;
				position = this.Movement.actor.myTransform.position;
				VInt ob;
				PathfindingUtility.GetGroundY(this.Movement.actor.location, out ob);
				for (int i = 0; i < this.motionControlers.Count; i++)
				{
					SpecialMotionControler specialMotionControler = this.motionControlers[i];
					num += specialMotionControler.GetMotionLerpDistance(_deltaTime);
				}
				position.y += (float)num / 1000f;
				if ((float)ob > position.y)
				{
					position.y = (float)ob;
					this.Movement.actor.myTransform.position = position;
				}
				else
				{
					this.Movement.actor.myTransform.position = position;
				}
			}
			else if (this.Movement.isLerpFlying)
			{
				position = this.Movement.actor.myTransform.position;
				VInt ob2;
				PathfindingUtility.GetGroundY(this.Movement.actor.location, out ob2);
				if ((float)ob2 >= position.y)
				{
					position.y = (float)ob2;
					this.Movement.actor.myTransform.position = position;
					this.Movement.isLerpFlying = false;
					this.gravityControler.ResetLerpTime();
				}
				else
				{
					num = this.gravityControler.GetMotionLerpDistance(_deltaTime);
					position.y += (float)num / 1000f;
					if ((float)ob2 > position.y)
					{
						position.y = (float)ob2;
						this.Movement.actor.myTransform.position = position;
						this.Movement.isLerpFlying = false;
						this.gravityControler.ResetLerpTime();
					}
					else
					{
						this.Movement.actor.myTransform.position = position;
					}
				}
			}
		}

		public void Move(int _deltaTime)
		{
			int num = 0;
			if (!this.Movement.isFlying || this.Movement.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Freeze))
			{
				return;
			}
			if (this.motionControlers.Count != 0)
			{
				for (int i = 0; i < this.motionControlers.Count; i++)
				{
					SpecialMotionControler specialMotionControler = this.motionControlers[i];
					num += specialMotionControler.GetMotionDeltaDistance(_deltaTime);
				}
				VInt3 location = this.Movement.actor.location;
				location.y += num;
				if (this.Movement.actor.groundY.i > location.y)
				{
					location.y = this.Movement.actor.groundY.i;
					this.Movement.actor.location = location;
				}
				else
				{
					this.Movement.actor.location = location;
				}
			}
			else
			{
				VInt3 location2 = this.Movement.actor.location;
				if (this.Movement.actor.groundY.i == this.Movement.actor.location.y)
				{
					this.Movement.isFlying = false;
					this.gravityControler.ResetTime();
					return;
				}
				if (this.Movement.actor.groundY.i > this.Movement.actor.location.y)
				{
					location2.y = this.Movement.actor.groundY.i;
					this.Movement.actor.location = location2;
					this.Movement.isFlying = false;
					this.gravityControler.ResetTime();
				}
				else
				{
					num = this.gravityControler.GetMotionDeltaDistance(_deltaTime);
					location2.y += num;
					if (this.Movement.actor.groundY.i > location2.y)
					{
						location2.y = this.Movement.actor.groundY.i;
						this.Movement.actor.location = location2;
						this.Movement.isFlying = false;
						this.gravityControler.ResetTime();
					}
					else
					{
						this.Movement.actor.location = location2;
					}
				}
			}
		}
	}
}
