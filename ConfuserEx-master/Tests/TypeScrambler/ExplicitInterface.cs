namespace TypeScrambler {
	internal class ExplicitInterface : ITestInterface {
		string ITestInterface.GetText() => "Text from explicit interface implementation.";
	}
}
