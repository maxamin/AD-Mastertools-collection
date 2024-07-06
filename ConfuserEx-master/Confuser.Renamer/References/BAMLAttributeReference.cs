using System.Text;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	internal sealed class BAMLAttributeReference : INameReference<IDnlibDef> {
		readonly AttributeInfoRecord attrRec;
		readonly IDnlibDef member;
		readonly PropertyRecord propRec;

		public bool ShouldCancelRename => false;

		public BAMLAttributeReference(IDnlibDef member, AttributeInfoRecord rec) {
			this.member = member;
			attrRec = rec;
		}

		public BAMLAttributeReference(IDnlibDef member, PropertyRecord rec) {
			this.member = member;
			propRec = rec;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (attrRec != null) {
				if (UTF8String.Equals(attrRec.Name, member.Name)) return false;
				attrRec.Name = member.Name;
			}
			else {
				if (UTF8String.Equals(propRec.Value, member.Name)) return false;
				propRec.Value = member.Name;
			}
			return true;
		}

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("BAML Attribute Reference").Append("(");
			if (attrRec != null) {
				builder.Append("Attribute Info Record").Append("(");
				builder.AppendHashedIdentifier("Name", attrRec.Name);
				builder.Append(")");
			}
			if (propRec != null) {
				builder.Append("Property Record").Append("(");
				builder.AppendHashedIdentifier("Value", propRec.Value);
				builder.Append(")");
			}
			builder.Append("; ");
			builder.AppendReferencedDef(member, nameService);
			builder.Append(")");

			return builder.ToString();
		}
	}
}
