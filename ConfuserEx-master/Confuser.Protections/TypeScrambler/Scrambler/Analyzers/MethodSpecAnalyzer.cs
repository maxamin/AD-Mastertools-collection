using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Analyzers {
	internal sealed class MethodSpecAnalyzer : ContextAnalyzer<MethodSpec> {
		internal override void Process(ScannedMethod method, Instruction instruction, MethodSpec operand) {
			Debug.Assert(method != null, $"{nameof(method)} != null");
			Debug.Assert(instruction != null, $"{nameof(instruction)} != null");
			Debug.Assert(operand != null, $"{nameof(operand)} != null");

			foreach (var t in operand.GenericInstMethodSig.GenericArguments)
				method.RegisterGeneric(t);
		}
	}
}
