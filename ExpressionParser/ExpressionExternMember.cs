using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
	class ExpressionExternMember : ExpressionNode
	{
		public string Name { get; protected set; }
		public bool IsStatic { get; protected set; }
		protected Aggregation aggregation;
		public ExpressionExternMember(Aggregation _aggr, string name, bool isStatic)
			: base(null, null)
		{
			aggregation = _aggr;
			Name = name;
			IsStatic = isStatic;
		}
	}
}
