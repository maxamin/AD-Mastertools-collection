using System;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Analyzers {
	internal abstract class ContextAnalyzer {
		internal abstract Type TargetType();

		internal abstract void ProcessOperand(ScannedMethod method, Instruction instruction, object operand);
	}
}
