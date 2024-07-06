using System;
using System.Text;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	internal sealed class BAMLTypeReference : INameReference<TypeDef> {
		readonly TypeInfoRecord rec;
		readonly TypeSig sig;

		public bool ShouldCancelRename => false;

		public BAMLTypeReference(TypeSig sig, TypeInfoRecord rec) {
			this.sig = sig;
			this.rec = rec;
		}

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (string.Equals(rec.TypeFullName, sig.ReflectionFullName, StringComparison.Ordinal)) return false;
			rec.TypeFullName = sig.ReflectionFullName;
			return true;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("BAML Type Reference").Append("(");
			builder.Append("Type Info Record").Append("(").AppendHashedIdentifier("Name", rec.TypeFullName).Append(")");
			builder.Append("; ");
			builder.Append("Type Signature").Append("(").AppendHashedIdentifier("Name", sig.ReflectionFullName).Append(")");
			builder.Append(")");
			return builder.ToString();
		}
	}
}
