using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParser
{
	class Program
	{
		static void Main(string[] args)
		{
            var str = Console.ReadLine();
            var t = Parser.Parse(str);
		}
	}
}
