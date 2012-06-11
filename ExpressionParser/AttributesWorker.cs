using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ExpressionParser
{
	internal static class AttributesWorker
	{
		public static string GetDescription(object source)
		{
			Type type = source.GetType();
			foreach (var item in type.GetFields())
			{
				var attribute = item.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
				if (attribute == default(System.ComponentModel.DescriptionAttribute))
				{
					continue;
				}
				return (attribute as System.ComponentModel.DescriptionAttribute).Description;
			}
			return null;
		}

		public static IEnumerable<object> Where<TAttribute>(this object source, Predicate<TAttribute> condition)
			where TAttribute : Attribute
		{
			return Where<TAttribute>(source.GetType(), condition);
		}

		public static IEnumerable<object> Where<TAttribute>(Type type, Predicate<TAttribute> condition)
			where TAttribute : Attribute
		{
			foreach (var item in type.GetMembers())
			{
				var attributes = item.GetCustomAttributes(typeof(TAttribute), false);
				foreach (var attribute in attributes)
				{
					if (condition(attribute as TAttribute))
					{
						yield return item.GetCustomAttributes(typeof(OperationTypeAttribute), false).First();
					}
				}
			}
		}

		public static IEnumerable<TResult> SelectWhere<TAttribute, TResult>(Type type, Func<MemberInfo, TAttribute, TResult> selector)
			where TAttribute : Attribute
		{
			foreach (var item in type.GetMembers())
			{
				var attributes = item.GetCustomAttributes(typeof(TAttribute), false);
				foreach (var attribute in attributes)
				{
					yield return selector(item, attribute as TAttribute);
				}
			}
		}
	}
}
