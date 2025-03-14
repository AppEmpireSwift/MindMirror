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

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
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

            var availablePlane = _practicePlanes.FirstOrDefault(p => !p.IsActive);
            availablePlane?.Enable(data);
        }
    }
}