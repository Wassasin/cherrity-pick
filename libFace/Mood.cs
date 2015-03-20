using System;

namespace libFace
{
	public struct Mood
	{
		public enum MoodValue
		{
			Positive,
			Negative
		}

		public int confidence;
		public MoodValue value;

		public static MoodValue parseMoodValue(string str)
		{
			switch (str) {
			case "Positive":
				return MoodValue.Positive;
			case "Negative":
				return MoodValue.Negative;
			default:
				throw new Exception ("Cannot parse MoodValue '" + str + "'");
			}
		}
	}
}

