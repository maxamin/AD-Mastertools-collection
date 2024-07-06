using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confuser.Renamer.BAML {
	internal struct IndexerParamInfo {
		// parse each indexer param "(abc)xyz" into two pieces - either can be empty
		public string parenString;
		public string valueString;

		public IndexerParamInfo(string paren, string value) {
			parenString = paren;
			valueString = value;
		}
	}
}
