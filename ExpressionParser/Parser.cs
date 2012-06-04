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

            var str = GenerateReverseRecord(expression);
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


            foreach (var c in expression)
            {
                if (c == '(')
                {
                    operations.Push(c);
                }
                else if (IsOperation(c))
                {
                    if (operations.Count == 0)
                    {
                        operations.Push(c);
                    }
                    else
                    {
                        while (operations.Count != 0 && priority.First(item => item.Value.Contains(operations.Peek())).Key >= priority.First(item => item.Value.Contains(c)).Key)
                        {
                            var tempOperation = GetOperation(operations.Pop());
                            var _r = stack.Pop();
                            var _l = stack.Pop();
                            var expr = new ExpressionOperation(tempOperation ?? Operation.Unknown, _l, _r);
                            stack.Push(expr);
                        }
                        operations.Push(c);
                    }
                }
                else if (c == ')')
                {
                    while (operations.Count > 0)
                    {
                        char temp = operations.Pop();
                        if (temp != '(')
                        {
                            var tempOperation = GetOperation(temp);
                            var _r = stack.Pop();
                            var _l = stack.Pop();
                            var expr = new ExpressionOperation(tempOperation ?? Operation.Unknown, _l, _r);
                            stack.Push(expr);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var value = new ExpressionValue(float.Parse(c.ToString()));
                    stack.Push(value);
                }
            }
            while (operations.Count > 0)
            {
                var tempOperation = GetOperation(operations.Pop());
                var _r = stack.Pop();
                var _l = stack.Pop();
                var expr = new ExpressionOperation(tempOperation ?? Operation.Unknown, _l, _r);
                stack.Push(expr);
            }
            return stack.Pop();
        }

        #region to delete
        private static string GenerateReverseRecord(string expression)
        {
            var stack = new Stack<char>();
            var reverseRecord = new List<char>();
            foreach (var c in expression)
            {
                if (stack.Count == 0)
                {
                    if (IsOperation(c) || c == '(')
                    {
                        stack.Push(c);
                        //if (reverseRecord.Count == 0 || IsOperation(reverseRecord.Last()))
                        //{
                        //    reverseRecord.Add('0');
                        //}
                        //reverseRecord.Add(',');
                        continue;
                    }
                }
                if (IsOperation(c))
                {
                    while (stack.Count != 0 && priority.First(item => item.Value.Contains(stack.Peek())).Key >= priority.First(item => item.Value.Contains(c)).Key)
                    {
                        reverseRecord.Add(stack.Pop());
                    }
                }

                if (c == ')')
                {
                    while (stack.Count > 0)
                    {
                        char temp = stack.Pop();
                        if (temp != '(')
                        {
                            reverseRecord.Add(temp);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (operations.Contains(c) || c == '(')
                {
                    stack.Push(c);
                    //if (reverseRecord.Count == 0 || IsOperation(reverseRecord.Last()))
                    //{
                    //    reverseRecord.Add('0');
                    //}
                    //reverseRecord.Add(',');
                }
                else
                {
                    reverseRecord.Add(c);
                }
            }
            while (stack.Count > 0)
            {
                reverseRecord.Add(stack.Pop());
            }
            return String.Join(String.Empty, reverseRecord.ToArray());
        }
        #endregion

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
