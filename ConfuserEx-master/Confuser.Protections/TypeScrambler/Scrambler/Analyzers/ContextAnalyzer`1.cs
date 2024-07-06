using System;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Analyzers {
	internal abstract class ContextAnalyzer<T> : ContextAnalyzer {
		internal override Type TargetType() => typeof(T);
		internal abstract void Process(ScannedMethod method, Instruction instruction, T operand);
		internal override void ProcessOperand(ScannedMethod method, Instruction instruction, object operand) => 
			Process(method, instruction, (T)operand);
	}
}
