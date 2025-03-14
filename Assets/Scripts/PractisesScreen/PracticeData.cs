using System;

namespace PractisesScreen
{
    [Serializable]
    public class PracticeData
    {
        public int Minutes;
        public int Seconds;
        public string Type;
        public string Title;
        public string TimeDisplay;

        public PracticeData(int minutes, int seconds, string type, string title)
        {
            Minutes = minutes;
            Seconds = seconds;
            Type = type;
            Title = title;
            TimeDisplay = $"{minutes:D2}:{seconds:D2}";
        }
        
        public int TotalSeconds => Minutes * 60 + Seconds;
    }
}