using System;
using System.Collections.Generic;
using Emotions;
using MainScreen;
using Stress;
using UnityEngine;

namespace SaveSystem
{
    public class DataSaveSystem : MonoBehaviour
    {
        private const string EmotionsSaveKey = "SavedEmotionsData";
        private const string StressSaveKey = "SavedStressData";

        [SerializeField] private EmotionsController _emotionsController;

        [Serializable]
        private class EmotionDataList
        {
            public List<EmotionData> emotions = new List<EmotionData>();
        }

        [Serializable]
        private class StressDataList
        {
            public List<StressData> stressData = new List<StressData>();
        }

        private EmotionDataList _emotionDataList = new EmotionDataList();
        private StressDataList _stressDataList = new StressDataList();

        private void Start()
        {
            LoadAllData();
        }

        public void SaveEmotionData(EmotionData emotionData)
        {
            _emotionDataList.emotions.Add(emotionData);

            string json = JsonUtility.ToJson(_emotionDataList);
            PlayerPrefs.SetString(EmotionsSaveKey, json);
            PlayerPrefs.Save();
        }

        public void SaveStressData(StressData stressData)
        {
            _stressDataList.stressData.Add(stressData);

            string json = JsonUtility.ToJson(_stressDataList);
            PlayerPrefs.SetString(StressSaveKey, json);
            PlayerPrefs.Save();
        }

        public void LoadAllData()
        {
            if (PlayerPrefs.HasKey(EmotionsSaveKey))
            {
                string json = PlayerPrefs.GetString(EmotionsSaveKey);
                EmotionDataList loadedList = JsonUtility.FromJson<EmotionDataList>(json);

                if (loadedList != null)
                {
                    _emotionDataList = loadedList;

                    if (_emotionsController != null)
                    {
                        foreach (EmotionData emotion in _emotionDataList.emotions)
                        {
                            _emotionsController.AddEmotion(emotion);
                        }
                    }
                }
            }

            if (PlayerPrefs.HasKey(StressSaveKey))
            {
                string json = PlayerPrefs.GetString(StressSaveKey);
                StressDataList loadedList = JsonUtility.FromJson<StressDataList>(json);

                if (loadedList != null)
                {
                    _stressDataList = loadedList;
                }
            }
        }

        public EmotionData GetLatestEmotionData()
        {
            if (_emotionDataList.emotions.Count > 0)
            {
                return _emotionDataList.emotions[_emotionDataList.emotions.Count - 1];
            }

            return null;
        }

        public StressData GetLatestStressData()
        {
            if (_stressDataList.stressData.Count > 0)
            {
                return _stressDataList.stressData[_stressDataList.stressData.Count - 1];
            }

            return null;
        }

        public List<EmotionData> GetAllEmotionData()
        {
            return new List<EmotionData>(_emotionDataList.emotions);
        }

        public List<StressData> GetAllStressData()
        {
            return new List<StressData>(_stressDataList.stressData);
        }

        public void ClearAllData()
        {
            _emotionDataList.emotions.Clear();
            _stressDataList.stressData.Clear();

            PlayerPrefs.DeleteKey(EmotionsSaveKey);
            PlayerPrefs.DeleteKey(StressSaveKey);
            PlayerPrefs.Save();
        }
    }
}