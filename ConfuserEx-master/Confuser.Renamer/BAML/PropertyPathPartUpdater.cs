using System;
using System.Collections.Generic;
using System.Linq;

namespace Confuser.Renamer.BAML {
	internal struct PropertyPathPartUpdater {
		PropertyPathUpdater Parent { get; }
		int PathIndex { get; }
		SourceValueInfo PathInfo => Parent.PropertyPath[PathIndex];

		internal IEnumerable<PropertyPathIndexUpdater> ParamList {
			get {
				var parent = Parent;
				var pathIndex = PathIndex;
				return Enumerable.Range(0, PathInfo.paramList.Count).Select(i => new PropertyPathIndexUpdater(parent, pathIndex, i));
			}
		}

		internal SourceValueType Type => PathInfo.type;

		internal string Name {
			get => PathInfo.name;
			set => Parent.Update(PathIndex, value);
		}

		internal PropertyPathPartUpdater(PropertyPathUpdater parent, int pathIndex) {
			Parent = parent ?? throw new ArgumentNullException(nameof(parent));
			PathIndex = pathIndex;
		}

		internal string GetTypeName() {
			var propertyName = PathInfo.name?.Trim();
			if (propertyName != null && propertyName.StartsWith("(") && propertyName.EndsWith(")")) {
				var indexOfDot = propertyName.LastIndexOf('.');
				if (indexOfDot < 0) return null;
				return propertyName.Substring(1, indexOfDot - 1);
			}
			return null;
		}

		internal string GetPropertyName() {
			switch (PathInfo.type) {
				case SourceValueType.Direct:
					return null;
				case SourceValueType.Property:
					var propertyName = PathInfo.name?.Trim();
					if (propertyName != null && propertyName.StartsWith("(") && propertyName.EndsWith(")")) {
						var indexOfDot = propertyName.LastIndexOf('.');
						if (indexOfDot < 0) return propertyName.Substring(1, propertyName.Length - 2);
						return propertyName.Substring(indexOfDot + 1, propertyName.Length - indexOfDot - 2);
					}
					return propertyName;
				case SourceValueType.Indexer:
					return "Item";
				default:
					throw new InvalidOperationException("Unexpected SourceValueType.");
			}
		}
	}
}
