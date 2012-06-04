using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ExpressionParser
{
	public enum Operation
	{
		Unknown,
        [Description("+")]
		Addition,
		[Description("-")]
		Substraction,
		[Description("*")]
		Multiply,
		[Description("/")]
        Divide
	}
}
