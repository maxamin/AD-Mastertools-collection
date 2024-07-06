using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using Confuser.Protections.TypeScrambler.Scrambler;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScrambler {
	internal sealed class TypeService {
		private Dictionary<IMemberDef, ScannedItem> GenericsMapper = new Dictionary<IMemberDef, ScannedItem>();

		internal bool ScrambledAnything => GenericsMapper.Any();

		internal void AddScannedItem(ScannedMethod m) => AddScannedItemGeneral(m);

		internal void AddScannedItem(ScannedType m) {
			//AddScannedItemGeneral(m);
		}

		private void AddScannedItemGeneral(ScannedItem m) {
			m.Scan();
			if (!GenericsMapper.ContainsKey(m.GetMemberDef())) {
				GenericsMapper.Add(m.GetMemberDef(), m);
			}
		}

		internal void PrepareItems() {
			foreach (var item in GenericsMapper.Values) {
				item.PrepareGenerics();
			}
		}

		private ScannedItem GetItemInternal(IMemberDef memberDef) {
			Debug.Assert(memberDef != null, $"{nameof(memberDef)} != null");

			if (GenericsMapper.TryGetValue(memberDef, out var item)) return item;
			return null;
		}

		internal ScannedMethod GetItem(MethodDef methodDef) => GetItemInternal(methodDef) as ScannedMethod;

		internal ScannedType GetItem(TypeDef typeDef) => GetItemInternal(typeDef) as ScannedType;
	}
}
