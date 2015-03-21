using System;

using NUnit.Framework;

namespace tests
{
	[TestFixture]
	public class ParseTests
	{
		[Test]
		public void ParseSubjectSlices()
		{
			string json = System.IO.File.ReadAllText ("../../../movie/SampleMovie1.json");

			var sss = libengagement.SubjectSlicesParser.Parse (json);
			Assert.AreEqual (11, sss.Count);

			Assert.AreEqual ("Nature", sss [0].subject);
			Assert.AreEqual (1.0f, sss [0].weight);
			Assert.AreEqual (0.0f, sss [0].slice.start);
			Assert.AreEqual (29.0f, sss [0].slice.end);

			Assert.AreEqual ("Planet", sss [8].subject);
			Assert.AreEqual (0.5f, sss [8].weight);
			Assert.AreEqual (90.0f, sss [8].slice.start);
			Assert.AreEqual (116.0f, sss [8].slice.end);

			Assert.AreEqual ("Space", sss [10].subject);
			Assert.AreEqual (1.0f, sss [10].weight);
			Assert.AreEqual (117.0f, sss [10].slice.start);
			Assert.AreEqual (150.0f, sss [10].slice.end);
		}

		[Test]
		public void ParseVerdictMaps()
		{
			string json = System.IO.File.ReadAllText ("../../../movie/Charities.json");

			var vms = libengagement.VerdictMapsParser.Parse (json);
			Assert.AreEqual (3, vms.Count);

			Assert.AreEqual ("WNF", vms [0].verdict);
			Assert.AreEqual (0.8f, vms [0].maps["Nature"]);
			Assert.AreEqual (1.0f, vms [0].maps["Animals"]);

			Assert.AreEqual ("Hartstichting", vms [2].verdict);
			Assert.AreEqual (1.0f, vms [2].maps["Health"]);
		}
	}
}

