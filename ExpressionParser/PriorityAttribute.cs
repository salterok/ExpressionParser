using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
	public class PriorityAttribute : Attribute
	{
		public readonly int Priority;
		public PriorityAttribute(int priority)
		{
			Priority = priority;
		}
	}
}
