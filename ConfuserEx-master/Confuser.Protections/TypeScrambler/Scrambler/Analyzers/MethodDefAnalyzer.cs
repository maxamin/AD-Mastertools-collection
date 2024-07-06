using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Analyzers {
	internal sealed class MethodDefAnalyzer : ContextAnalyzer<MethodDef> {
		private TypeService Service { get; }

		internal MethodDefAnalyzer(TypeService service) {
			Debug.Assert(service != null, $"{nameof(service)} != null");

			Service = service;
		}
		internal override void Process(ScannedMethod method, Instruction instruction, MethodDef operand) {
			Debug.Assert(method != null, $"{nameof(method)} != null");
			Debug.Assert(instruction != null, $"{nameof(instruction)} != null");
			Debug.Assert(operand != null, $"{nameof(operand)} != null");

			var sc = Service.GetItem(operand);
			if (sc?.IsScambled == true)
				foreach (var regTypes in sc.TrueTypes)
					method.RegisterGeneric(regTypes);
		}
	}
}
