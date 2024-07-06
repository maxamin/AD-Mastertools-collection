using System;
using SignatureMismatch2Helper;

namespace SignatureMismatch {
	public interface IInterface
	{
		void Method(External obj);
	}

	public class Class : IInterface
	{
		public void Method(External obj) => Console.WriteLine(obj.Name);
	}

	public interface IInterface2<Result> {
		void Method(Result obj);
	}

	public class Class2 : IInterface2<External> {
		public void Method(External obj) => Console.WriteLine(obj.Name);
	}

	public class Program {
		static int Main(string[] args) {
			Console.WriteLine("START");
			new Class().Method(new External());
			new Class2().Method(new External());
			Console.WriteLine("END");

			return 42;
		}
	}
}
