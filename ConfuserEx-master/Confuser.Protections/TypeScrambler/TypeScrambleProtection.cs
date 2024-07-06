using System;
using Confuser.Core;

namespace Confuser.Protections.TypeScrambler {
	class TypeScrambleProtection : Protection {
		public override ProtectionPreset Preset => ProtectionPreset.None;

		public override string Name => "Type Scrambler";

		public override string Description => "Replaces types with generics";

		public override string Id => "typescramble";

		public override string FullId => "BahNahNah.typescramble";

		protected override void Initialize(ConfuserContext context) {
			if (context == null) throw new ArgumentNullException(nameof(context));

			context.Registry.RegisterService(FullId, typeof(TypeService), new TypeService());
		}

		protected override void PopulatePipeline(ProtectionPipeline pipeline) {
			if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

			pipeline.InsertPreStage(PipelineStage.Inspection, new AnalyzePhase(this));
			pipeline.InsertPostStage(PipelineStage.ProcessModule, new ScramblePhase(this));
		}
	}
}
