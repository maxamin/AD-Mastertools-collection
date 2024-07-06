using System.Text;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	public sealed class CAMemberReference : INameReference<IDnlibDef> {
		readonly IDnlibDef definition;
		readonly CANamedArgument namedArg;

		public bool ShouldCancelRename => false;

		public CAMemberReference(CANamedArgument namedArg, IDnlibDef definition) {
			this.namedArg = namedArg;
			this.definition = definition;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (UTF8String.Equals(namedArg.Name, definition.Name)) return false;
			namedArg.Name = definition.Name;
			return true;
		}

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("Custom Argument Reference").Append("(");
			builder.Append("CA Argument").Append("(").AppendHashedIdentifier("Name", namedArg.Name).Append(")");
			builder.Append("; ");
			builder.AppendReferencedDef(definition, nameService);
			builder.Append(")");
			return builder.ToString();
		}
	}
}
