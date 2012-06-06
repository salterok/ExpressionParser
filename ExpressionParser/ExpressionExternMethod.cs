using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ExpressionParser
{
	class ExpressionExternMethod : ExpressionExternMember
	{
		public ExpressionNode[] Params { get; private set; }

		public ExpressionExternMethod(Aggregation _aggr, string name, bool isStatic, params ExpressionNode[] nodes)
			: base(_aggr, name, isStatic)
		{
			Params = nodes;
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
					var type = Type.GetType(result.First().type);
					if (IsStatic)
					{
						return (double)type.InvokeMember(Name, BindingFlags.InvokeMethod, null, null,
							Params.Select(param => param.Value).Cast<object>().ToArray());
					}
					else
					{
						throw new NotImplementedException();
					}
				}
				catch (System.Exception ex)
				{
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
