using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace ConstantsInlining.Test {
	public class ConstantInliningTest : TestBase {
		public ConstantInliningTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "constants")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/193")]
		public async Task ConstantInlining() =>
			await Run(new[] {"193_ConstantsInlining.exe", "193_ConstantsInlining.Lib.dll"},
				new[] {"From External"},
				new SettingItem<Protection>("constants") {{"elements", "S"}});
	}
}
