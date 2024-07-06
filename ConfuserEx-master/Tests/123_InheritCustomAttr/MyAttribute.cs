using System;

namespace InheritCustomAttr {

	[System.AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
	sealed class MyAttribute : Attribute {
		public int Value { get; set; }
	}
}
