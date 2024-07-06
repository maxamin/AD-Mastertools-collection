namespace InheritCustomAttr {
	// this should inherit T with the MyAttribute from its base class D, C
	sealed class E : D {
		public override int U => 43;
	}
}
