using System;
using System.Collections.Generic;

namespace libengagement
{
	public struct VerdictMap
	{
		public string verdict;
		public Dictionary<string, float> maps; // Subject -> ratio
	}
}

