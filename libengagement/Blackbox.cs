using System;
using System.Collections.Generic;

namespace libengagement
{
	public class Blackbox
	{
		private SortedSet<string> subjects;
		private SortedSet<string> verdicts;

		private List<SubjectSlice> subjectSlices;
		private List<VerdictMap> verdictMaps;

		public Blackbox() {}

		public void AddSubject(string subject)
		{
			subjects.Add (subject);
		}

		public void AddVerdict(string verdict)
		{
			verdicts.Add (verdict);
		}

		public void AddSubjectSlice(SubjectSlice ss)
		{
			subjectSlices.Add (ss);
		}

		public void AddVerdictMap(VerdictMap vm)
		{
			verdictMaps.Add (vm);
		}

		/* Returns ratio between length of inclusion and length of engagement */
		private static float InclusionRatio(Slice engagement, Slice subject)
		{
			Slice newSlice;
			if (engagement.start > subject.end || engagement.end < subject.start)
				return 0.0f;

			if (engagement.start <= subject.start)
				newSlice.start = subject.start;
			else
				newSlice.start = engagement.start;

			if (engagement.end <= subject.end)
				newSlice.end = engagement.end;
			else
				newSlice.end = subject.end;

			if (newSlice.Length () == engagement.Length ())
				return 1.0f;

			return newSlice.Length () / engagement.Length ();
		}

		public List<EngagementVerdict> Run(List<EngagementSlice> slices)
		{
			var subjectacc = new Dictionary<string, float>();
			foreach (string subject in subjects)
				subjectacc [subject] = 0.0f;

			foreach (EngagementSlice es in slices)
				foreach (SubjectSlice ss in subjectSlices)
					subjectacc [ss.subject] += es.engagement * InclusionRatio (es.slice, ss.slice);

			var verdictacc = new Dictionary<string, float>();
			foreach (string verdict in verdicts)
				verdictacc [verdict] = 0.0f;

			foreach (VerdictMap vm in verdictMaps)
				foreach (var kvp in subjectacc)
					verdictacc [vm.verdict] += vm.maps [kvp.Key] * kvp.Value;

			var result = new List<EngagementVerdict>();
			foreach(var kvp in verdictacc)
			{
				EngagementVerdict ev;
				ev.verdict = kvp.Key;
				ev.engagement = kvp.Value;
				result.Add (ev);
			}

			result.Sort ();
			result.Reverse (); // Descending order
			return result;
		}
	}
}

