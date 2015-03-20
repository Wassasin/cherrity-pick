using System;

namespace libengagement
{
	public struct Slice
	{
		public float start, end;

		public float Length()
		{
			return end - start;
		}
	}
}

