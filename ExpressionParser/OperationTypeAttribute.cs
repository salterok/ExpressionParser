using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
	public class OperationTypeAttribute : Attribute
	{
		public readonly OType Type;
		public OperationTypeAttribute(OType type)
		{
			Type = type;
		}
	}
}
