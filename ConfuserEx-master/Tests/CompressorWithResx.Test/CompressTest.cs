using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace CompressorWithResx.Test {
	public sealed class CompressTest : TestBase {
		public CompressTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Theory]
		[MemberData(nameof(CompressAndExecuteTestData))]
		[Trait("Category", "Packer")]
		[Trait("Packer", "compressor")]
		public async Task CompressAndExecuteTest(string compatKey, string deriverKey, string resourceProtectionMode) =>
			await Run(
				new[] {"CompressorWithResx.exe", Path.Combine("de", "CompressorWithResx.resources.dll")},
				new[] {"Test (fallback)", "Test (deutsch)"},
				resourceProtectionMode != "none"
					? new SettingItem<Protection>("resources") {{"mode", resourceProtectionMode}}
					: null,
				$"_{compatKey}_{deriverKey}_{resourceProtectionMode}",
				packer: new SettingItem<Packer>("compressor") {{"compat", compatKey}, {"key", deriverKey}});

		public static IEnumerable<object[]> CompressAndExecuteTestData() {
			foreach (var compressorCompatKey in new [] { "true", "false" })
				foreach (var compressorDeriveKey in new [] { "normal", "dynamic" })
					foreach (var resourceProtectionMode in new [] { "none", "normal", "dynamic" })
						yield return new object[] { compressorCompatKey, compressorDeriveKey, resourceProtectionMode };
		}
	}
}
