using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScrambler.Scrambler.Rewriter.Instructions {
	internal sealed class InstructionRewriterFactory : IEnumerable<InstructionRewriter> {
		private IDictionary<Type, InstructionRewriter> RewriterDefinitions { get; } 
			= new Dictionary<Type, InstructionRewriter>();

		internal void Add(InstructionRewriter i) {
			Debug.Assert(i != null, $"{nameof(i)} != null");

			RewriterDefinitions.Add(i.TargetType(), i);
		}

		internal void Process(TypeService service, MethodDef method, IList<Instruction> instructions, ref int index) {
			Debug.Assert(service != null, $"{nameof(service)} != null");
			Debug.Assert(method != null, $"{nameof(method)} != null");
			Debug.Assert(instructions != null, $"{nameof(instructions)} != null");
			Debug.Assert(index >= 0, $"{nameof(index)} >= 0");
			Debug.Assert(index < instructions.Count, $"{nameof(index)} < {nameof(instructions)}.Count");

			Instruction current = instructions[index];
			if (current.Operand == null) return;

			var currentRefType = current.Operand.GetType();
			while (currentRefType != typeof(object)) {
				if (RewriterDefinitions.TryGetValue(currentRefType, out var rw)) {
					rw.ProcessInstruction(service, method, instructions, ref index, current);
					break;
				}
				currentRefType = currentRefType.BaseType;
			}
		}

		public IEnumerator<InstructionRewriter> GetEnumerator() => RewriterDefinitions.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
