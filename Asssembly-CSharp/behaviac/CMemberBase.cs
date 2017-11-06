using System;
using System.Reflection;

namespace behaviac
{
	public class CMemberBase
	{
		private FieldInfo field_;

		private float m_range = 1f;

		private CStringID m_id = default(CStringID);

		private string m_instaceName;

		public Type MemberType
		{
			get
			{
				return this.field_.get_FieldType();
			}
		}

		public CMemberBase(FieldInfo f, MemberMetaInfoAttribute a)
		{
			this.field_ = f;
			this.m_id.SetId(this.field_.get_Name());
			if (a != null)
			{
				this.m_range = a.Range;
			}
			else
			{
				this.m_range = 1f;
			}
		}

		public bool ISSTATIC()
		{
			return this.field_.get_IsStatic();
		}

		public float GetRange()
		{
			return this.m_range;
		}

		public ParentType GetParentType()
		{
			return ParentType.PT_INVALID;
		}

		public CStringID GetId()
		{
			return this.m_id;
		}

		public string GetName()
		{
			return this.field_.get_Name();
		}

		public string GetClassNameString()
		{
			return this.field_.get_DeclaringType().get_FullName();
		}

		public string GetInstanceNameString()
		{
			return this.m_instaceName;
		}

		public void SetInstanceNameString(string name)
		{
			this.m_instaceName = name;
		}

		public virtual Property CreateProperty(string defaultValue, bool bConst)
		{
			Property property = new Property(this, bConst);
			if (!string.IsNullOrEmpty(defaultValue))
			{
				property.SetDefaultValue(defaultValue);
			}
			return property;
		}

		public virtual int GetTypeId()
		{
			return 0;
		}

		public virtual CMemberBase clone()
		{
			return null;
		}

		public virtual void Load(Agent parent, ISerializableNode node)
		{
		}

		public virtual void Save(Agent parent, ISerializableNode node)
		{
		}

		public virtual object Get(object agentFrom)
		{
			if (this.ISSTATIC())
			{
				return this.field_.GetValue(null);
			}
			return this.field_.GetValue(agentFrom);
		}

		public void Set(object objectFrom, object v)
		{
			this.field_.SetValue(objectFrom, v);
		}
	}
}
