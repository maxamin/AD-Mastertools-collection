using System.Text;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	public sealed class TypeRefReference : INameReference<TypeDef> {
		readonly TypeDef typeDef;
		readonly TypeRef typeRef;

		public bool ShouldCancelRename => false;

		public TypeRefReference(TypeRef typeRef, TypeDef typeDef) {
			this.typeRef = typeRef;
			this.typeDef = typeDef;
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (UTF8String.Equals(typeRef.Namespace, typeDef.Namespace) && 
				UTF8String.Equals(typeRef.Name, typeDef.Name)) return false;

			typeRef.Namespace = typeDef.Namespace;
			typeRef.Name = typeDef.Name;
			return true;
		}

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("TypeRef Reference").Append("(");

			builder.Append("TypeRef").Append("(").AppendHashedIdentifier("Name", typeRef.FullName).Append(")");
			builder.Append("; ");
			builder.AppendReferencedType(typeDef, nameService);

			builder.Append(")");

			return builder.ToString();
		}
	}
}
