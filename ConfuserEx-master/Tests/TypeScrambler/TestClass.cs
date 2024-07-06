using System;

namespace TypeScrambler {
	internal class TestClass {
		public static void WriteTextToConsole() => Console.WriteLine("Text from WriteTextToConsole");

		public static string GetTextStatic() => "Static Text";

		public static string GetTextStaticGeneric<T>(T input) => input.ToString();

		public string GetText() => "Non-Static Text";

		public string GetTextFromGeneric<T>(T input) => input.ToString();
	}
}
