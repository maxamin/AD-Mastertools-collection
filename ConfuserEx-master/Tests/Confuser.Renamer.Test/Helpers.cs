using dnlib.DotNet;
using Moq;
using Xunit;

namespace Confuser.Renamer.Test {
	internal static class Helpers {
		internal static ModuleDefMD LoadTestModuleDef() {
			var asmResolver = new AssemblyResolver { EnableTypeDefCache = true };
			asmResolver.DefaultModuleContext = new ModuleContext(asmResolver);
			var options = new ModuleCreationOptions(asmResolver.DefaultModuleContext) {
				TryToLoadPdbFromDisk = false
			};

            asmResolver.AddToCache(ModuleDefMD.Load(typeof(Mock).Module, options));
            asmResolver.AddToCache(ModuleDefMD.Load(typeof(FactAttribute).Module, options));

            var thisModule = ModuleDefMD.Load(typeof(VTableTest).Module, options);
            asmResolver.AddToCache(thisModule);

			return thisModule;
		}
	}
}
