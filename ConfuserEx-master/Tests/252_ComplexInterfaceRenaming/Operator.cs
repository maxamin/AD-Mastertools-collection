using System;

namespace ComplexInterfaceRenaming {
	internal sealed class Operator : IC, IOperator, IName {
		public string Name { get; set; }

		public void C(string x) => throw new NotImplementedException();

		void IOperator.Operate() => throw new NotImplementedException();
	}
}
