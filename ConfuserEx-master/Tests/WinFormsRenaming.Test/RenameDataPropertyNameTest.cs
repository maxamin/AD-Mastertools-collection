using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace WinFormsRenaming.Test {
	public class RenameDataPropertyNameTest : TestBase {
		public RenameDataPropertyNameTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "rename")]
		[Trait("Technology", "Windows Forms")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/54")]
		public async Task RenameWindowsFormsTest() =>
			await Run(
				"WinFormsRenaming.dll",
				null,
				new SettingItem<Protection>("rename"),
				outputAction: message => Assert.DoesNotContain("Failed to extract binding property name in", message));
	}
}
