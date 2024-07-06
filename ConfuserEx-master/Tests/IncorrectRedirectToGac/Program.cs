using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace IncorrectRedirectToGac {
	public static class Program {
		public static int Main() {
			Console.WriteLine("START");
			var engine = new Engine();
			if (engine.BuildProjectFile("", null, null, null)) {
				Console.WriteLine("END");
			}

			return 42;
		}
	}

	class Engine : IBuildEngine5 {
		public bool IsRunningMultipleNodes => throw new NotImplementedException();

		public bool ContinueOnError => throw new NotImplementedException();

		public int LineNumberOfTaskNode => throw new NotImplementedException();

		public int ColumnNumberOfTaskNode => throw new NotImplementedException();

		public string ProjectFileOfTaskNode => throw new NotImplementedException();

		public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties,
			IDictionary targetOutputs, string toolsVersion) => true;

		public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties,
			IDictionary targetOutputs) => true;

		public BuildEngineResult BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames,
			IDictionary[] globalProperties, IList<string>[] removeGlobalProperties, string[] toolsVersion,
			bool returnTargetOutputs) =>
			throw new NotImplementedException();

		public bool BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames,
			IDictionary[] globalProperties, IDictionary[] targetOutputsPerProject, string[] toolsVersion,
			bool useResultsCache, bool unloadProjectsOnCompletion) =>
			throw new NotImplementedException();

		public object GetRegisteredTaskObject(object key, RegisteredTaskObjectLifetime lifetime) => throw new NotImplementedException();

		public void LogCustomEvent(CustomBuildEventArgs e) => throw new NotImplementedException();

		public void LogErrorEvent(BuildErrorEventArgs e) => throw new NotImplementedException();

		public void LogMessageEvent(BuildMessageEventArgs e) => throw new NotImplementedException();

		public void LogTelemetry(string eventName, IDictionary<string, string> properties) => throw new NotImplementedException();

		public void LogWarningEvent(BuildWarningEventArgs e) => throw new NotImplementedException();

		public void Reacquire() => throw new NotImplementedException();

		public void RegisterTaskObject(object key, object obj, RegisteredTaskObjectLifetime lifetime,
			bool allowEarlyCollection) =>
			throw new NotImplementedException();

		public object UnregisterTaskObject(object key, RegisteredTaskObjectLifetime lifetime) => throw new NotImplementedException();

		public void Yield() => throw new NotImplementedException();
	}
}
