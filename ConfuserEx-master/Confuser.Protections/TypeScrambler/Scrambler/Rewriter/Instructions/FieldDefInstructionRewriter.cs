using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Rewriter.Instructions {
	internal sealed class FieldDefInstructionRewriter : InstructionRewriter<FieldDef> {
		internal override void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, FieldDef operand) {
			Debug.Assert(service != null, $"{nameof(service)} != null");
			Debug.Assert(method != null, $"{nameof(method)} != null");
			Debug.Assert(body != null, $"{nameof(body)} != null");
			Debug.Assert(operand != null, $"{nameof(operand)} != null");
			Debug.Assert(index >= 0, $"{nameof(index)} >= 0");
			Debug.Assert(index < body.Count, $"{nameof(index)} < {nameof(body)}.Count");

			if (method.Module.IsClr40 && body[index].OpCode != OpCodes.Ldsfld && body[index].OpCode != OpCodes.Ldsflda && body[index].OpCode != OpCodes.Stsfld) {
				return;
			}

			var declType = service.GetItem(operand.DeclaringType);
			if (declType?.IsScambled == true) {
				body[index].Operand = new MemberRefUser(operand.Module, operand.Name, operand.FieldSig,
					declType.CreateGenericTypeSig(service.GetItem(method.DeclaringType)).ToTypeDefOrRef());
			}
		}
	}
}
