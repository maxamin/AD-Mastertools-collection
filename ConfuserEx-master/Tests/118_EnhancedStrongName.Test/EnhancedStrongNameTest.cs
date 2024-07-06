using System;
using System.IO;
using System.Threading.Tasks;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace EnhancedStrongName.Test {
	public class EnhancedStrongNameTest : TestBase {
		public EnhancedStrongNameTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact]
		[Trait("Category", "core")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/118")]
		public async Task EnhancedStrongName() =>
			await Run("118_EnhancedStrongName.exe",
				new[] {"My strong key token: 79A18AF4CEA8A9BD", "My signature is valid!"},
				NoProtections,
				projectModuleAction: projectModule => {
					projectModule.SNSigKeyPath = Path.Combine(Environment.CurrentDirectory, "SignatureKey.snk");
					projectModule.SNPubSigKeyPath = Path.Combine(Environment.CurrentDirectory, "SignaturePubKey.snk");
					projectModule.SNKeyPath = Path.Combine(Environment.CurrentDirectory, "IdentityKey.snk");
					projectModule.SNPubKeyPath = Path.Combine(Environment.CurrentDirectory, "IdentityPubKey.snk");
				});
	}
}
