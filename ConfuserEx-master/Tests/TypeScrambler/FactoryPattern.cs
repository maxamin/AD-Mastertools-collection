using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScrambler {
	internal sealed class FactoryPattern {
		internal string Message { get; }
		private FactoryPattern(string message) => Message = $"From the factory: {message}";

		internal static FactoryPattern Create(string message) => new FactoryPattern(message);
	}
}
