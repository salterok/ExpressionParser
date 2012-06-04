using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ExpressionParser
{
    class Parser
    {
        public static Dictionary<int, char[]> priority = new Dictionary<int, char[]>();
        static char[] operations;

        static Parser()
        {
            priority.Add(0, new char[]
			{
				'('
			});
            priority.Add(1, new char[]
			{
				')'
			});
            priority.Add(2, new char[]
			{
				'+',
                '-'
			});
            priority.Add(3, new char[]
			{
				'*',
                '/'
			});
        }

        public static ExpressionNode Parse(string expression)
        {
            List<char> list = new List<char>();
            ExpressionNode node = new ExpressionNode();
            ExpressionNode root = node;

            // (a+b)*(c-d)-e
            // (-3*6)+5
            // -4+5/2
            // temp*45-angel/(2*pi)

            // 6+5-(3-4*8)

            var tt = foo(expression);
            var er = tt.Value;
            throw new NotImplementedException();
        }

        private static ExpressionNode foo(string expression)
        {
            var stack = new Stack<ExpressionNode>();
            var operations = new Stack<char>();
            var buffer = new List<char>();

            #region nested action

			var action = new Action(
				() =>
				{
					if (buffer.Count == 0)
					{
						return;
					}
					string source = String.Join(String.Empty, buffer.ToArray());
					float value;
					ExpressionNode expr;
					if (float.TryParse(source, out value))
					{
						expr = new ExpressionValue(value);
					}
					else if (false)
					{
						// check for keyword
					}
					else
					{
						// [source] is variable name
						expr = new ExpressionVariable(source);
					}
					stack.Push(expr);
					buffer.Clear();
				}
			);

			var createNode = new Action<char>(
				(c) =>
				{
					var tempOperation = GetOperation(c);
					var _r = stack.Pop();
					var _l = stack.Pop();
					var expr = new ExpressionOperation(tempOperation ?? Operation.Unknown, _l, _r);
					stack.Push(expr);
				}
			);

            #endregion


            foreach (var c in expression)
            {
                if (c == '(')
                {
					action();
					operations.Push(c);
                }
                else if (IsOperation(c))
                {
					action();
					if (operations.Count == 0)
                    {
                        operations.Push(c);
                    }
                    else
                    {
                        while (operations.Count != 0 && 
							priority.First(item => item.Value.Contains(operations.Peek())).Key >= 
							priority.First(item => item.Value.Contains(c)).Key)
                        {
							createNode(operations.Pop());
                        }
                        operations.Push(c);
                    }
                }
                else if (c == ')')
                {
					action();
					while (operations.Count > 0)
                    {
                        char temp = operations.Pop();
                        if (temp != '(')
                        {
							createNode(temp);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    buffer.Add(c);
                }
            }
			action();
            while (operations.Count > 0)
            {
				createNode(operations.Pop());
            }
            return stack.Pop();
        }

        private static bool IsOperation(char c)
        {
            if (operations == null)
            {
                var temp = new List<char>();
                Type type = typeof(Operation);
                foreach (var name in Enum.GetNames(type))
                {
                    var item = type.GetMember(name).First();
                    var attribute = item.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
                    if (attribute == default(System.ComponentModel.DescriptionAttribute))
                    {
                        continue;
                    }
                    temp.Add((attribute as System.ComponentModel.DescriptionAttribute).Description.First());
                }
                operations = temp.ToArray();
            }
            return operations.Contains(c);
        }

        private static Operation? GetOperation(char c)
        {
            Type type = typeof(Operation);
            foreach (var item in type.GetFields())
            {
                var attribute = item.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
                if (attribute == default(System.ComponentModel.DescriptionAttribute))
                {
                    continue;
                }
                if ((attribute as System.ComponentModel.DescriptionAttribute).Description.First() == c)
                {
                    return (Operation)Enum.Parse(type, item.Name);
                }
            }
            return null;
        }
    }
}
