﻿using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns {
	/// <summary>
	///     A function that indicate the visibility of members.
	/// </summary>
	public class IsPublicFunction : PatternFunction {
		internal const string FnName = "is-public";

		/// <inheritdoc />
		public override string Name {
			get { return FnName; }
		}

		/// <inheritdoc />
		public override int ArgumentCount {
			get { return 0; }
		}

		/// <inheritdoc />
		public override object Evaluate(IDnlibDef definition) {
			var member = definition as IMemberDef;
			if (member == null)
				return false;

			var declType = ((IMemberDef)definition).DeclaringType;
			while (declType != null) {
				if (!declType.IsPublic)
					return false;
				declType = declType.DeclaringType;
			}

			if (member is MethodDef)
				return ((MethodDef)member).IsVisibleOutside();
			if (member is FieldDef)
				return ((FieldDef)member).IsVisibleOutside();
			if (member is PropertyDef)
				return ((PropertyDef)member).IsVisibleOutside();
			if (member is EventDef)
				return ((EventDef)member).IsVisibleOutside();
			if (member is TypeDef)
				return ((TypeDef)member).IsVisibleOutside();

			throw new NotSupportedException();
		}
	}
}
