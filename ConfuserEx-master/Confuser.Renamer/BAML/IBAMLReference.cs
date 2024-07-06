using System;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML {
	internal interface IBAMLReference {
		bool CanRename(ModuleDef moduleDef, string oldName, string newName);
		void Rename(ModuleDef moduleDef, string oldName, string newName);
	}
}
