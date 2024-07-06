using System;
using System.Collections.Generic;
using System.Text;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	/// <summary>
	/// This is the reverse reference to <see cref="MemberSiblingReference"/>.
	/// It's required to detect complex inheritance situations,
	/// where multiple sets of overlapping siblings are declared.
	/// </summary>
	/// <remarks>This reference does not perform any renaming itself.</remarks>
	public sealed class MemberOldestSiblingReference : INameReference<IDnlibDef> {
		public IMemberDef OldestSiblingDef { get; set; }
		public IList<IMemberDef> OtherSiblings { get; }

		public MemberOldestSiblingReference(IMemberDef oldestSiblingDef, IMemberDef otherSiblingDef) {
			OldestSiblingDef = oldestSiblingDef ?? throw new ArgumentNullException(nameof(oldestSiblingDef));
			if (otherSiblingDef is null) throw new ArgumentNullException(nameof(otherSiblingDef));
			OtherSiblings = new List<IMemberDef> {otherSiblingDef};
		}

		/// <inheritdoc />
		public bool ShouldCancelRename => false;

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		/// <inheritdoc />
		public bool UpdateNameReference(ConfuserContext context, INameService service) => false;

		public override string ToString() => ToString(null);
		
		/// <inheritdoc />
		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("Oldest Sibling Reference").Append("(");
			builder.Append("Oldest Sibling ").AppendReferencedDef(OldestSiblingDef, nameService);
			builder.Append("; Other Siblings: ");
			foreach (var otherSibling in OtherSiblings) 
				builder.AppendReferencedDef(otherSibling, nameService).Append(", ");

			builder.Length -= 2;
			builder.Append(")");
			return builder.ToString();
		}
	}
}
