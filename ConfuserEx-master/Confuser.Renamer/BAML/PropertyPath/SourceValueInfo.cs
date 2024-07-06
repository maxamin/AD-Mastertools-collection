using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confuser.Renamer.BAML {
	internal struct SourceValueInfo {
		public SourceValueType type;
		public DrillIn drillIn;
		public string name;                 // the name the user supplied - could be "(0)"
		public IReadOnlyList<IndexerParamInfo> paramList;    // params for indexer

		public SourceValueInfo(SourceValueType t, DrillIn d, string n) {
			type = t;
			drillIn = d;
			name = n;
			paramList = null;
		}

		public SourceValueInfo(SourceValueType t, DrillIn d, IReadOnlyList<IndexerParamInfo> list) {
			type = t;
			drillIn = d;
			name = null;
			paramList = list;
		}
	}
}
