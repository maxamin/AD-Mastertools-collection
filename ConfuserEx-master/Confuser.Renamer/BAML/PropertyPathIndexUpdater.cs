using System;

namespace Confuser.Renamer.BAML {
	internal struct PropertyPathIndexUpdater {
		PropertyPathUpdater Parent { get; }
		int PathIndex { get; }
		int IndexerIndex { get; }
		IndexerParamInfo IndexInfo => Parent.PropertyPath[PathIndex].paramList[IndexerIndex];

		internal string ParenString {
			get => IndexInfo.parenString;
			set => Parent.UpdateParenString(PathIndex, IndexerIndex, value);
		}

		internal PropertyPathIndexUpdater(PropertyPathUpdater parent, int pathIndex, int indexerIndex) {
			Parent = parent ?? throw new ArgumentNullException(nameof(parent));
			PathIndex = pathIndex;
			IndexerIndex = indexerIndex;
		}
	}
}
