using System;
using System.Linq;
using Confuser.Core.Services;
using dnlib.DotNet.Emit;
using Xunit;

namespace Confuser.Core.Test.Services {
	public class TraceServiceTest {
		public static readonly Type Int = typeof(int);
		static Type GetType(object v) => v.GetType();

#pragma warning disable IDE0051 // Remove unused private member
#pragma warning disable IDE0060 // Remove unused parameter
		private void X(int a, int b) { }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private member

		static void TestReferenceMethod() => typeof(TraceServiceTest).GetMethod("X", new[] { GetType(1) ?? Int });

		[Fact]
		public void TraceTestReferenceMethodTest() {
			var moduleDef = Helpers.LoadTestModuleDef();
			var thisTypeDef = moduleDef.Find("Confuser.Core.Test.Services.TraceServiceTest", false);
			var refMethod = thisTypeDef.FindMethod(nameof(TestReferenceMethod));

			var traceService = new TraceService();
			var methodTrace = traceService.Trace(refMethod);

			var getMethodCall = refMethod.Body.Instructions.Single(i =>
				i.OpCode == OpCodes.Call && i.Operand.ToString().Contains("GetMethod"));
			var arguments = methodTrace.TraceArguments(getMethodCall);

			Assert.NotNull(arguments);
		}
	}
}
