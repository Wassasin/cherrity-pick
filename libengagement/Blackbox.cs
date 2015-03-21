using System;
using System.Collections.Generic;

namespace libengagement
{
	public class Blackbox
	{
		private SortedSet<string> subjects = new SortedSet<string>();
		private SortedSet<string> verdicts = new SortedSet<string>();

		private List<SubjectSlice> subjectSlices = new List<SubjectSlice> ();
		private List<VerdictMap> verdictMaps = new List<VerdictMap>();

		public Blackbox() {}

		public void AddSubjectSlice(SubjectSlice ss)
		{
			subjects.Add (ss.subject);
			subjectSlices.Add (ss);
		}

		public void AddVerdictMap(VerdictMap vm)
		{
			verdicts.Add (vm.verdict);
			foreach (var kvp in vm.maps)
				subjects.Add (kvp.Key);

			verdictMaps.Add (vm);
		}

		/* Returns ratio between length of inclusion and length of engagement */
		public static float InclusionRatio(Slice engagement, Slice subject)
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
            var subjectcount = new Dictionary<string, float>();
            foreach (string subject in subjects)
            {
                subjectacc[subject] = 0.0f;
                subjectcount[subject] = 0.0f;
            }

            foreach (EngagementSlice es in slices) { 
                foreach (SubjectSlice ss in subjectSlices)
                {
                    subjectacc[ss.subject] += ss.weight * es.engagement * InclusionRatio(es.slice, ss.slice);
                    subjectcount[ss.subject] += InclusionRatio(es.slice, ss.slice);
                }
            }

            foreach (string subject in subjects)
            {
                if (subjectcount[subject] > 0)
                    subjectacc[subject] /= subjectcount[subject];
            }

			var verdictacc = new Dictionary<string, float>();
			foreach (string verdict in verdicts)
				verdictacc [verdict] = 0.0f;

			foreach (VerdictMap vm in verdictMaps)
				foreach (var kvp in subjectacc)
					if(vm.maps.ContainsKey(kvp.Key))
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

