using System;

namespace ComplexInterfaceRenaming {
	public class Program {
		static int Main(string[] args) {
			Console.WriteLine("START");
			var op = new Operator {
				Name = "I'm a operator."
			};
			Console.WriteLine("Operator: " + op.Name);
			Console.WriteLine("Operator(IOperator): " + (op as IOperator).Name);
			Console.WriteLine("Operator(IName): " + (op as IName).Name);

			var manager = new Manager();
			Console.WriteLine("Manager: " + manager.Name);
			Console.WriteLine("Manager(IWorker): " + (manager as IWorker).Name);
			Console.WriteLine("Manager(IOperator): " + (manager as IOperator).Name);
			Console.WriteLine("Manager(IName): " + (manager as IName).Name);
			manager.C("abc");

			Console.WriteLine("END");

			return 42;
		}
	}
}
