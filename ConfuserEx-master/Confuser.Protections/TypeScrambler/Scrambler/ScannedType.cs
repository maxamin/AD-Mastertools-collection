using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScrambler.Scrambler {
	internal sealed class ScannedType : ScannedItem {
		internal TypeDef TargetType { get; private set; }

		public ScannedType(TypeDef target) : base(target) {
			Debug.Assert(target != null, $"{nameof(target)} != null");

			TargetType = target;
		}

		internal override void Scan() {
			if (!CanScrambleType(TargetType)) return;

			foreach (var field in TargetType.Fields)
				RegisterGeneric(field.FieldType);
		}

		private static bool CanScrambleType(TypeDef type) {
			// Enums don't work with generics.
			if (type.IsEnum) return false;

			// ComImports and PInvokes don't like generics.
			if (type.Methods.Any(x => x.IsPinvokeImpl)) return false;
			if (type.IsComImport()) return false;

			// Delegates are something that shouldn't be touched.
			if (type.IsDelegate) return false;

			// No entrypoints
			if (type.Methods.Any(x => x.Module.EntryPoint == x)) return false;

			if (type.IsValueType) return false;

			return true;
		}

		protected override void PrepareGenerics(IEnumerable<GenericParam> scrambleParams) {
			Debug.Assert(scrambleParams != null, $"{nameof(scrambleParams)} != null");
			if (!IsScambled) return;

			TargetType.GenericParameters.Clear();
			foreach (var generic in scrambleParams)
				TargetType.GenericParameters.Add(generic);

			foreach (var field in TargetType.Fields)
				field.FieldType = ConvertToGenericIfAvalible(field.FieldType);
		}

		internal GenericInstSig CreateGenericTypeSig(ScannedType from) => new GenericInstSig(GetTarget(), TrueTypes.ToList());

		internal override IMemberDef GetMemberDef() => TargetType;

		internal override ClassOrValueTypeSig GetTarget() => TargetType.ToTypeSig().ToClassOrValueTypeSig();
	}
}
