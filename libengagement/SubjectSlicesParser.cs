using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace libengagement
{
	public class SubjectSlicesParser
	{
		private static float ParseTime(string str)
		{
			string[] parts = str.Split(new char[]{'m', 's'});
			float minutes = float.Parse (parts [0]);
			float seconds = float.Parse (parts [1]);

			return minutes * 60 + seconds;
		}

		private static List<SubjectSlice> ParseSlice(JToken slice)
		{
			var result = new List<SubjectSlice> ();

			float start = ParseTime (slice ["start"].Value<string>());
			float end = ParseTime (slice ["end"].Value<string>());

			foreach (var subject in slice["subjects"].Children()) {
				var ss = new SubjectSlice ();

				ss.slice.start = start;
				ss.slice.end = end;

				ss.subject = subject ["subject"].Value<string> ();
				ss.weight = subject ["weight"].Value<float> ();

				result.Add (ss);
			}

			return result;
		}

		public static List<SubjectSlice> Parse(string str)
		{
			var results = new List<SubjectSlice> ();

			var doc = JObject.Parse (str);
			foreach (var section in doc["sections"].Children())
				results.AddRange (ParseSlice (section));

			return results;
		}
	}
}

