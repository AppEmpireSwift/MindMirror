using System;
using Emotions;

namespace HealthJournalScreen
{
    [Serializable]
    public class JournalData
    {
        public string Name;
        public string Description;
        public DateTime Date;
        public EmotionType EmotionType;
    }
}