using System.Text;
using dnlib.DotNet;

namespace Confuser.Renamer {
	internal static class ReferenceUtilities {
		internal static StringBuilder AppendDescription(this StringBuilder builder, IDnlibDef def, INameService nameService) {
			if (nameService is null)
				return builder.AppendHashedIdentifier("Name", def.FullName);
			
			builder.Append("Original Name").Append(": ");

			switch (def) {
				case TypeDef typeDef:
					builder.AppendTypeName(typeDef, nameService);
					break;
				case IMemberDef memberDef:
					builder.AppendTypeName(memberDef.DeclaringType, nameService)
						.Append("::")
						.AppendOriginalName(memberDef, nameService);
					break;
				default:
					builder.AppendOriginalName(def, nameService);
					break;
			}
			builder.Append("; ");
			return builder.AppendHashedIdentifier("Name", def.FullName);
		}

		private static StringBuilder AppendTypeName(this StringBuilder builder, TypeDef typeDef, INameService nameService) {
			var originalNamespace = nameService.GetOriginalNamespace(typeDef);
			var originalName = nameService.GetOriginalName(typeDef);

			if (string.IsNullOrWhiteSpace(originalNamespace))
				originalNamespace = typeDef.Namespace;
			if (string.IsNullOrWhiteSpace(originalName))
				originalName = typeDef.Name;

			if (!string.IsNullOrWhiteSpace(originalNamespace))
				builder.Append(originalNamespace).Append(".");

			return builder.Append(originalName);
		}

		private static StringBuilder AppendOriginalName(this StringBuilder builder, IDnlibDef def, INameService nameService) {
			var originalName = nameService.GetOriginalName(def);
			if (string.IsNullOrWhiteSpace(originalName))
				originalName = def.Name;
			return builder.Append(originalName);
		}

		internal static StringBuilder AppendReferencedDef(this StringBuilder builder, IDnlibDef def, INameService nameService) {
			switch (def) {
				case EventDef eventDef:
					return builder.AppendReferencedEvent(eventDef, nameService);
				case FieldDef fieldDef:
					return builder.AppendReferencedField(fieldDef, nameService);
				case MethodDef methodDef:
					return builder.AppendReferencedMethod(methodDef, nameService);
				case PropertyDef propDef:
					return builder.AppendReferencedProperty(propDef, nameService);
				case TypeDef typeDef:
					return builder.AppendReferencedType(typeDef, nameService);
				default:
					return builder.Append("Referenced Definition").Append("(").AppendDescription(def, nameService).Append(")");
			}
		}

		internal static StringBuilder AppendReferencedEvent(this StringBuilder builder, EventDef eventDef, INameService nameService) =>
			builder.Append("Referenced Event").Append("(").AppendDescription(eventDef, nameService).Append(")");

		internal static StringBuilder AppendReferencedField(this StringBuilder builder, FieldDef fieldDef, INameService nameService) =>
			builder.Append("Referenced Method").Append("(").AppendDescription(fieldDef, nameService).Append(")");

		internal static StringBuilder AppendReferencedMethod(this StringBuilder builder, MethodDef methodDef, INameService nameService) =>
			builder.Append("Referenced Method").Append("(").AppendDescription(methodDef, nameService).Append(")");

		internal static StringBuilder AppendReferencedProperty(this StringBuilder builder, PropertyDef propertyDef, INameService nameService) =>
			builder.Append("Referenced Property").Append("(").AppendDescription(propertyDef, nameService).Append(")");

		internal static StringBuilder AppendReferencedType(this StringBuilder builder, TypeDef typeDef, INameService nameService) =>
			builder.Append("Referenced Type").Append("(").AppendDescription(typeDef, nameService).Append(")");

		internal static StringBuilder AppendHashedIdentifier(this StringBuilder builder, string descriptor, object value) =>
			builder.Append(descriptor).Append(" Hash: ").AppendFormat("{0:X}", value.GetHashCode());
	}
}
