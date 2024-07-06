using System;
using System.Collections.Generic;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.AntiTamper {
	internal static class AntiTamperExtensions {
		internal static void AddBeforeReloc(this List<PESection> sections, PESection newSection) {
			if (sections == null) throw new ArgumentNullException(nameof(sections));
			InsertBeforeReloc(sections, sections.Count, newSection);
		}

		internal static void InsertBeforeReloc(this List<PESection> sections, int preferredIndex, PESection newSection) {
			if (sections == null) throw new ArgumentNullException(nameof(sections));
			if (preferredIndex < 0 || preferredIndex > sections.Count) throw new ArgumentOutOfRangeException(nameof(preferredIndex), preferredIndex, "Preferred index is out of range.");
			if (newSection == null) throw new ArgumentNullException(nameof(newSection));

			var relocIndex = sections.FindIndex(0, Math.Min(preferredIndex + 1, sections.Count), IsRelocSection);
			if (relocIndex == -1)
				sections.Insert(preferredIndex, newSection);
			else
				sections.Insert(relocIndex, newSection);
		}

		private static bool IsRelocSection(PESection section) => 
			section.Name.Equals(".reloc", StringComparison.Ordinal);
	}
}
