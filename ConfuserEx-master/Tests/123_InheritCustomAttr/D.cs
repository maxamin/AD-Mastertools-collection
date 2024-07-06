using System;

namespace InheritCustomAttr {
	class D : C {
		#pragma warning disable CS0067
		// Just here to make sure it works.
		public event EventHandler<EventArgs> TestEvent;
		#pragma warning restore CS0067

		// this property should inherit the MyAttribute from its base class
		public override DayOfWeek T { get => DayOfWeek.Monday; }

		public virtual int U => 42;
	}
}
