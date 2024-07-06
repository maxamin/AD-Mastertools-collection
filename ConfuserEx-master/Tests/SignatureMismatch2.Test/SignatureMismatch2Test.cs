using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace SignatureMismatch2.Test {
	public class SignatureMismatch2Test : TestBase {
		public SignatureMismatch2Test(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "rename")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/187")]
		public async Task SignatureMismatch2() =>
			await Run(
				new [] { "SignatureMismatch2.exe", "SignatureMismatch2Helper.dll" },
				new [] { "External", "External" },
				new SettingItem<Protection>("rename") { ["renPublic"] = "true" }
			);
	}
}
