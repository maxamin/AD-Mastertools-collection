using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Rewriter.Instructions {
	internal abstract class InstructionRewriter {
		internal abstract void ProcessInstruction(TypeService service, MethodDef method, IList<Instruction> body, ref int index, Instruction i);
		internal abstract Type TargetType();
	}
}
