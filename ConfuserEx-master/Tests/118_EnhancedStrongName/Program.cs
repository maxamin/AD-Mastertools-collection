using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace EnhancedStrongName {
	public class Program {
		internal static int Main(string[] args) {
			Console.WriteLine("START");
			var assembly = typeof(Program).Assembly;
			var assemblyName = new AssemblyName(assembly.FullName);

			Console.WriteLine("My strong key token: " + BytesToString(assemblyName.GetPublicKeyToken()));

			var clrStrongName = (IClrStrongName)RuntimeEnvironment.GetRuntimeInterfaceAsObject(
				new Guid("B79B0ACD-F5CD-409b-B5A5-A16244610B92"),
				new Guid("9FD93CCF-3280-4391-B3A9-96E1CDE77C8D"));

			int result = clrStrongName.StrongNameSignatureVerificationEx(assembly.Location, true, out var verified);
			if (result == 0 && verified)
				Console.WriteLine("My signature is valid!");

			Console.WriteLine("END");
			return 42;
		}

		private static string BytesToString(byte[] data) {
			var builder = new StringBuilder();
			foreach (var val in data) {
				builder.AppendFormat("{0:X2}", val);
			}
			return builder.ToString();
		}
	}
}
