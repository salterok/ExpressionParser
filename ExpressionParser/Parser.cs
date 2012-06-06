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
        char[] operations;
		Aggregation aggregation = new Aggregation();

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

		public void AddExternLib(string filename)
		{
			// temporary for testing
			// TODO: more smart adding new aggregation
			aggregation.Load(filename);
		}

        public ExpressionNode Parse(string expression)
        {
            List<char> list = new List<char>();
            ExpressionNode node = new ExpressionNode();
            ExpressionNode root = node;

			var type = Type.GetType("System.Math");

			string Name = "Pow";
			var Params = new object[] { 2.0, 3.0 };
			var ert = (double)type.InvokeMember(Name, BindingFlags.InvokeMethod, null, null, Params);

			var e = type.InvokeMember("Pow", BindingFlags.InvokeMethod, null, null, new object[] { 2, 3 });
			var pi = type.InvokeMember("PI", BindingFlags.GetField, null, null, null);
			

            // (a+b)*(c-d)-e
            // (-3*6)+5
            // -4+5/2
            // temp*45-angel/(2*pi)

            // 6+5-(3-4*8)

            var tt = foo(expression);
            var er = tt.Value;
            throw new NotImplementedException();
        }

        private ExpressionNode foo(string expression)
        {
            var stack = new Stack<ExpressionNode>();
            var operations = new Stack<char>();
            var buffer = new List<char>();

            #region nested actions

			var action = new Action<bool>(
				(state) =>
				{
					if (buffer.Count == 0)
					{
						return;
					}
					string source = String.Join(String.Empty, buffer.ToArray());
					float value;
					ExpressionNode expr = null;
					if (float.TryParse(source, out value))
					{
						expr = new ExpressionValue(value);
					}
					else if (state)
					{
						// check for aggregate function
						var method = aggregation.GetMethod(source);
						if (method.HasValue)
						{
							return;
							//var _params = new List<ExpressionNode>();
							//for (int i = 0; i < method.Value.Params.Length; i++)
							//{
							//	_params.Add(stack.Pop());
							//}
							//_params.Reverse();
							//expr = new ExpressionMethod(aggregation, method.Value.Name, method.Value.IsStatic, _params.ToArray());
						}
					}
					else
					{
						var constant = aggregation.GetConstant(source);
						if (constant.HasValue)
						{
							// [source] is defined constant
							expr = new ExpressionExternConstant(aggregation, constant.Value.Name, constant.Value.IsStatic);
						}
						else
						{
							// [source] is variable name
							expr = new ExpressionVariable(source);
						}
					}
					if (expr != null)
					{
						stack.Push(expr);
					}
					buffer.Clear();
				}
			);

			var createNode = new Action<char>(
				(c) =>
				{
					if (IsOperation(c) || c == '(')
					{
						ExpressionNode expr = null;
						if (buffer.Count > 0)
						{
							buffer.Reverse();
							var method = aggregation.GetMethod(String.Join(String.Empty, buffer));
							if (method.HasValue)
							{
								var _params = new List<ExpressionNode>();
								for (int i = 0; i < method.Value.Params.Length; i++)
								{
									_params.Add(stack.Pop());
								}
								_params.Reverse();
								expr = new ExpressionExternMethod(aggregation, method.Value.Name, method.Value.IsStatic, _params.ToArray());
							}
							buffer.Clear();
						}
						else if (c != '(')
						{
							var tempOperation = GetOperation(c);
							var _r = stack.Pop();
							var _l = stack.Pop();
							expr = new ExpressionOperation(tempOperation ?? Operation.Unknown, _l, _r);
						}
						if (expr != null)
						{
							stack.Push(expr);
						}
					}
					else
					{
						buffer.Add(c);
					}
				}
			);

            #endregion


            foreach (var c in expression)
            {
                if (c == '(')
                {
					action(true);
					//
					operations.Push(c);
					foreach (var _c in String.Join(String.Empty, buffer.ToArray()))
					{
						operations.Push(_c);
					}
					buffer.Clear();
                }
                else if (IsOperation(c))
                {
					action(false);
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
					action(false);
					while (operations.Count > 0)
                    {
                        char temp = operations.Pop();
						createNode(temp);
						if (temp == '(')
						{
							break;
						}
                    }
                }
				else if (c == ',')
				{
					action(false);
				}
				else
				{
					buffer.Add(c);
				}
            }
			action(false);
            while (operations.Count > 0)
            {
				createNode(operations.Pop());
            }
            return stack.Pop();
        }

        private bool IsOperation(char c)
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

        private Operation? GetOperation(char c)
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
