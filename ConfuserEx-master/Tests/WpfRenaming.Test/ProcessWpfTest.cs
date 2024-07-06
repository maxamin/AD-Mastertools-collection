using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace WpfRenaming.Test {
	public class ProcessWpfTest : TestBase {
		public ProcessWpfTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		/// <see cref="https://github.com/mkaring/ConfuserEx/issues/1"/>
		[Fact]
		[Trait("Category", "Analysis")]
		[Trait("Protection", "rename")]
		[Trait("Technology", "WPF")]
		public async Task ProcessWithoutObfuscationTest() =>
			await Run(
				"WpfRenaming.dll",
				null,
				NoProtections);

		[Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "rename")]
		[Trait("Technology", "WPF")]
		public async Task ProcessWithObfuscationTest() =>
			await Run(
				"WpfRenaming.dll",
				null,
				new SettingItem<Protection>("rename"));
	}
}
