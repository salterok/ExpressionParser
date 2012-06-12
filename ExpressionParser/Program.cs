using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ExpressionParser
{
	class Program
	{
		static void Main(string[] args)
		{			
			var parser = new Parser();
			parser.AddExternLib("System.Math.xml");

			var str = Console.ReadLine();
            var t = parser.Parse(str);

			Console.Clear();
			Console.WriteLine("{0} = {1}", str, t.Value);
			Console.ReadKey();
		}
	}
}
