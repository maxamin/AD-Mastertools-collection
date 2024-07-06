using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers {
	internal sealed class VisualBasicRuntimeAnalyzer : IRenamer {
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
			var typeDef = (def as TypeDef);
			if (typeDef != null) {
				AnalyzeType(context, service, parameters, typeDef);
			}
		}

		private static void AnalyzeType(ConfuserContext context, INameService service, ProtectionParameters parameters, TypeDef def) {
			if (IsEmbeddedAttribute(def) &&
				def.BaseType != null &&
				def.BaseType.FullName.Equals("System.Attribute", StringComparison.Ordinal)) {
				service.SetCanRename(def, false);
			} else if (def.HasCustomAttributes && def.CustomAttributes.Any(a => IsEmbeddedAttribute(a.AttributeType))) {
				service.SetCanRename(def, false);
			}
		}

		private static bool IsEmbeddedAttribute(ITypeDefOrRef defOrRef) {
			if (defOrRef.FullName.Equals("Microsoft.VisualBasic.Embedded", StringComparison.Ordinal)) {
				var typeDef = (defOrRef as TypeDef);
				if (typeDef != null) {
					return typeDef.IsNotPublic && 
						typeDef.BaseType != null && 
						typeDef.BaseType.FullName.Equals("System.Attribute", StringComparison.Ordinal);
				}
			}
			return false;
		}

		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
		}

		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
		}
	}
}
