using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Rewriter.Instructions {
	internal abstract class InstructionRewriter<T> : InstructionRewriter {
		internal override void ProcessInstruction(TypeService service, MethodDef method, IList<Instruction> body, ref int index, Instruction i) {
			ProcessOperand(service, method, body, ref index, (T)i.Operand);
		}
		internal override Type TargetType() => typeof(T);

		internal abstract void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, T operand);
	}
}
