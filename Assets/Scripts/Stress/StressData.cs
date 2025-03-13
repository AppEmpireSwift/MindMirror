using System;
using UnityEngine;

namespace Stress
{
    [Serializable]
    public class StressData
    {
        public StressType StressType;
        public int Number;
    }

    public enum StressType
    {
        RelaxedStressed,
        MildlyTense,
        ModeratelyStressed,
        HighlyStressed,
        Overwhelmed
    }
}