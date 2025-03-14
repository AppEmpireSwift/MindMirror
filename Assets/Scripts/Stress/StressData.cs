using System;
using UnityEngine;

namespace Stress
{
    [Serializable]
    public class StressData
    {
        public StressType StressType;
        public int Number;

        public StressData(StressType stressType)
        {
            StressType = stressType;
            Number = (int)stressType;
        }
    }

    public enum StressType
    {
        RelaxedStressed = 1,
        MildlyTense,
        ModeratelyStressed,
        HighlyStressed,
        Overwhelmed
    }
}