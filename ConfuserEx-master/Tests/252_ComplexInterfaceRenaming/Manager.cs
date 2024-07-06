using System;

namespace ComplexInterfaceRenaming {
	internal sealed class Manager : Worker, IWorker, IOperator
	{
		public string Name => "I'm a manager!";

		public void Operate() => throw new NotImplementedException();
	}
}
