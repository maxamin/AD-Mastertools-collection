using System;
using System.Diagnostics;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.BAML {
	sealed class BAMLStringReference : IBAMLReference {
		readonly ModuleDef _refModule;
		readonly Instruction _instr;

		public BAMLStringReference(ModuleDef refModule, Instruction instr) {
			_refModule = refModule;
			_instr = instr;
		}

		public bool CanRename(ModuleDef moduleDef, string oldName, string newName) {
			if (moduleDef != _refModule) return true; // Not relevant for renaming.

			return _instr.OpCode.Code == Code.Ldstr;
		}

		public void Rename(ModuleDef moduleDef, string oldName, string newName) {
			if (moduleDef != _refModule) return;

			var value = (string)_instr.Operand;
			while (true) {
				if (value.EndsWith(oldName, StringComparison.OrdinalIgnoreCase)) {
					value = value.Substring(0, value.Length - oldName.Length) + newName;
					_instr.Operand = value;
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
			Debug.Assert(refName.EndsWith(".baml", StringComparison.OrdinalIgnoreCase));
			return refName.Substring(0, refName.Length - 5) + ".xaml";
		}
	}
}
