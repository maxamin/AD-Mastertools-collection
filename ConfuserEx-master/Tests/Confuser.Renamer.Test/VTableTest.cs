using System;
using System.Collections.Generic;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace Confuser.Renamer.Test {
	public class VTableTest {
		private readonly ITestOutputHelper outputHelper;

		public VTableTest(ITestOutputHelper outputHelper) =>
			this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));

		[Fact]
		[Trait("Category", "Protection")]
		[Trait("Protection", "rename")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/34")]
		public void DuplicatedMethodSignatureTest() {
			var refClass = new VTableTestRefClass();
			Assert.Equal(1, refClass.TestMethod(new List<string>()));
			Assert.Equal(2, refClass.TestMethod2(new List<string>()));
			var refInterface = refClass as VTableTestRefInterface<string>;
			Assert.NotNull(refInterface);
			Assert.Equal(1, refInterface.TestMethod(new List<string>()));
			Assert.Equal(2, refInterface.TestMethod2(new List<string>()));
			int CallGenericFunction<T>(VTableTestRefInterface<T> refIfc) => refIfc.TestMethod(new List<T>());
			Assert.Equal(1, CallGenericFunction(refInterface));

			var moduleDef = Helpers.LoadTestModuleDef();
			var refClassTypeDef = moduleDef.Find("Confuser.Renamer.Test.VTableTestRefClass", false);
			
			Assert.NotNull(refClassTypeDef);
			var vTableStorage = new VTableStorage(new XunitLogger(outputHelper));
			var refClassVTable = vTableStorage.GetVTable(refClassTypeDef);
			Assert.NotNull(refClassVTable);
		}
	}

	internal class VTableTestRefClass : VTableTestRefInterface<string> {
		public int TestMethod(List<string> values) => 1;
		public int TestMethod2(List<string> values) => 2;
	}

	internal interface VTableTestRefInterface<T> {
		int TestMethod(List<string> values);
		int TestMethod(List<T> values);
		int TestMethod2(List<string> values);
	}
}
