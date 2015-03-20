using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace libengagement
{
	public class VerdictMapsParser
	{
		private static VerdictMap ParseVerdictMap(JToken verdictmap)
		{
			var result = new VerdictMap ();

			result.verdict = verdictmap ["name"].Value<string> ();

			foreach (var subject in verdictmap["subjects"].Children())
			{
				var v = subject ["subject"].Value<string> ();
				var w = subject ["weight"].Value<float> ();
				result.maps.Add (v, w);
			}

			return result;
		}

		public static List<VerdictMap> Parse(string str)
		{
			var results = new List<VerdictMap> ();

			var doc = JObject.Parse (str);
			foreach (var charity in doc["charities"].Children())
				results.Add (ParseVerdictMap (charity));

			return results;
		}
	}
}

