using System;
using System.Text;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	internal sealed class BAMLConverterMemberReference : INameReference<IDnlibDef> {
		readonly IMemberDef member;
		readonly PropertyRecord rec;
		readonly TypeSig sig;
		readonly BAMLAnalyzer.XmlNsContext xmlnsCtx;

		public bool ShouldCancelRename => false;

		public BAMLConverterMemberReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, IMemberDef member, PropertyRecord rec) {
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.member = member;
			this.rec = rec;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			string typeName = sig.ReflectionName;
			string prefix = xmlnsCtx.GetPrefix(sig.ReflectionNamespace, sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			if (!string.IsNullOrEmpty(prefix))
				typeName = prefix + ":" + typeName;
			var newValue = typeName + "." + member.Name;
			if (string.Equals(rec.Value, newValue, StringComparison.Ordinal)) return false;
			rec.Value = newValue;
			return true;
		}

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("BAML Converter Member Reference").Append("(");
			builder.Append("Property Record").Append("(").AppendHashedIdentifier("Value", rec.Value).Append(")");
			builder.Append("; ");
			builder.Append("Type Signature").Append("(").AppendHashedIdentifier("Name", sig.ReflectionFullName).Append(")");
			builder.Append("; ");
			builder.AppendReferencedDef(member, nameService);
			builder.Append(")");
			return builder.ToString();
		}
	}
}
