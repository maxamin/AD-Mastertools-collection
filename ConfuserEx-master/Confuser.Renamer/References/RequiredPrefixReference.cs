using System;
using System.Text;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References {
	public sealed class RequiredPrefixReference<T> : INameReference<T> where T : class, IDnlibDef {
		T Def { get; }
		string Prefix  { get; }

		/// <inheritdoc />
		public bool ShouldCancelRename => false;

		internal RequiredPrefixReference(T def, string prefix) {
			Def = def ?? throw new ArgumentNullException(nameof(def));
			Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
			if (prefix.Length < 0) throw new ArgumentException("Prefix must not be empty.", nameof(prefix));
		}

		/// <inheritdoc />
		public bool DelayRenaming(INameService service) => false;

		/// <inheritdoc />
		public bool UpdateNameReference(ConfuserContext context, INameService service) {
			if (Def.Name.StartsWith(Prefix, StringComparison.Ordinal)) return false;

			Def.Name = Prefix + Def.Name;
			return true;
		}

		public override string ToString() => ToString(null);

		public string ToString(INameService nameService) {
			var builder = new StringBuilder();
			builder.Append("Required Prefix").Append("(");
			builder.Append("Prefix").Append("(").Append(Prefix).Append(")");
			builder.Append("; ");
			builder.AppendReferencedDef(Def, nameService);
			builder.Append(")");
			return builder.ToString();
		}
	}
}
