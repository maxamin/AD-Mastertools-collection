using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections {
	internal sealed class HardeningPhase : ProtectionPhase {
		//private new HardeningComponent Parent => (HardeningComponent)base.Parent;

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
		public HardeningPhase(HardeningProtection parent) : base(parent) { }

		/// <inheritdoc />
		public override ProtectionTargets Targets => ProtectionTargets.Modules;

		/// <inheritdoc />
		public override string Name => "Hardening Phase";

		/// <inheritdoc />
		public override bool ProcessAll => true;

		/// <inheritdoc />
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters) {
			foreach (var module in parameters.Targets.OfType<ModuleDef>())
				HardenMethod(context, module);
		}

		private static void HardenMethod(ConfuserContext context, ModuleDef module) {
			var cctor = module.GlobalType.FindStaticConstructor();
			if (cctor == null) {
				context.Logger.Debug("No .cctor containing protection code found. Nothing to do.");
				return;
			}

			if (!cctor.HasBody || !cctor.Body.HasInstructions) return;

			var marker = context.Registry.GetService<IMarkerService>();
			var instructions = cctor.Body.Instructions;
			for (var i = instructions.Count - 1; i >= 0; i--) {
				if (instructions[i].OpCode.Code != Code.Call) continue;
				if (!(instructions[i].Operand is MethodDef targetMethod)) continue;
				if (!targetMethod.IsStatic || targetMethod.DeclaringType != module.GlobalType) continue;
				if (!marker.IsMarked(targetMethod) || !(marker.GetHelperParent(targetMethod) is Protection protection)) continue;

				// Resource protection needs to rewrite the method during the write phase. Not compatible!
				if (protection.FullId.Equals(ResourceProtection._FullId)) continue; 

				cctor.Body.MergeCall(instructions[i]);
				targetMethod.DeclaringType.Methods.Remove(targetMethod);
			}
		}
	}
}
