using System.Collections.Generic;
using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace IncorrectRedirectToGac.Test {
	public class IncorrectRedirectToGacTest : TestBase {
		public IncorrectRedirectToGacTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "core")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/144")]
		public async Task IncorrectRedirectToGac() =>
			await Run(
				new [] { "IncorrectRedirectToGac.exe", "Microsoft.Build.Framework.dll" }, new string[0], NoProtections
			);

		[Fact]
		[Trait("Category", "core")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/144")]
		public async Task IncorrectExternalRedirectToGac() =>
			await Run(
				new [] { "IncorrectRedirectToGac.exe", "external:Microsoft.Build.Framework.dll" }, new string[0], NoProtections, outputDirSuffix: "_external"
			);

		[Theory]
		[MemberData(nameof(IncorrectRedirectToGacPackerTestData))]
		[Trait("Category", "core")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/144")]
		public async Task IncorrectRedirectToGacPacker(string compatKey, string deriverKey) =>
			await Run(
				new [] { "IncorrectRedirectToGac.exe", "Microsoft.Build.Framework.dll" }, 
				new string[0], 
				NoProtections,
				outputDirSuffix: $"_packer_{compatKey}_{deriverKey}",
				packer: new SettingItem<Packer>("compressor") {{"compat", compatKey}, {"key", deriverKey}}
			);

		public static IEnumerable<object[]> IncorrectRedirectToGacPackerTestData() {
			foreach (var compressorCompatKey in new [] { "true", "false" })
				foreach (var compressorDeriveKey in new [] { "normal", "dynamic" })
					yield return new object[] { compressorCompatKey, compressorDeriveKey };
		}
	}
}
