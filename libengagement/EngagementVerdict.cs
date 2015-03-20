using System;

namespace libengagement
{
	public struct EngagementVerdict : IComparable<EngagementVerdict>
	{
		public string verdict;
		public float engagement;

		public int CompareTo(EngagementVerdict obj) {
			int ec = engagement.CompareTo (obj.engagement);
			if (ec != 0)
				return ec;

			return verdict.CompareTo (obj.verdict);
		}
	}
}

