using System.Text;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	public sealed class MemberRefReference : INameReference<IMemberDef> {
		readonly IMemberDef memberDef;
		readonly MemberRef memberRef;

		public bool ShouldCancelRename => false;

		public MemberRefReference(MemberRef memberRef, IMemberDef memberDef) {
			this.memberRef = memberRef;
			this.memberDef = memberDef;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (UTF8String.Equals(memberRef.Name, memberDef.Name)) return false;
			memberRef.Name = memberDef.Name;
			return true;
		}

		public override string ToString() => ToString(null); 

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("MemberRef Reference").Append("(");
			builder.Append("MemberRef").Append("(").AppendHashedIdentifier("Name", memberRef.Name).Append(")");
			builder.Append("; ");
			builder.AppendReferencedDef(memberDef, nameService);
			builder.Append(")");
			return builder.ToString();
		}
	}
}
