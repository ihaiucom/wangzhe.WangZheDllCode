using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AkUnityEventHandler : MonoBehaviour
{
	public const int AWAKE_TRIGGER_ID = 1151176110;

	public const int START_TRIGGER_ID = 1281810935;

	public const int DESTROY_TRIGGER_ID = -358577003;

	public const int MAX_NB_TRIGGERS = 32;

	public static Dictionary<uint, string> triggerTypes = AkTriggerBase.GetAllDerivedTypes();

	public List<int> triggerList = new List<int>
	{
		1281810935
	};

	public bool useOtherObject;

	private bool didDestroy;

	public abstract void HandleEvent(GameObject in_gameObject);

	protected virtual void Awake()
	{
		this.RegisterTriggers(this.triggerList, new AkTriggerBase.Trigger(this.HandleEvent));
		if (this.triggerList.Contains(1151176110))
		{
			this.HandleEvent(null);
		}
	}

	protected virtual void Start()
	{
		if (this.triggerList.Contains(1281810935))
		{
			this.HandleEvent(null);
		}
	}

	protected virtual void OnDestroy()
	{
		if (!this.didDestroy)
		{
			this.DoDestroy();
		}
	}

	public void DoDestroy()
	{
		this.UnregisterTriggers(this.triggerList, new AkTriggerBase.Trigger(this.HandleEvent));
		if (this.triggerList.Contains(-358577003))
		{
			this.HandleEvent(null);
		}
		this.didDestroy = true;
	}

	protected void RegisterTriggers(List<int> in_triggerList, AkTriggerBase.Trigger in_delegate)
	{
		using (List<int>.Enumerator enumerator = in_triggerList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				uint current = (uint)enumerator.Current;
				string empty = string.Empty;
				if (AkUnityEventHandler.triggerTypes.TryGetValue(current, out empty))
				{
					if (!(empty == "Awake") && !(empty == "Start") && !(empty == "Destroy"))
					{
						AkTriggerBase akTriggerBase = (AkTriggerBase)base.GetComponent(UtilityPlugin.GetType(empty));
						if (akTriggerBase == null)
						{
							akTriggerBase = (AkTriggerBase)base.gameObject.AddComponent(UtilityPlugin.GetType(empty));
						}
						AkTriggerBase expr_97 = akTriggerBase;
						expr_97.triggerDelegate = (AkTriggerBase.Trigger)Delegate.Combine(expr_97.triggerDelegate, in_delegate);
					}
				}
			}
		}
	}

	protected void UnregisterTriggers(List<int> in_triggerList, AkTriggerBase.Trigger in_delegate)
	{
		using (List<int>.Enumerator enumerator = in_triggerList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				uint current = (uint)enumerator.Current;
				string empty = string.Empty;
				if (AkUnityEventHandler.triggerTypes.TryGetValue(current, out empty))
				{
					if (!(empty == "Awake") && !(empty == "Start") && !(empty == "Destroy"))
					{
						AkTriggerBase akTriggerBase = (AkTriggerBase)base.GetComponent(UtilityPlugin.GetType(empty));
						if (akTriggerBase != null)
						{
							AkTriggerBase expr_80 = akTriggerBase;
							expr_80.triggerDelegate = (AkTriggerBase.Trigger)Delegate.Remove(expr_80.triggerDelegate, in_delegate);
							if (akTriggerBase.triggerDelegate == null)
							{
								UnityEngine.Object.Destroy(akTriggerBase);
							}
						}
					}
				}
			}
		}
	}
}
