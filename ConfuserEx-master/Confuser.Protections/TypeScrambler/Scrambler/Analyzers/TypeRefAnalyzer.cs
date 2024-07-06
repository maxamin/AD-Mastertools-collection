using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Analyzers {
	internal sealed class TypeRefAnalyzer : ContextAnalyzer<TypeRef> {
		internal override void Process(ScannedMethod method, Instruction instruction, TypeRef operand) {
			Debug.Assert(method != null, $"{nameof(method)} != null");
			Debug.Assert(instruction != null, $"{nameof(instruction)} != null");
			Debug.Assert(operand != null, $"{nameof(operand)} != null");

			method.RegisterGeneric(operand.ToTypeSig());
		}
	}
}
