using System.Collections.Generic;

namespace InheritCustomAttr.ExtReference {
	interface IExt<TKey, TValue> : IExt, IDictionary<TKey, TValue> {
	}
}
