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
			

            // (a+b)*(c-d)-e
            // (-3*6)+5
            // -4+5/2
            // temp*45-angel/(2*pi)

            // 6+5-(3-4*8)

            var result = foo(expression);

            //throw new NotImplementedException();
			return result;
        }

        private ExpressionNode foo(string expression)
        {
            var stack = new Stack<ExpressionNode>();
            var operations = new Stack<char>();
            var buffer = new List<char>();
			var methods = new Stack<string>();

            #region nested actions

			var action = new Action<bool>(
				(state) =>
				{
					if (buffer.Count == 0)
					{
						return;
					}
					string source = String.Join(String.Empty, buffer.ToArray());
					double value;
					ExpressionNode expr = null;
					if (double.TryParse(source, out value))
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
					ExpressionNode expr = null;
					if (IsOperation(c))
					{
						var tempOperation = GetOperation(c);
						var _r = stack.Pop();
						var _l = stack.Pop();
						expr = new ExpressionOperation(tempOperation ?? Operation.Unknown, _l, _r);
					}
					else if (c == '(')
					{
						if (methods.Count > 0)
						{
							var m_name = methods.Pop();
							var method = aggregation.GetMethod(m_name);
							if (method.HasValue)
							{
								var _params = new List<ExpressionNode>();
								for (int i = 0; i < method.Value.Params.Length; i++)
								{
									_params.Add(stack.Pop());
								}
								_params.Reverse();
								expr = new ExpressionExternMethod(aggregation, method.Value.Name, method.Value.IsStatic, 
									_params.ToArray());
							}
						}

					}
					if (expr != null)
					{
						stack.Push(expr);
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
					if (buffer.Count > 0)
					{
						methods.Push(String.Join(String.Empty, buffer.ToArray()));
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
