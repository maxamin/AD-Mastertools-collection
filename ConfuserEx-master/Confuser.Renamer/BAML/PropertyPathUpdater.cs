using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confuser.Renamer.BAML {
	/// <summary>
	/// This class is used to update parts of an property path based on the references.
	/// </summary>
	internal sealed class PropertyPathUpdater : IEnumerable<PropertyPathPartUpdater> {
		internal SourceValueInfo[] PropertyPath { get; }
		Action<string> UpdateAction { get; }

		internal PropertyPathUpdater(string path, Action<string> updateAction) {
			UpdateAction = updateAction ?? throw new ArgumentNullException(nameof(updateAction));
			var pathParser = new PropertyPathParser();
			PropertyPath = pathParser.Parse(path);
		}

		internal void Update(int index, string value) {
			PropertyPath[index].name = value;
			RebuildAndUpdatePath();
		}

		public void UpdateParenString(int pathIndex, int indexerIndex, string value) {
			var originalParams = PropertyPath[pathIndex].paramList;
			var newIndexParams = originalParams.ToArray();
			newIndexParams[indexerIndex].parenString = value;
			PropertyPath[pathIndex].paramList = newIndexParams;
			RebuildAndUpdatePath();
		}

		private void RebuildAndUpdatePath() {
			var builder = new StringBuilder();
			foreach (var sourceValueInfo in PropertyPath) {
				if (builder.Length > 0) builder.Append('.');
				builder.Append(sourceValueInfo.name);
				if (sourceValueInfo.paramList?.Count > 0) {
					builder.Append('[');
					builder.Append(sourceValueInfo.paramList[0].parenString);
					builder.Append(sourceValueInfo.paramList[0].valueString);
					for (int i = 1; i < sourceValueInfo.paramList.Count; i++) {
						builder.Append(',');
						builder.Append(sourceValueInfo.paramList[i].parenString);
						builder.Append(sourceValueInfo.paramList[i].valueString);
					}
					builder.Append(']');
				}
			}

			UpdateAction.Invoke(builder.ToString());
		}

		/// <inheritdoc />
		public IEnumerator<PropertyPathPartUpdater> GetEnumerator() =>
			Enumerable.Range(0, PropertyPath.Length).Select(i => new PropertyPathPartUpdater(this, i)).GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
