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
		[Priority(1)]
		[OperationType(OType.Separator)]
		[Description(",")]
		Comma,
		[Priority(2)]
		[OperationType(OType.Arithmetic)]
		[Description("+")]
		Addition,
		[Priority(2)]
		[OperationType(OType.Arithmetic)]
		[Description("-")]
		Substraction,
		[Priority(3)]
		[OperationType(OType.Arithmetic)]
		[Description("*")]
		Multiply,
		[Priority(3)]
		[OperationType(OType.Arithmetic)]
		[Description("/")]
		Divide,
		[Priority(0)]
		[OperationType(OType.Separator)]
		[Description("(")]
		OpenBracket,
		[Priority(1)]
		[OperationType(OType.Separator)]
		[Description(")")]
		CloseBracket,
		[Priority(5)]
		[OperationType(OType.Arithmetic)]
		[Description("==")]
		Equal,
		[Priority(4)]
		[OperationType(OType.Arithmetic)]
		[Description("=")]
		Assignment,
		[Priority(1)]
		[OperationType(OType.Method)]
		[Description("{Method}")]
		Method
	}
}
