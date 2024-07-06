using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRenaming {
	internal class UserControl1Context {
		public string TestProperty => "This is from a property!";

		public string[] TestListProperty => new string[] { "Index 1", "Index 2" };

		public string this[int index] {
			get => $"Integer Indxer: {index}";
		}

		public string this[long index] {
			get => $"Long Indexer: {index}";
		}

		public string this[string index] {
			get => $"String Indexer: {index}";
		}
	}
}
