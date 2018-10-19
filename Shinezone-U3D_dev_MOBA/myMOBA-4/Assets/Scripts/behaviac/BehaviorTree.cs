using Mono.Xml;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace behaviac
{
	public class BehaviorTree : BehaviorNode
	{
		public class Descriptor_t
		{
			public Property Descriptor;

			public Property Reference;

			public Descriptor_t()
			{
			}

			public Descriptor_t(BehaviorTree.Descriptor_t copy)
			{
				this.Descriptor = ((copy.Descriptor == null) ? null : copy.Descriptor.clone());
				this.Reference = ((copy.Reference == null) ? null : copy.Reference.clone());
			}

			~Descriptor_t()
			{
				this.Descriptor = null;
				this.Reference = null;
			}
		}

		protected string m_name;

		protected string m_domains;

		private List<BehaviorTree.Descriptor_t> m_descriptorRefs;

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			if (properties.Count > 0)
			{
				foreach (property_t current in properties)
				{
					if (current.name == "Domains")
					{
						this.m_domains = current.value;
					}
					else if (current.name == "DescriptorRefs")
					{
						this.m_descriptorRefs = (List<BehaviorTree.Descriptor_t>)StringUtils.FromString(typeof(List<BehaviorTree.Descriptor_t>), current.value, false);
						if (this.m_descriptorRefs != null)
						{
							for (int i = 0; i < this.m_descriptorRefs.Count; i++)
							{
								BehaviorTree.Descriptor_t descriptor_t = this.m_descriptorRefs[i];
								if (descriptor_t.Descriptor != null)
								{
									descriptor_t.Descriptor.SetDefaultValue(descriptor_t.Reference);
								}
							}
						}
					}
				}
			}
		}

		public bool load_xml(byte[] pBuffer)
		{
			try
			{
				string @string = Encoding.UTF8.GetString(pBuffer);
				SecurityParser securityParser = new SecurityParser();
				securityParser.LoadXml(@string);
				SecurityElement securityElement = securityParser.ToXml();
				bool result;
				if (securityElement.Tag != "behavior" && (securityElement.Children == null || securityElement.Children.Count != 1))
				{
					result = false;
					return result;
				}
				this.m_name = securityElement.Attribute("name");
				string agentType = securityElement.Attribute("agenttype");
				string s = securityElement.Attribute("version");
				int version = int.Parse(s);
				base.SetClassNameString("BehaviorTree");
				base.SetId(-1);
				base.load_properties_pars_attachments_children(true, version, agentType, securityElement);
				result = true;
				return result;
			}
			catch (Exception var_6_B6)
			{
			}
			return false;
		}

		public static void Cleanup()
		{
			Workspace.UnLoadAll();
		}

		public string GetName()
		{
			return this.m_name;
		}

		public void SetName(string name)
		{
			this.m_name = name;
		}

		~BehaviorTree()
		{
			if (this.m_descriptorRefs != null)
			{
				this.m_descriptorRefs.Clear();
			}
		}

		protected override BehaviorTask createTask()
		{
			return new BehaviorTreeTask();
		}

		public string GetDomains()
		{
			return this.m_domains;
		}

		public void SetDomains(string domains)
		{
			this.m_domains = domains;
		}

		public List<BehaviorTree.Descriptor_t> GetDescriptors()
		{
			return this.m_descriptorRefs;
		}

		public void SetDescriptors(string descriptors)
		{
			this.m_descriptorRefs = (List<BehaviorTree.Descriptor_t>)StringUtils.FromString(typeof(List<BehaviorTree.Descriptor_t>), descriptors, false);
			if (this.m_descriptorRefs != null)
			{
				for (int i = 0; i < this.m_descriptorRefs.Count; i++)
				{
					BehaviorTree.Descriptor_t descriptor_t = this.m_descriptorRefs[i];
					descriptor_t.Descriptor.SetDefaultValue(descriptor_t.Reference);
				}
			}
		}
	}
}
