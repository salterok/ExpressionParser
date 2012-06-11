using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;

namespace ExpressionParser
{
	class CompleteOperation
	{
		private static List<string> operationList;

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
			if (Type == OType.Arithmetic)
			{
				var count = Value.Length + 1;
				var temp = Value;
				Value = temp;
				temp += c;
				var matches = operationList.Where(item => item.Length >= count).Count(item => item.Substring(0, count) == temp);
				if (matches > 0)
				{
					Value = temp;
					return true;
				}
				return false;
			}
			else if (Type == OType.Value)
			{
				var backup = Value;
				var type = Type;
				Value += c;
				if (type != Type)
				{
					Value = backup;
					return false;
				}
				else
				{
					return true;
				}
			}
			else if (Type == OType.Separator)
			{
				// all separators are one char
				return false;
			}
			else if (Type == OType.Method)
			{
				if (new CompleteOperation(c).Type == OType.Separator)
				{
					return false;
				}
				else
				{
					Value += c;
					return true;
				}
			}
			else
			{
				throw new Exception("unknown");
			}
		}

		public OType Type
		{
			get
			{	
				var results = AttributesWorker.Where<DescriptionAttribute>(typeof(Operation), item => item.Description == Value);
				double temp;
				CultureInfo culture = CultureInfo.InvariantCulture;
				//culture.NumberFormat.
				if (results.Count() > 0)
				{
					// WATCH: rewrite
					return (results.First() as dynamic).Type;
				}
				else if (double.TryParse(Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out temp))
				{
					return OType.Value;
				}
				else if (System.Text.RegularExpressions.Regex.IsMatch(Value, @"^(?:(_)|[a-zA-Z])(?(1)\w+|\w*)$"))
				{
					return OType.Method;
				}
				else
				{
					return OType.Unknown;
				}
			}
		}

		public int Priority
		{
			get
			{
				if (Type == OType.Method || Type == OType.Value)
				{
					return Int32.MinValue;
				}
				return AttributesWorker.SelectWhere<DescriptionAttribute, int>(typeof(Operation),
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
		}

		public Operation OriginalOperation
		{
			get
			{
				if (Type == OType.Method || Type == OType.Value)
				{
					return Operation.Unknown;
				}
				return AttributesWorker.SelectWhere<DescriptionAttribute, Operation>(typeof(Operation),
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
		}
	}
}
