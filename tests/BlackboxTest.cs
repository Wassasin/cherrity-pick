using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace tests
{
	[TestFixture]
	public class BlackboxTest
	{
		private static libengagement.Slice CreateSlice(float start, float end)
		{
			var slice = new libengagement.Slice ();
			slice.start = start;
			slice.end = end;
			return slice;
		}

		private static libengagement.EngagementSlice CreateEngagementSlice(float start, float end, float engagement)
		{
			var es = new libengagement.EngagementSlice ();
			es.slice = CreateSlice(start, end);
			es.engagement = engagement;
			return es;
		}

		[Test]
		public void RunSequenceTest()
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

		private static float Incl(libengagement.Slice x, libengagement.Slice y)
		{
			return libengagement.Blackbox.InclusionRatio(x, y);
		}

		[Test]
		public void InclusionTest()
		{
			Assert.AreEqual (0.0f, Incl(CreateSlice (1.0f, 2.0f), CreateSlice (3.0f, 4.0f)));
			Assert.AreEqual (0.0f, Incl(CreateSlice (3.0f, 4.0f), CreateSlice (1.0f, 2.0f)));
			Assert.AreEqual (0.0f, Incl(CreateSlice (1.0f, 4.0f), CreateSlice (3.0f, 3.0f)));
			Assert.AreEqual (1.0f, Incl(CreateSlice (3.0f, 3.0f), CreateSlice (1.0f, 4.0f)));
			Assert.AreEqual (0.5f, Incl(CreateSlice (1.0f, 3.0f), CreateSlice (2.0f, 4.0f)));
			Assert.AreEqual (0.5f, Incl(CreateSlice (3.0f, 5.0f), CreateSlice (2.0f, 4.0f)));
			Assert.AreEqual (1.0f, Incl(CreateSlice (2.0f, 3.0f), CreateSlice (1.0f, 4.0f)));
			Assert.AreEqual (0.5f, Incl(CreateSlice (3.0f, 5.0f), CreateSlice (-3.0f, 4.0f)));
		}
	}
}

