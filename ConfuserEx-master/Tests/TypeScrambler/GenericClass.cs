using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScrambler {
	internal class GenericClass<T> where T : IEnumerable<char> {
		public IEnumerable<char> GetReverse(T input) => input.Reverse();
	}
}
