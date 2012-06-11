using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

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
			var operations = new Stack<CompleteOperation>();
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
			#endregion

			foreach (var c in expression)
			{
				var current = new CompleteOperation(c);
				if (current.OriginalOperation == Operation.OpenBracket)
				{
					action(true);
					operations.Push(current);
					if (buffer.Count > 0)
					{
						operations.Push(new CompleteOperation(String.Join(String.Empty, buffer.ToArray())));
					}
					buffer.Clear();
				}
				else if (current.Type == OType.Arithmetic)
				{
					action(false);
					if (operations.Count == 0)
					{
						operations.Push(new CompleteOperation(c));
					}
					else
					{
						var operation = new CompleteOperation(c);
						while (operations.Count > 0 && operations.Peek().Priority >= operation.Priority)
						{
							CreateNode(ref stack, operations.Pop().OriginalOperation);
						}
						operations.Push(operation);
					}
				}
				else if (current.OriginalOperation == Operation.CloseBracket)
				{
					action(false);
					while (operations.Count > 0)
					{
						var temp = operations.Pop();
						int argsCount = 0;

						if (temp.Type == OType.Arithmetic)
						{
							CreateNode(ref stack, temp.OriginalOperation);
						}
						else if (temp.Type == OType.Method)
						{
							if (stack.Count >= argsCount)
							{
								var t = stack.Take(argsCount);

								var method = aggregation.GetMethod(temp.Value);
								if (method.HasValue)
								{
									var _params = new List<ExpressionNode>();
									for (int i = 0; i < method.Value.Params.Length; i++)
									{
										_params.Add(stack.Pop());
									}
									_params.Reverse();
									stack.Push(new ExpressionExternMethod(aggregation, method.Value.Name, method.Value.IsStatic,
										_params.ToArray()));
								}

							}
							else
							{
								throw new Exception("exception");
							}
						}
						else if (temp.Type == OType.Separator)
						{
							argsCount++;
						}


						if (temp.OriginalOperation == Operation.CloseBracket)
							break;

					}
				}
				else if (current.OriginalOperation == Operation.Comma)
				{
					action(false);
					while (operations.Peek().Type != OType.Method && operations.Peek().Type != OType.Separator)
					{
						CreateNode(ref stack, operations.Pop().OriginalOperation);
					}
					operations.Push(new CompleteOperation(','));
				}
				else
				{
					buffer.Add(c);
				}
			}
			action(false);
			while (operations.Count > 0)
			{
				CreateNode(ref stack, operations.Pop().OriginalOperation);
			}
			return stack.Pop();
		}

		private void CreateNode(ref Stack<ExpressionNode> stack, Operation operation)
		{
			var _r = stack.Pop();
			var _l = stack.Pop();
			stack.Push(new ExpressionOperation(operation, _l, _r));
		}
	}
}
