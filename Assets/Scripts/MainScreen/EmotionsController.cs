using System;
using System.Collections.Generic;
using System.Linq;
using Emotions;
using UnityEngine;

namespace MainScreen
{
    public class EmotionsController : MonoBehaviour
    {
        [SerializeField] private EmotionPlane _emotionPlane;

        private List<EmotionData> _emotionDatas = new();

        public void AddEmotion(EmotionData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _emotionDatas.Add(data);
            VerifyDuplicateData(data);

            _emotionPlane.SetEmotionData(data, GetStreakCount(data));
        }

        private void VerifyDuplicateData(EmotionData data)
        {
            foreach (var emotionData in _emotionDatas.Where(emotionData =>
                         emotionData.EmotionDate.Day == data.EmotionDate.Day))
            {
                emotionData.EmotionType = data.EmotionType;
            }
        }

        private int GetStreakCount(EmotionData data)
        {
            var sortedEmotions = _emotionDatas
                .OrderByDescending(e => e.EmotionDate)
                .ToList();

            int streak = 0;
            bool streakBroken = false;
            DateTime lastDate = DateTime.MinValue;

            foreach (var emotion in sortedEmotions)
            {
                if (lastDate == DateTime.MinValue)
                {
                    lastDate = emotion.EmotionDate;
                }

                TimeSpan difference = lastDate - emotion.EmotionDate;
                if (difference.Days > 1)
                {
                    streakBroken = true;
                }

                if (streakBroken)
                    break;

                if (emotion.EmotionType == data.EmotionType)
                {
                    streak++;
                    lastDate = emotion.EmotionDate;
                }
                else
                {
                    break;
                }
            }

            return streak;
        }
    }
}