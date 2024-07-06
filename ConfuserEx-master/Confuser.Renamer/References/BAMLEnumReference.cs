using System.Text;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	internal sealed class BAMLEnumReference : INameReference<FieldDef> {
		readonly FieldDef enumField;
		readonly PropertyRecord rec;

		public bool ShouldCancelRename => false;

		public BAMLEnumReference(FieldDef enumField, PropertyRecord rec) {
			this.enumField = enumField;
			this.rec = rec;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (UTF8String.Equals(rec.Value, enumField.Name)) return false;
			rec.Value = enumField.Name;
			return true;
		}

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("BAML Enum Reference").Append("(");
			builder.Append("Property Record").Append("(").AppendHashedIdentifier("Value", rec.Value).Append(")");
			builder.Append("; ");
			builder.AppendReferencedField(enumField, nameService);
			builder.Append(")");
			return builder.ToString();
		}
	}
}
