using System;
using System.Globalization;
using System.Threading;
using Emotions;
using HealthJournalScreen;
using MoodTrackerScreen;
using PractisesScreen;
using SaveSystem;
using Stress;
using StressLevelScreen;
using UnityEngine;

namespace MainScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class MainScreenController : MonoBehaviour
    {
        [SerializeField] private EmotionsController _emotionsController;
        [SerializeField] private EmotionPlane _emotionPlane;
        [SerializeField] private MoodTrackerScreenController _moodTrackerScreen;
        [SerializeField] private PracticePlane[] _practicePlanes;
        [SerializeField] private OpenPracticeScreen _openPracticeScreen;
        [SerializeField] private JournalScreen _journalScreen;
        [SerializeField] private JournalPlane _journalPlane;

        [SerializeField] private StressPlane _stressPlane;
        [SerializeField] private StressLevelScreenController _stressLevelScreen;

        [SerializeField] private DataSaveSystem _dataSaveSystem;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void Start()
        {
            if (_dataSaveSystem != null)
            {
                StressData latestStressData = _dataSaveSystem.GetLatestStressData();
                if (latestStressData != null)
                {
                    _stressPlane.SetData(latestStressData);
                }
            }
        }

        private void OnEnable()
        {
            _emotionPlane.Opened += OpenMoodTracker;
            _moodTrackerScreen.BackClicked += Enable;
            _moodTrackerScreen.DataSaved += SetEmotionsData;
            _journalScreen.BackClicked += Enable;
            _journalPlane.ButtonClicked += _journalScreen.Enable;
            _journalScreen.DataChanged += _journalPlane.UpdateText;

            _stressPlane.OpenButtonClicked += OpenStressLevelScreen;
            _stressLevelScreen.BackClicked += Enable;
            _stressLevelScreen.DataSaved += SetStressData;

            _openPracticeScreen.BackFromMainClicked += Enable;

            foreach (PracticePlane practicePlane in _practicePlanes)
            {
                practicePlane.OpenButtonClicked += OpenPractice;
            }
        }

        private void OnDisable()
        {
            _emotionPlane.Opened -= OpenMoodTracker;
            _moodTrackerScreen.BackClicked -= Enable;
            _moodTrackerScreen.DataSaved -= SetEmotionsData;

            _stressPlane.OpenButtonClicked -= OpenStressLevelScreen;
            _stressLevelScreen.BackClicked -= Enable;
            _stressLevelScreen.DataSaved -= SetStressData;
            _journalScreen.BackClicked -= Enable;
            _journalScreen.DataChanged -= _journalPlane.UpdateText;

            _journalPlane.ButtonClicked -= _journalScreen.Enable;

            _openPracticeScreen.BackFromMainClicked -= Enable;

            foreach (PracticePlane practicePlane in _practicePlanes)
            {
                practicePlane.OpenButtonClicked -= OpenPractice;
            }
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OpenMoodTracker()
        {
            _moodTrackerScreen.Enable();
            Disable();
        }

        private void OpenStressLevelScreen()
        {
            _stressLevelScreen.Enable();
            Disable();
        }

        private void SetStressData(StressData stressData)
        {
            Enable();
            _stressPlane.SetData(stressData);

            if (_dataSaveSystem != null)
            {
                _dataSaveSystem.SaveStressData(stressData);
            }
        }

        private void SetEmotionsData(EmotionData data)
        {
            Enable();
            _emotionsController.AddEmotion(data);

            if (_dataSaveSystem != null)
            {
                _dataSaveSystem.SaveEmotionData(data);
            }
        }

        private void OpenPractice(PracticeData data)
        {
            _openPracticeScreen.Enable(data, true);
            Disable();
        }
    }
}