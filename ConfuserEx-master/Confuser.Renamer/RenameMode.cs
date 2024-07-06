using System;

namespace Confuser.Renamer {
	public enum RenameMode {
		Empty = 0x0,
		Unicode = 0x1,
		// ReSharper disable once InconsistentNaming
		ASCII = 0x2,
		/// <summary>
		/// This the the rename mode with the largest set of possible characters,
		/// that is still save for reflection.
		/// </summary>
		Reflection = 0x3,
		Letters = 0x4,

		Decodable = 0x10,
		Sequential = 0x11,
		Reversible = 0x12,

		/// <summary>Add a underscore to the name to mark that it would be renamed.</summary>
		Debug = 0x20,

		/// <summary>Keep the names as they are.</summary>
		Retain = Int32.MaxValue
	}
}
