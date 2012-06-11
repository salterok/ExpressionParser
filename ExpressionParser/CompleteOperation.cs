using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace ExpressionParser
{
	class CompleteOperation
	{
		private static List<string> operationList;

		private Operation _operation = Operation.Unknown;
		private OType _type = OType.Unknown;
		private int _priority = Int32.MinValue;

		public string Value { get; private set; }

		static CompleteOperation()
		{
			// TODO: fill operationList
			operationList = new List<string>();
			Type type = typeof(Operation);
			foreach (var name in Enum.GetNames(type))
			{
				var member = type.GetMember(name).First();
				var attributes = member.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (attributes.Count() > 0)
				{
					operationList.Add((attributes.First() as DescriptionAttribute).Description);
				}
			}

		}

		public CompleteOperation(string input)
		{
			Value = input;
		}

		public CompleteOperation(char c)
		{
			Value = c.ToString();
		}

		public bool TryAdd(char c)
		{
			var count = Value.Length + 1;
			var temp = Value + c;
			var matches = operationList.Where(item => item.Length >= count).Count(item => item.Substring(0, count) == temp);
			if (matches > 0)
			{
				Value = temp;
				return true;
			}
			return false;
		}

		public OType Type
		{
			get
			{
				if (_type == OType.Unknown)
				{
					var results = AttributesWorker.Where<DescriptionAttribute>(typeof(Operation), item => item.Description == Value);
					double temp;
					if (results.Count() > 0)
					{
						// WATCH: rewrite
						_type = (results.First() as dynamic).Type;
					}
					else if (double.TryParse(Value, out temp))
					{
						_type = OType.Value;
					}
					else
					{
						_type = OType.Method;
					}
				}
				return _type;
			}
		}

		public int Priority
		{
			get
			{
				if (Type == OType.Method || Type == OType.Value)
				{
					return _priority = Int32.MinValue;
				}
				if (_priority == Int32.MinValue)
				{
					_priority = AttributesWorker.SelectWhere<DescriptionAttribute, int>(typeof(Operation),
						(member, attribute) =>
						{
							if (attribute.Description == Value)
							{
								return member.GetCustomAttribute<PriorityAttribute>().Priority;
							}
							return Int32.MinValue;
						}
					).Single(item => item != Int32.MinValue);
				}
				return _priority;
			}
		}

		public Operation OriginalOperation
		{
			get
			{
				if (Type == OType.Method || Type == OType.Value)
				{
					return _operation = Operation.Unknown;
				}
				if (_operation == Operation.Unknown)
				{
					_operation = AttributesWorker.SelectWhere<DescriptionAttribute, Operation>(typeof(Operation),
						(member, attribute) =>
						{
							if (attribute.Description == Value)
							{
								return (Operation)(member as FieldInfo).GetRawConstantValue();
							}
							return Operation.Unknown;
						}
					).Single(item => item != Operation.Unknown);
				}
				return _operation;
			}
		}
	}
}
