using System;

namespace InheritCustomAttr {
	abstract class C : I<DayOfWeek>, IDayOfWeek {
		[My(Value = 1)]
		public abstract DayOfWeek T { get; }
	}
}
