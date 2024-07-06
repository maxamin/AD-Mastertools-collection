using System;
using System.Diagnostics;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML {
	sealed class BAMLPropertyReference : IBAMLReference {
		readonly ModuleDef _refModule;
		readonly PropertyRecord _rec;

		public BAMLPropertyReference(ModuleDef refModule, PropertyRecord rec) {
			_refModule = refModule;
			_rec = rec;
		}

		public bool CanRename(ModuleDef moduleDef, string oldName, string newName) => true;

		public void Rename(ModuleDef moduleDef, string oldName, string newName) {
			if (moduleDef != _refModule) return;

			var value = _rec.Value;
			while (true) {
				if (value.EndsWith(oldName, StringComparison.OrdinalIgnoreCase)) {
					value = value.Substring(0, value.Length - oldName.Length) + newName;
					_rec.Value = value;
				}
				else if (oldName.EndsWith(".baml", StringComparison.OrdinalIgnoreCase)) {
					oldName = ToXaml(oldName);
					newName = ToXaml(newName);
					continue;
				}

				break;
			}
		}

		private static string ToXaml(string refName) {
			Debug.Assert(refName.EndsWith(".baml"));
			return refName.Substring(0, refName.Length - 5) + ".xaml";
		}
	}
}
