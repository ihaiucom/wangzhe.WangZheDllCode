using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	public class AGE_Helper
	{
		public static VCollisionShape GetCollisionShape(ActorRoot actorRoot)
		{
			if (actorRoot == null)
			{
				return null;
			}
			VCollisionShape vCollisionShape = actorRoot.shape;
			if (vCollisionShape == null)
			{
				vCollisionShape = VCollisionShape.InitActorCollision(actorRoot);
			}
			if (vCollisionShape != null)
			{
				vCollisionShape.ConditionalUpdateShape();
			}
			return vCollisionShape;
		}
	}
}
