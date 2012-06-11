using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.Xml.Linq;
using System.IO;

namespace ExpressionParser
{
	class Aggregation
	{
		#region nested
		public struct Lib
		{
			#region nested
			public struct Method
			{
				public string Name;
				public bool IsStatic;
				public IEnumerable<string[]> Params;

				public bool IsMatchArgs(int count)
				{
					return Params.Select(args => args.Length).Contains(count);
				}
			}
			public struct Constant
			{
				public string Name;
			}
			#endregion

			public string Name;
			public string Namespace;
			public Method[] Methods;
			public Constant[] Constants;

			public FullMemberName GetFullName(string name)
			{
				string temp;
				temp = Methods.FirstOrDefault(item => item.Name == name).Name;
				if (temp == null)
				{
					temp = Constants.FirstOrDefault(item => item.Name == name).Name;
				}
				return temp == null ? default(FullMemberName) : new FullMemberName(Namespace, temp);
			}
		}

		public struct FullMemberName
		{
			public string type;
			public string member;
			public FullMemberName(string _type, string _name)
			{
				type = _type;
				member = _name;
			}
		}
		#endregion

		public Lib[] Libs { get; private set; }
		public string Version { get; private set; }

		public bool Load(string filename)
		{
			var doc = XDocument.Load(filename);
			
			// TODO: add xsd schema validation

			XmlSchemaSet schemas = new XmlSchemaSet();
			schemas.Add(XmlSchema.Read(new MemoryStream(
				Encoding.UTF8.GetBytes(Properties.Resources.ExternResourceDefinitionLibrarySchema)),
				(o, e) =>
				{
					throw new Exception(e.Message);
				}));

			bool errors = false;
			doc.Validate(schemas, (o, e) =>
			{
				errors = true;
			});
			if (errors)
			{
				return false;
			}



			Version = doc.Root.Attribute("Version").Value;
			var libNames = doc.Root.Elements("Lib");
			var libs = new List<Lib>();
			foreach (var libName in libNames)
			{
				var lib = new Lib()
				{
					Name = libName.Attribute("Name").Value,
					Namespace = libName.Attribute("Type").Value,
					Methods = (from method
							  in libName.Element("Methods").Elements("Method")
							   select new Lib.Method()
							   {
								   Name = method.Attribute("Name").Value,
								   IsStatic = bool.Parse(method.Attribute("Static").Value),
								   Params = (
									   from item
									   in method.Elements("Signature")
									   select item.Elements("Param").Select(arg => arg.Attribute("Type").Value).ToArray()
									   )
							   }).ToArray(),
					Constants = (from constant
								in libName.Element("Constants").Elements("Constant")
								 select new Lib.Constant()
								 {
									 Name = constant.Attribute("Name").Value
								 }).ToArray()
				};
				libs.Add(lib);
			}
			Libs = libs.ToArray();

			return true;
		}

		public Lib.Method? GetMethod(string methodName)
		{
			foreach (var lib in Libs)
			{
				foreach (var method in lib.Methods)
				{
					if (method.Name == methodName)
					{
						return method;
					}
				}
			}
			return null;
		}

		public Lib.Constant? GetConstant(string constantName)
		{
			foreach (var lib in Libs)
			{
				foreach (var constant in lib.Constants)
				{
					if (constant.Name == constantName)
					{
						return constant;
					}
				}
			}
			return null;
		}

		public IEnumerable<FullMemberName> GetFullName(string name)
		{
			foreach (var lib in Libs)
			{
				var result = lib.GetFullName(name);
				if (!result.Equals(default(FullMemberName)))
				{
					yield return result;
				}
			}
		}
	}
}
