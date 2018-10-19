using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BlueBaBa : BasePet
	{
		public override void LateUpdate(int nDelta)
		{
			if (!base.CheckUpdate())
			{
				return;
			}
			this.deltaTime += nDelta;
			if (this.deltaTime > 500)
			{
				this.deltaTime -= 500;
				Vector3 position = this.petTrans.position;
				Vector3 vector = this.parentTrans.localToWorldMatrix.MultiplyPoint(this.offset);
				float num = Vector3.Distance(position, vector);
				if (num > this.offsetDistance)
				{
					this.curState = PetState.Run;
					base.PlayAnimation("Run", 0.05f);
					this.moveDir = vector - position;
					this.moveDir.Normalize();
					float num2 = (float)this.actorPtr.handle.ValueComponent.actorMoveSpeed * 1E-06f;
					if (num < 3f * this.offsetDistance)
					{
						this.moveSpeed = num2;
					}
					else if (num < 10f * this.offsetDistance)
					{
						this.moveSpeed = num2 * 1.5f;
					}
					else
					{
						this.petTrans.position = vector;
						this.petTrans.rotation = this.parentTrans.rotation;
					}
				}
				else
				{
					this.curState = PetState.Idle;
					base.PlayAnimation("Idle", 0.05f);
				}
			}
			else if (this.curState == PetState.Run)
			{
				this.petTrans.position += this.moveSpeed * this.moveDir * (float)nDelta;
				this.petTrans.rotation = base.ObjRotationLerp(this.moveDir, nDelta);
				Vector3 position = this.petTrans.position;
				Vector3 vector = this.parentTrans.localToWorldMatrix.MultiplyPoint(this.offset);
				float num = Vector3.Distance(position, vector);
				if (num < this.offsetDistance)
				{
					this.curState = PetState.Idle;
					base.PlayAnimation("Idle", 0.05f);
				}
			}
		}
	}
}
