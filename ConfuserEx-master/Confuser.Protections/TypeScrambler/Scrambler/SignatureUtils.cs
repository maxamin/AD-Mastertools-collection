using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScrambler.Scrambler {
	internal static class SignatureUtils {
		internal static TypeSig GetLeaf(TypeSig t) {
			Debug.Assert(t != null, $"{nameof(t)} != null");

			while (t is NonLeafSig nonLeafSig)
				t = nonLeafSig.Next;

			return t;
		}

		internal static TypeSig CopyModifiers(TypeSig from, TypeSig to) {
			if (from is NonLeafSig current) {
				// There are additional signatures. Store all of the in a stack and process them one by one.
				var sigStack = new Stack<NonLeafSig>();
				while (current != null) {
					sigStack.Push(current);
					current = current.Next as NonLeafSig;
				}

				// Now process the entries on the stack one by one.
				while (sigStack.Any()) {
					current = sigStack.Pop();
					if (current is SZArraySig arraySig)
						to = new ArraySig(to, arraySig.Rank, arraySig.GetSizes(), arraySig.GetLowerBounds());
					else if (current is ByRefSig)
						to = new ByRefSig(to);
					else if (current is CModReqdSig cModReqdSig)
						to = new CModReqdSig(cModReqdSig.Modifier, to);
					else if (current is CModOptSig cModOptSig)
						to = new CModOptSig(cModOptSig.Modifier, to);
					else if (current is PtrSig)
						to = new PtrSig(to);
					else if (current is PinnedSig)
						to = new PinnedSig(to);
					else
						Debug.Fail("Unexpected leaf signature: " + current.GetType().FullName);
				}
			}

			return to;
		}
	}
}
