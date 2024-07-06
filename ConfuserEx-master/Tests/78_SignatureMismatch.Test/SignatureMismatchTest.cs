using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace SignatureMismatch.Test {
	public class SignatureMismatchTest : TestBase {
		public SignatureMismatchTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "rename")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/78")]
		public async Task SignatureMismatch() =>
			await Run(
				"78_SignatureMismatch.exe",
				new [] {
					"Dictionary created",
					"Dictionary count: 1",
					"[Test1] = Test2"
				},
				new SettingItem<Protection>("rename")
			);
	}
}
