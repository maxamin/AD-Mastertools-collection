using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Confuser.Core.Services;
using Confuser.Renamer.Analyzers;
using Confuser.UnitTest;
using dnlib.DotNet;
using Moq;
using Xunit;
using Xunit.Abstractions;
using ILogger = Confuser.Core.ILogger;

namespace Confuser.Renamer.Test.Analyzers {
	public sealed class ReflectionAnalyzerTest {
#pragma warning disable IDE0044 // Add readonly modifier
		private string _referenceField;
#pragma warning restore IDE0044 // Add readonly modifier
		private readonly ITestOutputHelper _outputHelper;

		public ReflectionAnalyzerTest(ITestOutputHelper outputHelper) =>
			_outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));

		private ILogger CreateLogger() => new XunitLogger(_outputHelper);

		private string ReferenceProperty { get; }

		private void TestReferenceMethod1() {
			var method1 = typeof(ReflectionAnalyzerTest).GetMethod(nameof(TestReferenceMethod1));
			Assert.Null(method1);
			var method2 = typeof(ReflectionAnalyzerTest).GetMethod(nameof(TestReferenceMethod1), BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.NotNull(method2);
			var method3 = typeof(ReflectionAnalyzerTest).GetMethod(nameof(TestReferenceMethod1), BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Standard, new Type[] { typeof(string) }, null);
			Assert.Null(method3);
		}

		[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "It's not a test!")]
		public void TestReferenceField1() {
			var field1 = typeof(ReflectionAnalyzerTest).GetField(nameof(_referenceField));
			Assert.Null(field1);
			var field2 = typeof(ReflectionAnalyzerTest).GetField(nameof(_referenceField), BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.NotNull(field2);
		}
		
		[SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "It's not a test!")]
		public void TestReferenceProperty1() {
			var prop1 = typeof(ReflectionAnalyzerTest).GetProperty(nameof(ReferenceProperty));
			Assert.Null(prop1);
			var prop2 = typeof(ReflectionAnalyzerTest).GetProperty(nameof(ReferenceProperty), BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.NotNull(prop2);
		}

		[Fact]
		public void TestReferenceMethod1Test() {
			TestReferenceMethod1();

			var moduleDef = Helpers.LoadTestModuleDef();
			var thisTypeDef = moduleDef.Find("Confuser.Renamer.Test.Analyzers.ReflectionAnalyzerTest", false);
			var refMethod = thisTypeDef.FindMethod(nameof(TestReferenceMethod1));

			var nameService = Mock.Of<INameService>();
			Mock.Get(nameService).Setup(s => s.SetCanRename(refMethod, false));
			Mock.Get(nameService).Setup(s => s.SetCanRename(refMethod, false));
			Mock.Get(nameService).Setup(s => s.GetReferences(refMethod)).Returns(new List<INameReference>());

			var traceService = new TraceService();
			var analyzer = new ReflectionAnalyzer();
			analyzer.Analyze(nameService, traceService, new List<ModuleDef>() { moduleDef }, CreateLogger(), refMethod);

			Mock.Get(nameService).VerifyAll();
		}

		[Fact]
		public void TestReferenceField1Test() {
			TestReferenceField1();

			var moduleDef = Helpers.LoadTestModuleDef();
			var thisTypeDef = moduleDef.Find("Confuser.Renamer.Test.Analyzers.ReflectionAnalyzerTest", false);
			var refMethod = thisTypeDef.FindMethod(nameof(TestReferenceField1));
			var refField = thisTypeDef.FindField(nameof(_referenceField));

			var nameService = Mock.Of<INameService>();
			Mock.Get(nameService).Setup(s => s.SetCanRename(refField, false));
			Mock.Get(nameService).Setup(s => s.SetCanRename(refField, false));

			var traceService = new TraceService();
			var analyzer = new ReflectionAnalyzer();

			analyzer.Analyze(nameService, traceService, new List<ModuleDef>() { moduleDef }, CreateLogger(), refMethod);

			Mock.Get(nameService).VerifyAll();
		}

		[Fact]
		public void TestReferenceProperty1Test() {
			TestReferenceProperty1();

			var moduleDef = Helpers.LoadTestModuleDef();
			var thisTypeDef = moduleDef.Find("Confuser.Renamer.Test.Analyzers.ReflectionAnalyzerTest", false);
			var refMethod = thisTypeDef.FindMethod(nameof(TestReferenceProperty1));
			var refProp = thisTypeDef.FindProperty(nameof(ReferenceProperty));

			var nameService = Mock.Of<INameService>();
			Mock.Get(nameService).Setup(s => s.SetCanRename(refProp, false));
			Mock.Get(nameService).Setup(s => s.SetCanRename(refProp, false));
			Mock.Get(nameService).Setup(s => s.GetReferences(refProp)).Returns(new List<INameReference>());

			var traceService = new TraceService();
			var analyzer = new ReflectionAnalyzer();
			analyzer.Analyze(nameService, traceService, new List<ModuleDef>() { moduleDef }, CreateLogger(), refMethod);

			Mock.Get(nameService).VerifyAll();
		}
	}
}
