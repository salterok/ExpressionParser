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

		private void ExecuteAtomicOperation(ref Stack<ExpressionNode> stack, ref Stack<CompleteOperation> operations,
			CompleteOperation current, CompleteOperation previous)
		{
			if (current.OriginalOperation == Operation.OpenBracket)
			{
				if (operations.Peek().Type == OType.Method)
				{
					var temp = operations.Pop();
					operations.Push(current);
					operations.Push(temp);
				}
			}
			else if (current.Type == OType.Arithmetic)
			{
				if (previous == null || previous.Type != OType.Value && previous.Type != OType.Method)
				{
					// WATCH: previous.Type != OType.Method used for handle external constant
					// HACK: need to change later

					// if leading sign
					if (current.OriginalOperation == Operation.Addition || current.OriginalOperation == Operation.Substraction)
					{
						stack.Push(new ExpressionValue(0));
					}

				}
				if (operations.Count == 0)
				{
					operations.Push(current);
				}
				else
				{
					while (operations.Count > 0 && operations.Peek().Priority >= current.Priority)
					{
						CreateNode(ref stack, operations.Pop().OriginalOperation);
					}
					operations.Push(current);
				}
			}
			else if (current.OriginalOperation == Operation.CloseBracket)
			{
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
			else if (current.Type == OType.Method)
			{
				operations.Push(current);
			}
			else if (current.OriginalOperation == Operation.Comma)
			{
				while (operations.Peek().Type != OType.Method && operations.Peek().Type != OType.Separator)
				{
					CreateNode(ref stack, operations.Pop().OriginalOperation);
				}
				operations.Push(new CompleteOperation(','));
			}
			else if (current.Type == OType.Value)
			{
				stack.Push(new ExpressionValue(double.Parse(current.Value)));
			}
		}

		private ExpressionNode foo(string expression)
		{
			var stack = new Stack<ExpressionNode>();
			var operations = new Stack<CompleteOperation>();

			CompleteOperation current = null;
			CompleteOperation previous = null;

			foreach (var c in expression)
			{
				if (current == null)
				{
					current = new CompleteOperation(c);
				}
				else
				{
					if (current.TryAdd(c))
					{
						continue;
					}
					else
					{
						ExecuteAtomicOperation(ref stack, ref operations, current, previous);
						previous = current;
						current = new CompleteOperation(c);
					}
				}
			}

			ExecuteAtomicOperation(ref stack, ref operations, current, previous);
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
