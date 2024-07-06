using System;

namespace SignatureMismatch {
	class Program {
		static int Main(string[] args) {
			Console.WriteLine("START");
			var dict = new EasyDict<string, string> {
				{ "Test1", "Test2" }
			};
			Console.WriteLine("Dictionary created");
			Console.WriteLine($"Dictionary count: {dict.Count:d}");

			var file = new TextFile("filename", "text");

			foreach (var kvp in dict) 
				Console.WriteLine($"[{kvp.Key}] = {kvp.Value}");
			
			Console.WriteLine("END");

            return 42;
		}
	}
}
