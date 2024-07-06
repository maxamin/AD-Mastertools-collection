using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScrambler.Scrambler {
	internal abstract class ScannedItem {
		private readonly List<TypeSig> _trueTypes;

		private IDictionary<TypeSig, GenericParam> Generics { get; }
		internal IReadOnlyList<TypeSig> TrueTypes => _trueTypes;

		private ushort GenericCount { get; set; }

		internal bool IsScambled => GenericCount > 0;

		protected ScannedItem(IGenericParameterProvider genericsProvider) {
			Debug.Assert(genericsProvider != null, $"{nameof(genericsProvider)} != null");

			GenericCount = 0;
			Generics = new Dictionary<TypeSig, GenericParam>(TypeEqualityComparer.Instance);
			_trueTypes = new List<TypeSig>();
		}

		internal bool RegisterGeneric(TypeSig t) {
			Debug.Assert(t != null, $"{nameof(t)} != null");

			// This is a temporary fix.
			// Type visibility should be handled in a much better way which would involved some analysis.
			var typeDef = t.ToTypeDefOrRef().ResolveTypeDef();
			if (typeDef != null && !typeDef.IsVisibleOutside())
				return false;

			// Get proper type.
			t = SignatureUtils.GetLeaf(t);

			if (!Generics.ContainsKey(t)) {
				GenericParam newGenericParam;
				if (t.IsGenericMethodParameter) {
					var mVar = t.ToGenericMVar();
					Debug.Assert(mVar != null, $"{nameof(mVar)} != null");
					newGenericParam = new GenericParamUser(GenericCount, mVar.GenericParam.Flags, $"T{GenericCount}") {
						Rid = mVar.Rid
					};
				}
				else if (t.IsGenericTypeParameter) {
					var tVar = t.ToGenericVar();
					Debug.Assert(tVar != null, $"{nameof(tVar)} != null");
					newGenericParam = new GenericParamUser(GenericCount, tVar.GenericParam.Flags, $"T{GenericCount}") {
						Rid = tVar.Rid
					};
				}
				else {
					newGenericParam = new GenericParamUser(GenericCount, GenericParamAttributes.NoSpecialConstraint, $"T{GenericCount}");
				}
				Generics.Add(t, newGenericParam);
				GenericCount++;
				_trueTypes.Add(t);
				return true;
			}
			else {
				return false;
			}
		}

		internal GenericSig GetGeneric(TypeSig t) {
			Debug.Assert(t != null, $"{nameof(t)} != null");

			t = SignatureUtils.GetLeaf(t);

			GenericSig result = null;
			if (Generics.TryGetValue(t, out var gp))
				result = this is ScannedType ? (GenericSig)new GenericVar(gp.Number) : new GenericMVar(gp.Number);

			return result;
		}

		internal TypeSig ConvertToGenericIfAvalible(TypeSig t) {
			Debug.Assert(t != null, $"{nameof(t)} != null");

			TypeSig newSig = GetGeneric(t);
			if (newSig != null) {
				// Now it may be that the signature contains lots of modifiers and signatures.
				// We need to process those... inside out.
				newSig = SignatureUtils.CopyModifiers(t, newSig);
			}

			return newSig ?? t;
		}

		internal void PrepareGenerics() => PrepareGenerics(Generics.Values.OrderBy(gp => gp.Number));

		protected abstract void PrepareGenerics(IEnumerable<GenericParam> scrambleParams);
		internal abstract IMemberDef GetMemberDef();

		internal abstract void Scan();
		internal abstract ClassOrValueTypeSig GetTarget();
	}
}
