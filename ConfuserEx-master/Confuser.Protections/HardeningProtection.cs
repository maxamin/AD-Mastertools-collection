using System.Diagnostics.CodeAnalysis;
using Confuser.Core;

namespace Confuser.Protections {
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by reflection.")]
	internal sealed class HardeningProtection : Protection {
		/// <inheritdoc />
		public override string Name => "Protection Hardening";

		/// <inheritdoc />
		public override string Description => "This component improves the protection code, making it harder to circumvent it.";

		/// <inheritdoc />
		public override string Id => "harden";

		/// <inheritdoc />
		public override string FullId => "Cx.Harden";

		/// <inheritdoc />
		protected override void Initialize(ConfuserContext context) { }

		/// <inheritdoc />
		protected override void PopulatePipeline(ProtectionPipeline pipeline) => 
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new HardeningPhase(this));

		/// <inheritdoc />
		public override ProtectionPreset Preset => ProtectionPreset.Minimum;
	}
}
