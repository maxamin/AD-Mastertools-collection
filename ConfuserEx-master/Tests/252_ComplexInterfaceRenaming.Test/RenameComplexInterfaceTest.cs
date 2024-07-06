using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.Renamer;
using Confuser.UnitTest;
using Xunit;
using Xunit.Abstractions;

namespace ComplexInterfaceRenaming.Test {
	public class RenameComplexInterfaceTest : TestBase {
		public RenameComplexInterfaceTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Theory]
		[MemberData(nameof(RenameDynamicTypeData))]
		[Trait("Category", "Protection")]
		[Trait("Protection", "rename")]
		[Trait("Issue", "https://github.com/mkaring/ConfuserEx/issues/252")]
		public async Task RenameComplexInterfaces(string renameMode, bool flatten) =>
			await Run(
				"252_ComplexInterfaceRenaming.exe",
				new[] {
					"Operator: I'm a operator.",
					"Operator(IOperator): I'm a operator.",
					"Operator(IName): I'm a operator.",
					"Manager: I'm a manager!",
					"Manager(IWorker): I'm a manager!",
					"Manager(IOperator): I'm a manager!",
					"Manager(IName): I'm a manager!",
					"Working: abc"
				},
				new SettingItem<Protection>("rename") {
					{ "mode", renameMode },
					{ "flatten", flatten.ToString() }
				},
				$"_{renameMode}_{flatten}"
			);

		public static IEnumerable<object[]> RenameDynamicTypeData() {
			foreach (var renameMode in new[] { nameof(RenameMode.Unicode), nameof(RenameMode.ASCII), nameof(RenameMode.Letters), nameof(RenameMode.Debug), nameof(RenameMode.Retain) })
				foreach (var flatten in new[] { true, false })
					yield return new object[] { renameMode, flatten };
		}
	}
}
