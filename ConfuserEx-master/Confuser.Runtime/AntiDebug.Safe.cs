using System;
using System.Diagnostics;
using System.Threading;

namespace Confuser.Runtime {
	internal static class AntiDebugSafe {
		static void Initialize() {
			const string x = "COR";
			var env = typeof(Environment);
			var method = env.GetMethod("GetEnvironmentVariable", new[] { typeof(string) });

			// Comparison is done using is-operator to avoid the op_inequality overload of .NET 4.0
			// This is required to ensure that the result is .NET 2.0 compatible.
			if (!(method is null) &&
			    "1".Equals(method.Invoke(null, new object[] { x + "_ENABLE_PROFILING" })))
				Environment.FailFast(null);

			var thread = new Thread(Worker);
			thread.IsBackground = true;
			thread.Start(null);
		}

		static void Worker(object thread) {
			if (!(thread is Thread th)) {
				th = new Thread(Worker);
				th.IsBackground = true;
				th.Start(Thread.CurrentThread);
				Thread.Sleep(500);
			}
			while (true) {
				if (Debugger.IsAttached || Debugger.IsLogging())
					Environment.FailFast(null);

				if (!th.IsAlive)
					Environment.FailFast(null);

				Thread.Sleep(1000);
			}
		}
	}
}
