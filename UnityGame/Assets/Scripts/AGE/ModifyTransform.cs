using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Movement")]
	public class ModifyTransform : TickEvent
	{
		public bool enableTranslation = true;

		public bool currentTranslation;

		public Vector3 translation = Vector3.zero;

		public bool enableRotation = true;

		public bool currentRotation;

		public Quaternion rotation = Quaternion.identity;

		public bool enableScaling;

		public bool currentScaling;

		public Vector3 scaling = Vector3.one;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int objectSpaceId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int fromId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int toId = -1;

		public bool normalizedRelative;

		public static Vector3 axisWeight = new Vector3(1f, 0f, 1f);

		public bool cubic;

		private bool currentInitialized;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			ModifyTransform modifyTransform = ClassObjPool<ModifyTransform>.Get();
			modifyTransform.CopyData(this);
			return modifyTransform;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ModifyTransform modifyTransform = src as ModifyTransform;
			this.enableTranslation = modifyTransform.enableTranslation;
			this.currentTranslation = modifyTransform.currentTranslation;
			this.translation = modifyTransform.translation;
			this.enableRotation = modifyTransform.enableRotation;
			this.currentRotation = modifyTransform.currentRotation;
			this.rotation = modifyTransform.rotation;
			this.enableScaling = modifyTransform.enableScaling;
			this.currentScaling = modifyTransform.currentScaling;
			this.scaling = modifyTransform.scaling;
			this.targetId = modifyTransform.targetId;
			this.objectSpaceId = modifyTransform.objectSpaceId;
			this.fromId = modifyTransform.fromId;
			this.toId = modifyTransform.toId;
			this.normalizedRelative = modifyTransform.normalizedRelative;
			this.cubic = modifyTransform.cubic;
			this.currentInitialized = modifyTransform.currentInitialized;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.enableTranslation = true;
			this.currentTranslation = false;
			this.translation = Vector3.zero;
			this.enableRotation = true;
			this.currentRotation = false;
			this.rotation = Quaternion.identity;
			this.enableScaling = false;
			this.currentScaling = false;
			this.scaling = Vector3.one;
			this.targetId = 0;
			this.objectSpaceId = -1;
			this.fromId = -1;
			this.toId = -1;
			this.normalizedRelative = false;
			this.cubic = false;
			this.currentInitialized = false;
		}

		public override void Process(Action _action, Track _track)
		{
			if (_action.GetGameObject(this.targetId) == null)
			{
				return;
			}
			this.currentInitialized = false;
			this.SetCurrentTransform(_action.GetGameObject(this.targetId).transform);
			if (this.enableTranslation)
			{
				_action.GetGameObject(this.targetId).transform.position = this.GetTranslation(_action);
			}
			if (this.enableRotation)
			{
				_action.GetGameObject(this.targetId).transform.rotation = this.GetRotation(_action);
			}
			if (this.enableScaling)
			{
				_action.GetGameObject(this.targetId).transform.localScale = this.scaling;
			}
		}

		public void CubicVectorBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight, bool isPos)
		{
			int indexOfEvent = _track.GetIndexOfEvent(_prevEvent);
			int indexOfEvent2 = _track.GetIndexOfEvent(this);
			int eventsCount = _track.GetEventsCount();
			int num = indexOfEvent - 1;
			if (num < 0)
			{
				if (_action.loop)
				{
					num = eventsCount - 1;
					if (num < 0)
					{
						num = 0;
					}
				}
				else
				{
					num = 0;
				}
			}
			int num2 = indexOfEvent2 + 1;
			if (num2 >= eventsCount)
			{
				if (_action.loop)
				{
					num2 = 0;
				}
				else
				{
					num2 = indexOfEvent2;
				}
			}
			ModifyTransform modifyTransform = _prevEvent as ModifyTransform;
			ModifyTransform modifyTransform2 = _track.GetEvent(num) as ModifyTransform;
			ModifyTransform modifyTransform3 = _track.GetEvent(num2) as ModifyTransform;
			DebugHelper.Assert(modifyTransform != null && modifyTransform2 != null && modifyTransform3 != null, "��һ�Ѷ���Ӧ��Ϊ��");
			Vector3 vector = isPos ? modifyTransform.GetTranslation(_action) : modifyTransform.scaling;
			Vector3 vector2 = isPos ? this.GetTranslation(_action) : this.scaling;
			Vector3 formPoint = isPos ? modifyTransform2.GetTranslation(_action) : modifyTransform2.scaling;
			Vector3 lattPoint = isPos ? modifyTransform3.GetTranslation(_action) : modifyTransform3.scaling;
			Vector3 a;
			Vector3 a2;
			CurvlData.CalculateCtrlPoint(formPoint, vector, vector2, lattPoint, out a, out a2);
			float d = 1f - _blendWeight;
			Vector3 vector3 = vector * d * d * d + a * 3f * d * d * _blendWeight + a2 * 3f * d * _blendWeight * _blendWeight + vector2 * _blendWeight * _blendWeight * _blendWeight;
			if (isPos)
			{
				_action.GetGameObject(this.targetId).transform.position = vector3;
			}
			else
			{
				_action.GetGameObject(this.targetId).transform.localScale = vector3;
			}
		}

		public override void ProcessBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
		{
			if (_action.GetGameObject(this.targetId) == null || _prevEvent == null)
			{
				return;
			}
			if (this.enableTranslation)
			{
				if (this.cubic)
				{
					this.CubicVectorBlend(_action, _track, _prevEvent, _blendWeight, true);
				}
				else
				{
					_action.GetGameObject(this.targetId).transform.position = this.GetTranslation(_action) * _blendWeight + (_prevEvent as ModifyTransform).GetTranslation(_action) * (1f - _blendWeight);
				}
			}
			if (this.enableRotation)
			{
				_action.GetGameObject(this.targetId).transform.rotation = Quaternion.Slerp((_prevEvent as ModifyTransform).GetRotation(_action), this.GetRotation(_action), _blendWeight);
			}
			if (this.enableScaling)
			{
				if (this.cubic)
				{
					this.CubicVectorBlend(_action, _track, _prevEvent, _blendWeight, false);
				}
				else
				{
					_action.GetGameObject(this.targetId).transform.localScale = this.scaling * _blendWeight + (_prevEvent as ModifyTransform).scaling * (1f - _blendWeight);
				}
			}
		}

		public bool HasTempObj(Action _action)
		{
			return (this.fromId >= 0 && _action.GetGameObject(this.fromId) == null) || (this.toId >= 0 && _action.GetGameObject(this.toId) == null) || (this.objectSpaceId >= 0 && _action.GetGameObject(this.objectSpaceId) == null);
		}

		public int HasDependObject(Action _action)
		{
			if (this.currentTranslation || this.currentRotation || this.currentScaling)
			{
				return 1;
			}
			if (this.fromId >= 0)
			{
				if (_action.GetGameObject(this.fromId) != null)
				{
					return 1;
				}
				return -1;
			}
			else if (this.toId >= 0)
			{
				if (_action.GetGameObject(this.toId) != null)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (this.objectSpaceId < 0)
				{
					return 0;
				}
				if (_action.GetGameObject(this.objectSpaceId) != null)
				{
					return 1;
				}
				return -1;
			}
		}

		public Vector3 GetTranslation(Action _action)
		{
			if (_action.GetGameObject(this.targetId))
			{
				this.SetCurrentTransform(_action.GetGameObject(this.targetId).transform);
			}
			GameObject gameObject = _action.GetGameObject(this.fromId);
			GameObject gameObject2 = _action.GetGameObject(this.toId);
			if (gameObject && gameObject2)
			{
				Vector3 vector = default(Vector3);
				Vector3 forward = gameObject2.transform.position - gameObject.transform.position;
				Vector2 vector2 = new Vector2(forward.x, forward.z);
				float magnitude = vector2.magnitude;
				forward = Vector3.Normalize(new Vector3(forward.x * ModifyTransform.axisWeight.x, forward.y * ModifyTransform.axisWeight.y, forward.z * ModifyTransform.axisWeight.z));
				Quaternion quaternion = Quaternion.LookRotation(forward, Vector3.up);
				Vector3 vector3;
				if (this.normalizedRelative)
				{
					vector3 = quaternion * this.translation;
					vector3 = gameObject.transform.position + new Vector3(vector3.x * magnitude, vector3.y, vector3.z * magnitude);
					vector3 += new Vector3(0f, this.translation.z * (gameObject2.transform.position.y - gameObject.transform.position.y), 0f);
				}
				else
				{
					vector3 = gameObject.transform.position + quaternion * this.translation;
					vector3 += new Vector3(0f, this.translation.z / magnitude * (gameObject2.transform.position.y - gameObject.transform.position.y), 0f);
				}
				return vector3;
			}
			GameObject gameObject3 = _action.GetGameObject(this.objectSpaceId);
			if (gameObject3)
			{
				return gameObject3.transform.localToWorldMatrix.MultiplyPoint(this.translation);
			}
			GameObject gameObject4 = _action.GetGameObject(this.targetId);
			if (gameObject4 && gameObject4.transform.parent)
			{
				return gameObject4.transform.parent.localToWorldMatrix.MultiplyPoint(this.translation);
			}
			return this.translation;
		}

		public Quaternion GetRotation(Action _action)
		{
			if (_action.GetGameObject(this.targetId))
			{
				this.SetCurrentTransform(_action.GetGameObject(this.targetId).transform);
			}
			GameObject gameObject = _action.GetGameObject(this.fromId);
			GameObject gameObject2 = _action.GetGameObject(this.toId);
			if (gameObject && gameObject2)
			{
				Vector3 forward = gameObject2.transform.position - gameObject.transform.position;
				forward = Vector3.Normalize(new Vector3(forward.x * ModifyTransform.axisWeight.x, forward.y * ModifyTransform.axisWeight.y, forward.z * ModifyTransform.axisWeight.z));
				Quaternion lhs = Quaternion.LookRotation(forward, Vector3.up);
				return lhs * this.rotation;
			}
			GameObject gameObject3 = _action.GetGameObject(this.objectSpaceId);
			if (gameObject3)
			{
				return gameObject3.transform.rotation * this.rotation;
			}
			GameObject gameObject4 = _action.GetGameObject(this.targetId);
			if (gameObject4 && gameObject4.transform.parent)
			{
				return gameObject4.transform.parent.rotation * this.rotation;
			}
			return this.rotation;
		}

		private void SetCurrentTransform(Transform _transform)
		{
			if (this.currentInitialized)
			{
				return;
			}
			if (this.currentTranslation)
			{
				this.objectSpaceId = (this.fromId = (this.toId = -1));
				this.translation = _transform.localPosition;
			}
			if (this.currentRotation)
			{
				this.objectSpaceId = (this.fromId = (this.toId = -1));
				this.rotation = _transform.localRotation;
			}
			if (this.currentScaling)
			{
				this.scaling = _transform.localScale;
			}
			this.currentInitialized = true;
		}
	}
}
