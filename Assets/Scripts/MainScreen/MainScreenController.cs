using System;
using Emotions;
using MoodTrackerScreen;
using PractisesScreen;
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

        [SerializeField] private StressPlane _stressPlane;
        [SerializeField] private StressLevelScreenController _stressLevelScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _emotionPlane.Opened += OpenMoodTracker;
            _moodTrackerScreen.BackClicked += Enable;
            _moodTrackerScreen.DataSaved += SetEmotionsData;

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
        }

        private void SetEmotionsData(EmotionData data)
        {
            Enable();
            _emotionsController.AddEmotion(data);
        }

        private void OpenPractice(PracticeData data)
        {
            _openPracticeScreen.Enable(data, true);
            Disable();
        }
    }
}