using System;
using System.Linq;

namespace TypeScrambler {
	public class Program {
		internal static int Main(string[] args) {
			Console.WriteLine("START");
			TestClass.WriteTextToConsole();
			Console.WriteLine(TestClass.GetTextStatic());

			var instance = new TestClass();
			Console.WriteLine(instance.GetText());

			Console.WriteLine(instance.GetTextFromGeneric("Text from generic method".AsEnumerable()));

			var genericInstance = new GenericClass<string>();
			Console.WriteLine(new String(genericInstance.GetReverse("ssalc cireneg morf txeT").ToArray()));

			Console.WriteLine(Properties.Resources.Test);

			var implInterface = new ImplicitInterface();
			Console.WriteLine(implInterface.GetText());
			Console.WriteLine(((ITestInterface)implInterface).GetText());

			var explInterface = new ExplicitInterface();
			Console.WriteLine(((ITestInterface)explInterface).GetText());

			Console.WriteLine(TestClass.GetTextStaticGeneric("Text from static generic method."));

			Console.WriteLine(FactoryPattern.Create("Test").Message);

			Console.WriteLine("END");
			return 42;
		}
	}
}
