using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ExpressionParser
{
	class ExpressionExternConstant : ExpressionExternMember
	{
		public ExpressionExternConstant(Aggregation _aggr, string name)
			: base(_aggr, name, true)
		{

		}

		public override double Value
		{
			get
			{
				var result = aggregation.GetFullName(Name);
				if (result.Count() > 1)
				{
					// TODO: create new exception class which would contain info about result (what cases exist and info about each)
					// like Ambiguity between * and *
					throw new Exception("more than one match");
				}
				if (result.Count() == 0)
				{
					throw new Exception("no match");
				}
				try
				{
					Type type = Type.GetType(result.First().type);
					return (double)type.InvokeMember(result.First().member, BindingFlags.GetField, null, null, null);
				}
				catch (System.Exception ex)
				{
					// add normal throwing (more useful info)
					throw ex;
				}
			}
			protected set
			{
				base.Value = value;
			}
		}
	}
}
