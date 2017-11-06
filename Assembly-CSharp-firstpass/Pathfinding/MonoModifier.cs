using System;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public abstract class MonoModifier : MonoBehaviour, IPathModifier
	{
		[NonSerialized]
		public Seeker seeker;

		public int priority;

		public int Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}

		public abstract ModifierData input
		{
			get;
		}

		public abstract ModifierData output
		{
			get;
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
		}

		public void Awake()
		{
			this.seeker = base.GetComponent<Seeker>();
			if (this.seeker != null)
			{
				this.seeker.RegisterModifier(this);
			}
		}

		public void OnDestroy()
		{
			if (this.seeker != null)
			{
				this.seeker.DeregisterModifier(this);
			}
		}

		[Obsolete]
		public virtual void ApplyOriginal(Path p)
		{
		}

		public abstract void Apply(Path p, ModifierData source);

		[Obsolete]
		public virtual void PreProcess(Path p)
		{
		}

		[Obsolete]
		public virtual Vector3[] Apply(GraphNode[] path, Vector3 start, Vector3 end, int startIndex, int endIndex, NavGraph graph)
		{
			Vector3[] array = new Vector3[endIndex - startIndex];
			for (int i = startIndex; i < endIndex; i++)
			{
				array[i - startIndex] = (Vector3)path[i].position;
			}
			return array;
		}

		[Obsolete]
		public virtual Vector3[] Apply(Vector3[] path, Vector3 start, Vector3 end)
		{
			return path;
		}
	}
}
