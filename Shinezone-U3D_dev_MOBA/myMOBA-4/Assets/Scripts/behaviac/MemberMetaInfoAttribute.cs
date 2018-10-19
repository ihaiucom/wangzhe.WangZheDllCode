using System;
using System.Reflection;

namespace behaviac
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class MemberMetaInfoAttribute : TypeMetaInfoAttribute
	{
		private float m_range;

		public float Range
		{
			get
			{
				return this.m_range;
			}
		}

		public MemberMetaInfoAttribute(string displayName, string description) : this(displayName, description, 1f)
		{
		}

		public MemberMetaInfoAttribute(string displayName, string description, float range) : base(displayName, description)
		{
			this.m_range = 1f;
			this.m_range = range;
		}

		public MemberMetaInfoAttribute()
		{
			this.m_range = 1f;
		}

		private static string getEnumName(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			Type type = obj.GetType();
			if (!type.IsEnum)
			{
				return string.Empty;
			}
			string name = Enum.GetName(type, obj);
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			return name;
		}

		public static string GetEnumDisplayName(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			string result = MemberMetaInfoAttribute.getEnumName(obj);
			FieldInfo field = obj.GetType().GetField(obj.ToString());
			Attribute[] array = (Attribute[])field.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
			if (array.Length > 0)
			{
				result = ((MemberMetaInfoAttribute)array[0]).DisplayName;
			}
			return result;
		}

		public static string GetEnumDescription(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			string result = MemberMetaInfoAttribute.getEnumName(obj);
			FieldInfo field = obj.GetType().GetField(obj.ToString());
			Attribute[] array = (Attribute[])field.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
			if (array.Length > 0)
			{
				result = ((MemberMetaInfoAttribute)array[0]).Description;
			}
			return result;
		}
	}
}
