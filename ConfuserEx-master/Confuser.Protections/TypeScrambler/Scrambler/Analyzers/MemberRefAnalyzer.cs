using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Analyzers {
	internal sealed class MemberRefAnalyzer : ContextAnalyzer<MemberRef> {
		internal override void Process(ScannedMethod method, Instruction instruction, MemberRef operand) {
			Debug.Assert(method != null, $"{nameof(method)} != null");
			Debug.Assert(instruction != null, $"{nameof(instruction)} != null");
			Debug.Assert(operand != null, $"{nameof(operand)} != null");

			// Scrambling member references only works for constructors without parameters currently.
			if (instruction.OpCode != OpCodes.Newobj) return;
			if (operand.MethodSig.Params.Count > 0) return;

			TypeSig sig = null;
			if (operand.Class is TypeRef typeRef)
				sig = typeRef.ToTypeSig();

			if (operand.Class is TypeSpec typeSpec)
				sig = typeSpec.ToTypeSig();

			if (sig != null)
				method.RegisterGeneric(sig);
		}
	}
}
