using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace tests
{
	[TestFixture]
	public class BlackboxTest
	{
		static libengagement.EngagementSlice CreateEngagementSlice(float start, float end, float engagement)
		{
			var slice = new libengagement.Slice ();
			slice.start = start;
			slice.end = end;
			var es = new libengagement.EngagementSlice ();
			es.slice = slice;
			es.engagement = engagement;
			return es;
		}

		[Test]
		public void Everything()
		{
			var b = new libengagement.Blackbox();

			string sssjson = System.IO.File.ReadAllText ("../../../movie/SampleMovie1.json");
			var sss = libengagement.SubjectSlicesParser.Parse (sssjson);

			string vmsjson = System.IO.File.ReadAllText ("../../../movie/Charities.json");
			var vms = libengagement.VerdictMapsParser.Parse (vmsjson);

			foreach(var ss in sss)
				b.AddSubjectSlice(ss);

			foreach(var vm in vms)
				b.AddVerdictMap(vm);

			var ess = new List<libengagement.EngagementSlice>();
			ess.Add(CreateEngagementSlice(90.0f, 100.0f, 1.0f));

			var evs = b.Run (ess);
			Assert.AreEqual (3, evs.Count);

			Assert.AreEqual ("WNF", evs [0].verdict);
			Assert.AreEqual (1.8f, evs [0].engagement);
			Assert.AreEqual ("Unicef", evs [1].verdict);
			Assert.AreEqual ("Hartstichting", evs [2].verdict);
		}
	}
}

