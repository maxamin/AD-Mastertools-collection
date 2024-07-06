using System;

namespace CompressorWithResx {
	public class Program {
		internal static int Main(string[] args) {
			Console.WriteLine("START");
			Properties.Resources.Culture = new System.Globalization.CultureInfo("en-US");
			Console.WriteLine(Properties.Resources.TestString);
			Properties.Resources.Culture = new System.Globalization.CultureInfo("de-DE");
			Console.WriteLine(Properties.Resources.TestString);
			Console.WriteLine("END");
			return 42;
		}
	}
}
