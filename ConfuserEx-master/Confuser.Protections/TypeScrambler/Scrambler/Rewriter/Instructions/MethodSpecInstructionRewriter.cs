using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Rewriter.Instructions {
	internal sealed class MethodSpecInstructionRewriter : InstructionRewriter<MethodSpec> {
		internal override void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, MethodSpec operand) {
			Debug.Assert(service != null, $"{nameof(service)} != null");
			Debug.Assert(method != null, $"{nameof(method)} != null");
			Debug.Assert(body != null, $"{nameof(body)} != null");
			Debug.Assert(operand != null, $"{nameof(operand)} != null");
			Debug.Assert(index >= 0, $"{nameof(index)} >= 0");
			Debug.Assert(index < body.Count, $"{nameof(index)} < {nameof(body)}.Count");

			var current = service.GetItem(method);
			if (operand.Method is MethodDef operandDef) {
				var operandScanned = service.GetItem(operandDef);
				if (operandScanned?.IsScambled == true) {
					operand.GenericInstMethodSig = operandScanned.CreateGenericMethodSig(current, service, operand.GenericInstMethodSig);
				}
			} else if (current?.IsScambled == true) {
				var generics = operand.GenericInstMethodSig.GenericArguments.Select(x => current.ConvertToGenericIfAvalible(x));
				operand.GenericInstMethodSig = new GenericInstMethodSig(generics.ToArray());
			}
		}
	}
}
