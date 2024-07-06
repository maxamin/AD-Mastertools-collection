using System;
using System.Text;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	internal sealed class BAMLConverterTypeReference : INameReference<TypeDef> {
		readonly PropertyRecord propRec;
		readonly TypeSig sig;
		readonly TextRecord textRec;
		readonly BAMLAnalyzer.XmlNsContext xmlnsCtx;

		public bool ShouldCancelRename => false;

		public BAMLConverterTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, PropertyRecord rec) {
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			propRec = rec;
		}

		public BAMLConverterTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, TextRecord rec) {
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			textRec = rec;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			string name = sig.ReflectionName;
			string prefix = xmlnsCtx.GetPrefix(sig.ReflectionNamespace, sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			if (!string.IsNullOrEmpty(prefix))
				name = prefix + ":" + name;

			if (propRec != null) {
				if (string.Equals(propRec.Value, name, StringComparison.Ordinal)) return false;
				propRec.Value = name;
			}
			else {
				if (string.Equals(textRec.Value, name, StringComparison.Ordinal)) return false;
				textRec.Value = name;
			}

			return true;
		}

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("BAML Converter Type Reference").Append("(");
			if (propRec != null) {
				builder.Append("Property Record").Append("(").AppendHashedIdentifier("Value", propRec.Value).Append(")");
			}
			else {
				builder.Append("Text Record").Append("(").AppendHashedIdentifier("Value", propRec.Value).Append(")");
			}
			builder.Append("; ");
			builder.Append("Type Signature").Append("(").AppendHashedIdentifier("Name", sig.ReflectionFullName).Append(")");
			builder.Append(")");
			return builder.ToString();
		}
	}
}
