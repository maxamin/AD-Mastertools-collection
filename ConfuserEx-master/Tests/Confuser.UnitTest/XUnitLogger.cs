using System;
using Confuser.Core;
using Xunit.Abstractions;

namespace Confuser.UnitTest {
	public sealed class XunitLogger : ILogger {
		private readonly ITestOutputHelper _outputHelper;
		private readonly Action<string> _outputAction;

		public XunitLogger(ITestOutputHelper outputHelper) : this(outputHelper, null) { }

		public XunitLogger(ITestOutputHelper outputHelper, Action<string> outputAction) {
			_outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
			_outputAction = outputAction;
		}

		void ILogger.Debug(string msg) =>
			ProcessOutput("[DEBUG] " + msg);

		void ILogger.DebugFormat(string format, params object[] args) =>
			ProcessOutput("[DEBUG] " + format, args);

		void ILogger.EndProgress() { }

		void ILogger.Error(string msg) =>
			throw new Exception(msg);

		void ILogger.ErrorException(string msg, Exception ex) =>
			throw new Exception(msg, ex);

		void ILogger.ErrorFormat(string format, params object[] args) =>
			throw new Exception(string.Format(format, args));

		void ILogger.Finish(bool successful) =>
			ProcessOutput("[DONE]");

		void ILogger.Info(string msg) =>
			ProcessOutput("[INFO] " + msg);

		void ILogger.InfoFormat(string format, params object[] args) =>
			ProcessOutput("[INFO] " + format, args);

		void ILogger.Progress(int progress, int overall) { }

		void ILogger.Warn(string msg) =>
			ProcessOutput("[WARN] " + msg);

		void ILogger.WarnException(string msg, Exception ex) =>
			ProcessOutput("[WARN] " + msg + Environment.NewLine + ex.ToString());

		void ILogger.WarnFormat(string format, params object[] args) =>
			ProcessOutput("[WARN] " + format, args);

		private void ProcessOutput(string format, params object[] args) => 
			ProcessOutput(string.Format(format, args));

		private void ProcessOutput(string message) {
			_outputAction?.Invoke(message);
			_outputHelper.WriteLine(message);
		}
	}
}
