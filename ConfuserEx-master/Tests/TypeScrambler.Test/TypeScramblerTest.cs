using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace TypeScrambler.Test {
	public sealed class TypeScramblerTest : TestBase {
		public TypeScramblerTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "typescramble")]
		public async Task ScrambleAndExecuteTest()  =>
			await Run("TypeScrambler.exe",
				new[] {
					"Text from WriteTextToConsole",
					"Static Text",
					"Non-Static Text",
					"Text from generic method",
					"Text from generic class",
					"Text from Resources",
					"Text from implicit interface implementation.",
					"Text from implicit interface implementation.", 
					"Text from explicit interface implementation.", 
					"Text from static generic method.",
					"From the factory: Test"
				},
				new SettingItem<Protection>("typescramble"));
	}
}
