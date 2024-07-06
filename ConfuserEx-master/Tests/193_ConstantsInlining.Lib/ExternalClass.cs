using System.Runtime.CompilerServices;

namespace ConstantsInlining.Lib {
	public static class ExternalClass {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetText() => "From External";
	}
}
