using System;

namespace libFace
{
	public struct Person
	{
		public Face face;
		public Mood mood;
		public Expression expression;

        public float ToEngagement()
        {
            float moodMod = mood.confidence;
            if (mood.value == Mood.MoodValue.Negative)
                moodMod *= -1.0f;

            float exprMod = expression.anger + expression.disgust + expression.fear + expression.happiness + expression.sadness + expression.surprise - expression.neutral;
            return moodMod * exprMod;
        }
	}
}

