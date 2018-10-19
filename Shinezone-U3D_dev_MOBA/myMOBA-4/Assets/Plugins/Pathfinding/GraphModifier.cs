using System;
using UnityEngine;

namespace Pathfinding
{
	public abstract class GraphModifier : MonoBehaviour
	{
		public enum EventType
		{
			PostScan = 1,
			PreScan,
			LatePostScan = 4,
			PreUpdate = 8,
			PostUpdate = 16,
			PostCacheLoad = 32
		}

		private static GraphModifier root;

		private GraphModifier prev;

		private GraphModifier next;

		public static void FindAllModifiers()
		{
			GraphModifier[] array = UnityEngine.Object.FindObjectsOfType(typeof(GraphModifier)) as GraphModifier[];
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnEnable();
			}
		}

        public static void TriggerEvent(EventType type)
        {
            if (!Application.isPlaying)
            {
                FindAllModifiers();
            }
            GraphModifier root = GraphModifier.root;
            switch (type)
            {
                case EventType.PostScan:
                    while (root != null)
                    {
                        root.OnPostScan();
                        root = root.next;
                    }
                    return;

                case EventType.PreScan:
                    while (root != null)
                    {
                        root.OnPreScan();
                        root = root.next;
                    }
                    return;

                case EventType.LatePostScan:
                    while (root != null)
                    {
                        root.OnLatePostScan();
                        root = root.next;
                    }
                    return;

                case EventType.PreUpdate:
                    while (root != null)
                    {
                        root.OnGraphsPreUpdate();
                        root = root.next;
                    }
                    return;

                case EventType.PostUpdate:
                    while (root != null)
                    {
                        root.OnGraphsPostUpdate();
                        root = root.next;
                    }
                    break;

                case EventType.PostCacheLoad:
                    while (root != null)
                    {
                        root.OnPostCacheLoad();
                        root = root.next;
                    }
                    break;
            }
        }


		protected virtual void OnEnable()
		{
			this.OnDisable();
			if (GraphModifier.root == null)
			{
				GraphModifier.root = this;
			}
			else
			{
				this.next = GraphModifier.root;
				GraphModifier.root.prev = this;
				GraphModifier.root = this;
			}
		}

		protected virtual void OnDisable()
		{
			if (GraphModifier.root == this)
			{
				GraphModifier.root = this.next;
				if (GraphModifier.root != null)
				{
					GraphModifier.root.prev = null;
				}
			}
			else
			{
				if (this.prev != null)
				{
					this.prev.next = this.next;
				}
				if (this.next != null)
				{
					this.next.prev = this.prev;
				}
			}
			this.prev = null;
			this.next = null;
		}

		public virtual void OnPostScan()
		{
		}

		public virtual void OnPreScan()
		{
		}

		public virtual void OnLatePostScan()
		{
		}

		public virtual void OnPostCacheLoad()
		{
		}

		public virtual void OnGraphsPreUpdate()
		{
		}

		public virtual void OnGraphsPostUpdate()
		{
		}
	}
}
