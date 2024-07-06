using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Confuser.Core;
using Confuser.Core.Project;
using Xunit;
using Xunit.Abstractions;

namespace Confuser.UnitTest {
	public abstract class TestBase {
		private const string _externalPrefix = "external:";

		readonly ITestOutputHelper outputHelper;

		protected static IEnumerable<SettingItem<Protection>> NoProtections => Enumerable.Empty<SettingItem<Protection>>();

		protected TestBase(ITestOutputHelper outputHelper) =>
			this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));

		protected Task Run(string inputFileName, string[] expectedOutput, SettingItem<Protection> protection,
			string outputDirSuffix = "", Action<string> outputAction = null, SettingItem<Packer> packer = null,
			Action<ProjectModule> projectModuleAction = null, Func<string, Task> postProcessAction = null) =>

			Run(new[] { inputFileName }, expectedOutput, protection, outputDirSuffix, outputAction, packer,
				projectModuleAction, postProcessAction);

		protected Task Run(string inputFileName, string[] expectedOutput, IEnumerable<SettingItem<Protection>> protections,
			string outputDirSuffix = "", Action<string> outputAction = null, SettingItem<Packer> packer = null,
			Action<ProjectModule> projectModuleAction = null, Func<string, Task> postProcessAction = null) =>

			Run(new[] { inputFileName }, expectedOutput, protections, outputDirSuffix, outputAction, packer,
				projectModuleAction, postProcessAction);

		protected Task Run(string[] inputFileNames, string[] expectedOutput, SettingItem<Protection> protection,
			string outputDirSuffix = "", Action<string> outputAction = null, SettingItem<Packer> packer = null,
			Action<ProjectModule> projectModuleAction = null, Func<string, Task> postProcessAction = null) {
			var protections = (protection is null) ? Enumerable.Empty<SettingItem<Protection>>() : new[] { protection };
			return Run(inputFileNames, expectedOutput, protections, outputDirSuffix, outputAction, packer, projectModuleAction, postProcessAction);
		}

		protected async Task Run(string[] inputFileNames, string[] expectedOutput, IEnumerable<SettingItem<Protection>> protections,
			string outputDirSuffix = "", Action<string> outputAction = null, SettingItem<Packer> packer = null,
			Action<ProjectModule> projectModuleAction = null, Func<string, Task> postProcessAction = null) {

			var baseDir = Environment.CurrentDirectory;
			var outputDir = Path.Combine(baseDir, "obfuscated" + outputDirSuffix);
			if (Directory.Exists(outputDir)) {
				Directory.Delete(outputDir, true);
			}

			string firstFileName = GetFileName(inputFileNames[0]);
			string entryInputFileName = Path.Combine(baseDir, firstFileName);
			var entryOutputFileName = Path.Combine(outputDir, firstFileName);
			var proj = new ConfuserProject {
				BaseDirectory = baseDir,
				OutputDirectory = outputDir,
				Packer = packer
			};

			foreach (string name in inputFileNames) {
				var projectModule = new ProjectModule {
					Path = Path.Combine(baseDir, GetFileName(name)),
					IsExternal = IsExternal(name)
				};
				projectModuleAction?.Invoke(projectModule);
				proj.Add(projectModule);
			}

			var rule = new Rule();
			rule.AddRange(protections);
			if (rule.Count > 0)
				proj.Rules.Add(rule);

			var parameters = new ConfuserParameters {
				Project = proj,
				Logger = new XunitLogger(outputHelper, outputAction)
			};

			await ConfuserEngine.Run(parameters);

			for (var index = 0; index < inputFileNames.Length; index++) {
				string name = GetFileName(inputFileNames[index]);
				string outputName = Path.Combine(outputDir, name);

				bool exists;
				if (index == 0) {
					Assert.True(File.Exists(outputName));
					exists = true;
				}
				else {
					exists = File.Exists(outputName);
				}

				if (exists) {
					// Check if output assemblies is obfuscated
					Assert.NotEqual(FileUtilities.ComputeFileChecksum(Path.Combine(baseDir, name)),
						FileUtilities.ComputeFileChecksum(outputName));
				} else if (IsExternal(inputFileNames[index])) {
					File.Copy(
						Path.Combine(baseDir, GetFileName(inputFileNames[index])),
						Path.Combine(outputDir, GetFileName(inputFileNames[index])));
				}
			}

			if (Path.GetExtension(entryInputFileName) == ".exe") {
				var info = new ProcessStartInfo(entryOutputFileName) { RedirectStandardOutput = true, UseShellExecute = false };
				using (var process = Process.Start(info)) {
					var stdout = process.StandardOutput;
					Assert.Equal("START", await stdout.ReadLineAsync());

					foreach (string line in expectedOutput) {
						Assert.Equal(line, await stdout.ReadLineAsync());
					}

					Assert.Equal("END", await stdout.ReadLineAsync());
					Assert.Empty(await stdout.ReadToEndAsync());
					Assert.True(process.HasExited);
					Assert.Equal(42, process.ExitCode);
				}
			}

			if (!(postProcessAction is null))
				await postProcessAction.Invoke(outputDir);
		}

		private static string GetFileName(string name) {
			if (IsExternal(name))
				return name.Substring(_externalPrefix.Length);
			return name;
		}

		private static bool IsExternal(string name) => name.StartsWith(_externalPrefix, StringComparison.OrdinalIgnoreCase);
	}
}
