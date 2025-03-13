using System;
using UnityEngine;

namespace Emotions
{
    [Serializable]
    public class EmotionData
    {
        public DateTime EmotionDate;
        public EmotionType EmotionType;

        public EmotionData(EmotionType emotionType)
        {
            EmotionDate = DateTime.Today;
            EmotionType = emotionType;
        }
    }

    public enum EmotionType
    {
        Angry,
        Sad,
        Neutral,
        Happy,
        Overjoyed
    }
}