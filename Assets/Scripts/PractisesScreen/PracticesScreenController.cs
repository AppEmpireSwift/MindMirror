using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PractisesScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class PracticesScreenController : MonoBehaviour
    {
        [SerializeField] private List<PracticePlane> _practicePlanes;
        [SerializeField] private Button _addButton;
        [SerializeField] private OpenPracticeScreen _openPracticeScreen;
        [SerializeField] private CreatePracticeScreen _createPracticeScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private PracticeDataSaver _dataSaver;
        private List<PracticeData> _practiceDataList = new List<PracticeData>();

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _dataSaver = new PracticeDataSaver();
        }

        private void OnEnable()
        {
            _openPracticeScreen.BackClicked += Enable;
            _createPracticeScreen.BackClicked += Enable;
            _createPracticeScreen.DataSaved += AddData;
            _addButton.onClick.AddListener(OnAddDataClicked);

            foreach (PracticePlane practicePlane in _practicePlanes)
            {
                practicePlane.OpenButtonClicked += OpenPractice;
            }
        }

        private void OnDisable()
        {
            _openPracticeScreen.BackClicked -= Enable;
            _createPracticeScreen.BackClicked -= Enable;
            _createPracticeScreen.DataSaved -= AddData;
            _addButton.onClick.RemoveListener(OnAddDataClicked);

            foreach (PracticePlane practicePlane in _practicePlanes)
            {
                practicePlane.OpenButtonClicked -= OpenPractice;
            }
        }

        private void Start()
        {
            LoadSavedPractices();
            _screenVisabilityHandler.DisableScreen();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OpenPractice(PracticeData data)
        {
            _openPracticeScreen.Enable(data);
            Disable();
        }

        private void OnAddDataClicked()
        {
            _createPracticeScreen.Enable();
            Disable();
        }

        private void AddData(PracticeData data)
        {
            Enable();

            _practiceDataList.Add(data);
            SavePracticeData();

            var availablePlane =
                _practicePlanes.FirstOrDefault(p => !p.IsActive && !p.IsFilled);
            availablePlane?.Enable(data);
        }

        private void OnDataDeleted(PracticeData data)
        {
            _practiceDataList.RemoveAll(p =>
                p.Title == data.Title && p.Type == data.Type &&
                p.Minutes == data.Minutes && p.Seconds == data.Seconds);

            SavePracticeData();

            foreach (var plane in _practicePlanes)
            {
                if (plane.IsActive && IsSamePracticeData(plane.Data, data))
                {
                    plane.Disable();
                    break;
                }
            }
        }

        private bool IsSamePracticeData(PracticeData a, PracticeData b)
        {
            return a.Title == b.Title && a.Type == b.Type &&
                   a.Minutes == b.Minutes && a.Seconds == b.Seconds;
        }

        private void LoadSavedPractices()
        {
            _practiceDataList = _dataSaver.LoadPracticeData();

            foreach (PracticeData practiceData in _practiceDataList)
            {
                _practicePlanes.FirstOrDefault(p => !p.IsFilled)
                    ?.Enable(practiceData);
            }
        }

        private void SavePracticeData()
        {
            _dataSaver.SavePracticeData(_practiceDataList);
        }

        private void OnApplicationQuit()
        {
            SavePracticeData();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SavePracticeData();
            }
        }
    }
}