using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Confuser.Core.Services;
using Confuser.Renamer.Analyzers;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Xunit;

namespace Confuser.Renamer.Test.Analyzers {
	public sealed class ManifestResourceAnalyzerTest {
		private static Stream GetManifestStreamSource() =>
			typeof(ManifestResourceAnalyzerTest).Assembly.GetManifestResourceStream(typeof(ManifestResourceAnalyzerTest), "Test_Resource.txt");
		private static Stream GetManifestStreamResult() =>
			typeof(ManifestResourceAnalyzerTest).Assembly.GetManifestResourceStream("Confuser.Renamer.Test.Analyzers.Test_Resource.txt");

		[Fact]
		public void TestReferenceMethod1Test() {
			var moduleDef = Helpers.LoadTestModuleDef();
			var thisTypeDef = moduleDef.Find(typeof(ManifestResourceAnalyzerTest).FullName, false);
			var refMethod = thisTypeDef.FindMethod(nameof(GetManifestStreamSource));

			var traceService = new TraceService();
			ManifestResourceAnalyzer.PreRename(moduleDef, traceService, refMethod);

			CompareMethodBody(refMethod.Body, thisTypeDef.FindMethod(nameof(GetManifestStreamResult)).Body);
		}

		private static void CompareMethodBody(CilBody body1, CilBody body2) {
			Assert.Equal(body1.HasInstructions, body2.HasInstructions);
			if (!body1.HasInstructions) return;

			Assert.Equal(body1.Instructions.Count, body2.Instructions.Count);
			for (var i = 0; i < body1.Instructions.Count; i++) {
				var instruction1 = body1.Instructions[i];
				var instruction2 = body2.Instructions[i];

				Assert.Equal(instruction1.OpCode, instruction2.OpCode);
				if (instruction1.Operand is IMethodDefOrRef methodRef1) {
					var methodRef2 = Assert.IsAssignableFrom<IMethodDefOrRef>(instruction2.Operand);
					Assert.Equal(methodRef1.FullName, methodRef2.FullName);
				} else {
					Assert.Equal(instruction1.Operand, instruction2.Operand);
				}
			}
		}
	}
}
