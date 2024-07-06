using System;

namespace ConstantsInlining {
	public class Program {
		static int Main(string[] args) {
			Console.WriteLine("START");
			Console.WriteLine(Lib.ExternalClass.GetText());
			Console.WriteLine("END");
			return 42;
		}
	}
}
