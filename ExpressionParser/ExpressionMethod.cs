using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
	class ExpressionMethod : ExpressionExternMember
	{
		public ExpressionNode[] Params { get; private set; }

		public ExpressionMethod(Aggregation _aggr, string name, bool isStatic, params ExpressionNode[] nodes)
			: base(_aggr, name, isStatic)
		{
			Params = nodes;
		}

		public override double Value
		{
			get
			{
				throw new NotImplementedException();
			}
			protected set
			{
				base.Value = value;
			}
		}
	}
}
