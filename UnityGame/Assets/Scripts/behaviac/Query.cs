using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Query : BehaviorNode
	{
		private class Descriptor_t
		{
			public Property Attribute;

			public Property Reference;

			public float Weight;

			public Descriptor_t()
			{
				this.Attribute = null;
				this.Reference = null;
				this.Weight = 0f;
			}

			public Descriptor_t(Query.Descriptor_t copy)
			{
				this.Attribute = copy.Attribute.clone();
				this.Reference = copy.Reference.clone();
				this.Weight = copy.Weight;
			}

			~Descriptor_t()
			{
				this.Attribute = null;
				this.Reference = null;
			}
		}

		private class QueryTask : SingeChildTask
		{
			public override void Init(BehaviorNode node)
			{
				base.Init(node);
			}

			~QueryTask()
			{
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			protected override bool isContinueTicking()
			{
				return true;
			}

			protected override bool onenter(Agent pAgent)
			{
				return this.ReQuery(pAgent);
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			public override bool CheckPredicates(Agent pAgent)
			{
				bool flag = false;
				if (this.m_attachments != null)
				{
					flag = base.CheckPredicates(pAgent);
				}
				if (flag)
				{
					this.ReQuery(pAgent);
				}
				return flag;
			}

			private bool ReQuery(Agent pAgent)
			{
				Query query = base.GetNode() as Query;
				if (query != null)
				{
					List<Query.Descriptor_t> descriptors = query.GetDescriptors();
					if (descriptors.get_Count() > 0)
					{
						DictionaryView<string, BehaviorTree> behaviorTrees = Workspace.GetBehaviorTrees();
						BehaviorTree behaviorTree = null;
						float num = -1f;
						foreach (KeyValuePair<string, BehaviorTree> current in behaviorTrees)
						{
							BehaviorTree value = current.get_Value();
							string domains = value.GetDomains();
							if (string.IsNullOrEmpty(query.m_domain) || (!string.IsNullOrEmpty(domains) && domains.IndexOf(query.m_domain) != -1))
							{
								List<BehaviorTree.Descriptor_t> descriptors2 = value.GetDescriptors();
								float num2 = query.ComputeSimilarity(descriptors, descriptors2);
								if (num2 > num)
								{
									num = num2;
									behaviorTree = value;
								}
							}
						}
						if (behaviorTree != null)
						{
							pAgent.btreferencetree(behaviorTree.GetName());
							return true;
						}
					}
				}
				return false;
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				return EBTStatus.BT_RUNNING;
			}
		}

		protected string m_domain;

		private List<Query.Descriptor_t> m_descriptors;

		~Query()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			if (properties.get_Count() > 0)
			{
				using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						property_t current = enumerator.get_Current();
						if (current.name == "Domain")
						{
							this.m_domain = current.value;
						}
						else if (current.name == "Descriptors")
						{
							this.SetDescriptors(current.value);
						}
					}
				}
			}
		}

		protected override BehaviorTask createTask()
		{
			return new Query.QueryTask();
		}

		private static Property FindProperty(Query.Descriptor_t q, List<BehaviorTree.Descriptor_t> c)
		{
			for (int i = 0; i < c.get_Count(); i++)
			{
				BehaviorTree.Descriptor_t descriptor_t = c.get_Item(i);
				if (descriptor_t.Descriptor.GetVariableId() == q.Attribute.GetVariableId())
				{
					return descriptor_t.Descriptor;
				}
			}
			return null;
		}

		private List<Query.Descriptor_t> GetDescriptors()
		{
			return this.m_descriptors;
		}

		protected void SetDescriptors(string descriptors)
		{
			this.m_descriptors = (List<Query.Descriptor_t>)StringUtils.FromString(typeof(List<Query.Descriptor_t>), descriptors, false);
			for (int i = 0; i < this.m_descriptors.get_Count(); i++)
			{
				Query.Descriptor_t descriptor_t = this.m_descriptors.get_Item(i);
				descriptor_t.Attribute.SetDefaultValue(descriptor_t.Reference);
			}
		}

		private float ComputeSimilarity(List<Query.Descriptor_t> q, List<BehaviorTree.Descriptor_t> c)
		{
			float num = 0f;
			for (int i = 0; i < q.get_Count(); i++)
			{
				Query.Descriptor_t descriptor_t = q.get_Item(i);
				Property property = Query.FindProperty(descriptor_t, c);
				if (property != null)
				{
					float num2 = descriptor_t.Attribute.DifferencePercentage(property);
					num += (1f - num2) * descriptor_t.Weight;
				}
			}
			return num;
		}
	}
}
