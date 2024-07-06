using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confuser.Renamer.Analyzers;
using Confuser.UnitTest;
using dnlib.DotNet;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Confuser.Renamer.Test.Analyzers {
	[Implementation(Value = 5)]
	public class TypeBlobAnalyzerTest {
		private readonly ITestOutputHelper outputHelper;

		public TypeBlobAnalyzerTest(ITestOutputHelper outputHelper) =>
			this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));

        [Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "rename")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/84")]
		public void AnalyseAttributeTest() {
			var moduleDef = Helpers.LoadTestModuleDef();
            
			var nameService = Mock.Of<INameService>();

            void VerifyLog(string message) {
                Assert.DoesNotContain("Failed to resolve CA field", message);
                Assert.DoesNotContain("Failed to resolve CA property", message);
			}

            TypeBlobAnalyzer.Analyze(nameService, new List<ModuleDefMD>() { moduleDef }, new XunitLogger(outputHelper, VerifyLog), moduleDef);

			Mock.Get(nameService).VerifyAll();
		}
	}
}
