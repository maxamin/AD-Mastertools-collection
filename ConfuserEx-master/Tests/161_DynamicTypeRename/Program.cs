using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeRename {
	public delegate void TestDelegate<T>();

	class Program {
		static int Main(string[] args) {
			Console.WriteLine("START");
			var assemblyName = new AssemblyName("DynamicAssembly");
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

			var typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public | TypeAttributes.Sealed);
			var genericTypes = typeBuilder.DefineGenericParameters("T");

			typeBuilder.DefineField("DynamicField", typeof(TestDelegate<>).MakeGenericType(genericTypes[0]), FieldAttributes.Public | FieldAttributes.Static);
			Console.WriteLine("Type declaration done");

			var dynamicType = typeBuilder.CreateType();
			var genericDynamicType = dynamicType.MakeGenericType(typeof(string));
			Console.WriteLine("Dynamic type created");

			var fields = genericDynamicType.GetFields();
			Console.WriteLine("Fields in type: " + fields.Length);

			if (fields.Length == 1) {
				fields[0].GetValue(null);
				Console.WriteLine("Fetching field value is okay");
			}

			Console.WriteLine("END");
			return 42;
		}
	}
}
