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
			var str = Console.ReadLine();
			var parser = new Parser();
			parser.AddExternLib("System.Math.xml");
            var t = parser.Parse(str);

			// 3-6*7-4/(2+5*0.4)
			Console.Clear();
			Console.WriteLine("{0} = {1}", str, t.Value);
			Console.ReadKey();
		}
	}
}
