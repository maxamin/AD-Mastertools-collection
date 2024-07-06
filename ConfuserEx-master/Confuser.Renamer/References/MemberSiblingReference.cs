using System;
using System.Diagnostics;
using System.Text;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	public sealed class MemberSiblingReference : INameReference<IDnlibDef> {
		IMemberDef _oldestSiblingDef;
		public IMemberDef ThisMemberDef { get; }

		public IMemberDef OldestSiblingDef {
			get => _oldestSiblingDef;
			set {
				Debug.Assert(!ReferenceEquals(ThisMemberDef, value));
				_oldestSiblingDef = value;
			}
		}

		public MemberSiblingReference(IMemberDef thisMemberDef, IMemberDef oldestSiblingDef) {
			ThisMemberDef = thisMemberDef ?? throw new ArgumentNullException(nameof(thisMemberDef));
			OldestSiblingDef = oldestSiblingDef ?? throw new ArgumentNullException(nameof(oldestSiblingDef));
			Debug.Assert(!ReferenceEquals(ThisMemberDef, OldestSiblingDef));
		}

		/// <inheritdoc />
		public bool ShouldCancelRename => false;

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => !service.IsRenamed(OldestSiblingDef);

		/// <inheritdoc />
		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (UTF8String.Equals(ThisMemberDef.Name, OldestSiblingDef.Name)) return false;
			ThisMemberDef.Name = OldestSiblingDef.Name;
			return true;
		}

		public override string ToString() => ToString(null);
		
		/// <inheritdoc />
		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("Member Sibling Reference").Append("(");
			builder.Append("This ").AppendReferencedDef(ThisMemberDef, nameService);
			builder.Append("; ");
			builder.Append("Oldest Sibling ").AppendReferencedDef(OldestSiblingDef, nameService);
			builder.Append(")");
			return builder.ToString();
		}
	}
}
