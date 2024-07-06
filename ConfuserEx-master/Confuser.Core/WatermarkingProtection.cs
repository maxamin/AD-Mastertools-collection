using System.Linq;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core {
	public sealed class WatermarkingProtection : Protection {
		public const string _Id = "watermark";
		public const string _FullId = "Cx.Watermark";

		/// <inheritdoc />
		public override string Name => "Watermarking";

		/// <inheritdoc />
		public override string Description =>
			"This applies a watermark to the assembly, showing that ConfuserEx protected the assembly. So people try to reverse the obfuscation know to just give up.";

		/// <inheritdoc />
		public override string Id => _Id;

		/// <inheritdoc />
		public override string FullId => _FullId;

		/// <inheritdoc />
		protected internal override void Initialize(ConfuserContext context) { }

		/// <inheritdoc />
		protected internal override void PopulatePipeline(ProtectionPipeline pipeline) =>
			pipeline.InsertPostStage(PipelineStage.EndModule, new WatermarkingPhase(this));

		/// <inheritdoc />
		public override ProtectionPreset Preset => ProtectionPreset.None;

		private sealed class WatermarkingPhase : ProtectionPhase {
			/// <inheritdoc />
			public WatermarkingPhase(ConfuserComponent parent) : base(parent) { }

			/// <inheritdoc />
			public override ProtectionTargets Targets => ProtectionTargets.Modules;

			/// <inheritdoc />
			public override string Name => "Apply watermark";

			/// <inheritdoc />
			protected internal override void Execute(ConfuserContext context, ProtectionParameters parameters) {
				var marker = context.Registry.GetService<IMarkerService>();

				context.Logger.Debug("Watermarking...");
				foreach (var module in parameters.Targets.OfType<ModuleDef>()) {
					var attrRef = module.CorLibTypes.GetTypeRef("System", "Attribute");
					var attrType = module.FindNormal("ConfusedByAttribute");
					if (attrType == null) {
						attrType = new TypeDefUser("", "ConfusedByAttribute", attrRef);
						module.Types.Add(attrType);
						marker.Mark(attrType, Parent);
					}

					var ctor = attrType.FindInstanceConstructors()
						.FirstOrDefault(m => m.Parameters.Count == 1 && m.Parameters[0].Type == module.CorLibTypes.String);
					if (ctor == null) {
						ctor = new MethodDefUser(
							".ctor",
							MethodSig.CreateInstance(module.CorLibTypes.Void, module.CorLibTypes.String),
							MethodImplAttributes.Managed,
							MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName) {
							Body = new CilBody {MaxStack = 1}
						};
						ctor.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
						ctor.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(module, ".ctor",
							MethodSig.CreateInstance(module.CorLibTypes.Void), attrRef)));
						ctor.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
						attrType.Methods.Add(ctor);
						marker.Mark(ctor, Parent);
					}

					var attr = new CustomAttribute(ctor);
					attr.ConstructorArguments.Add(new CAArgument(module.CorLibTypes.String, ConfuserEngine.Version));

					module.CustomAttributes.Add(attr);
				}
			}
		}
	}
}
