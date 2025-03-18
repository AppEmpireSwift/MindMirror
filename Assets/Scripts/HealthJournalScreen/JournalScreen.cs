using System;
using System.Collections.Generic;
using System.Linq;
using HealthJournalScreen.CreateJournalScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace HealthJournalScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class JournalScreen : MonoBehaviour
    {
        [SerializeField] private DateElement[] _dateElements;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private TMP_Text _monthText;
        [SerializeField] private List<JournalPlane> _journalPlanes;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _addButton;
        [SerializeField] private CreateJournalScreenController _createJournalScreen;

        [Header("Данные журнала")] [SerializeField]
        private bool _autoSaveOnChange = true;

        [SerializeField] private bool _loadOnStart = true;

        [Header("Animation Settings")] [SerializeField]
        private float _fadeAnimDuration = 0.3f;

        [SerializeField] private float _scaleAnimDuration = 0.3f;
        [SerializeField] private Ease _animEaseType = Ease.OutBack;

        [Header("Screen Animation")] [SerializeField]
        private RectTransform _screenContainer;

        [SerializeField] private CanvasGroup _screenCanvasGroup;

        [Header("Date Elements Animation")] [SerializeField]
        private float _staggerDelay = 0.05f;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private List<JournalData> _journalDataList;
        private Sequence _screenSequence;
        private DateTime _currentSelectedDate;

        public event Action BackClicked;
        public event Action<int> DataChanged;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();

            if (_screenCanvasGroup == null)
                _screenCanvasGroup = GetComponent<CanvasGroup>();

            _backButton.onClick.AddListener(OnBackButtonClicked);
            _addButton.onClick.AddListener(OnAddButtonClicked);

            foreach (var dateElement in _dateElements)
            {
                dateElement.DateSelected += OnDateSelected;
            }

            if (_createJournalScreen != null)
            {
                _createJournalScreen.BackButtonClicked += OnCreateJournalBackButtonClicked;
                _createJournalScreen.JournalCreated += OnJournalCreated;
            }
        }

        private void Start()
        {
            if (_loadOnStart)
            {
                LoadJournalData();
            }

            _screenVisabilityHandler.DisableScreen();
        }

        private void OnDisable()
        {
            SaveJournalData();

            DOTween.Kill(_screenContainer);
            DOTween.Kill(_screenCanvasGroup);
            _screenSequence?.Kill();

            foreach (var dateElement in _dateElements)
            {
                DOTween.Kill(dateElement.transform);
            }

            foreach (var journalPlane in _journalPlanes)
            {
                DOTween.Kill(journalPlane.transform);
            }

            DOTween.Kill(_dateText.transform);
            DOTween.Kill(_monthText.transform);
            DOTween.Kill(_backButton.transform);
            DOTween.Kill(_addButton.transform);

            _backButton.onClick.RemoveAllListeners();
            _addButton.onClick.RemoveAllListeners();

            foreach (var dateElement in _dateElements)
            {
                dateElement.DateSelected -= OnDateSelected;
            }

            if (_createJournalScreen != null)
            {
                _createJournalScreen.BackButtonClicked -= OnCreateJournalBackButtonClicked;
                _createJournalScreen.JournalCreated -= OnJournalCreated;
            }
        }

        public void LoadJournalData()
        {
            _journalDataList = JournalDataSaver.LoadJournalData();
            if (_journalDataList != null) DataChanged?.Invoke(_journalDataList.Count);
        }

        public void SaveJournalData()
        {
            if (_journalDataList != null && _journalDataList.Count > 0)
            {
                JournalDataSaver.SaveJournalData(_journalDataList);
            }

            if (_journalDataList != null) DataChanged?.Invoke(_journalDataList.Count);
        }


        public void SetJournalData(List<JournalData> journalDataList)
        {
            _journalDataList = journalDataList;

            if (_autoSaveOnChange)
            {
                SaveJournalData();
            }
        }


        public void AddJournalEntry(JournalData journalData)
        {
            if (_journalDataList == null)
                _journalDataList = new List<JournalData>();

            int existingIndex = _journalDataList.FindIndex(j => j.Date.Date == journalData.Date.Date);

            if (existingIndex >= 0)
            {
                _journalDataList[existingIndex] = journalData;
            }
            else
            {
                _journalDataList.Add(journalData);
            }

            if (_autoSaveOnChange)
            {
                SaveJournalData();
            }

            if (_journalDataList != null) DataChanged?.Invoke(_journalDataList.Count);
        }

        public bool DeleteJournalEntry(DateTime date)
        {
            if (_journalDataList == null || _journalDataList.Count == 0)
                return false;

            int entryIndex = _journalDataList.FindIndex(j => j.Date.Date == date.Date);

            if (entryIndex >= 0)
            {
                _journalDataList.RemoveAt(entryIndex);

                if (_autoSaveOnChange)
                {
                    SaveJournalData();
                }

                return true;
            }

            return false;
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            AnimateScreenIn();
            DisableAllPlanes();
            InitializeDateElements();
        }

        public void Disable()
        {
            AnimateScreenOut(() => _screenVisabilityHandler.DisableScreen());
        }

        private void AnimateScreenIn()
        {
            _screenSequence?.Kill();

            if (_screenCanvasGroup != null)
                _screenCanvasGroup.alpha = 0f;

            if (_screenContainer != null)
                _screenContainer.localScale = new Vector3(0.95f, 0.95f, 1f);

            _screenSequence = DOTween.Sequence();

            if (_screenCanvasGroup != null)
                _screenSequence.Join(_screenCanvasGroup.DOFade(1f, _fadeAnimDuration));

            if (_screenContainer != null)
                _screenSequence.Join(_screenContainer.DOScale(1f, _scaleAnimDuration).SetEase(_animEaseType));

            _screenSequence.AppendCallback(() => AnimateUIElementsIn());
        }

        private void AnimateUIElementsIn()
        {
            _monthText.transform.localScale = Vector3.zero;
            _monthText.transform.DOScale(1f, _scaleAnimDuration).SetEase(_animEaseType);
        }

        private void AnimateScreenOut(Action onComplete = null)
        {
            _screenSequence?.Kill();

            _screenSequence = DOTween.Sequence();

            if (_screenCanvasGroup != null)
                _screenSequence.Join(_screenCanvasGroup.DOFade(0f, _fadeAnimDuration));

            if (_screenContainer != null)
                _screenSequence.Join(_screenContainer.DOScale(0.95f, _scaleAnimDuration).SetEase(Ease.InBack));

            _screenSequence.OnComplete(() => onComplete?.Invoke());
        }

        private void OnBackButtonClicked()
        {
            _backButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.3f, 10, 1).OnComplete(() =>
            {
                SaveJournalData();
                BackClicked?.Invoke();
                Disable();
            });
        }

        private void OnAddButtonClicked()
        {
            _addButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.3f, 10, 1).OnComplete(() =>
            {
                _createJournalScreen.Enable();
                Disable();
            });
        }

        private void OnCreateJournalBackButtonClicked()
        {
            Enable();
        }

        private void OnJournalCreated(JournalData journalData)
        {
            AddJournalEntry(journalData);

            Enable();

            if (_currentSelectedDate.Date == journalData.Date.Date)
            {
                EnablePlane(journalData);
            }
        }

        private void InitializeDateElements()
        {
            DateTime today = DateTime.Today;
            _currentSelectedDate = today;
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            _monthText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f);
            _monthText.text = today.ToString("MMMM yyyy");

            int daysToSubtract = (int)today.DayOfWeek;

            DateTime startOfWeek = today.AddDays(-daysToSubtract);

            for (int i = 0; i < _dateElements.Length; i++)
            {
                DateTime currentDate = startOfWeek.AddDays(i);
                _dateElements[i].SetDate(currentDate);

                if (currentDate.Month != today.Month)
                {
                    _dateElements[i].UnSelect();
                }
                else
                {
                    JournalData journalData = _journalDataList?.FirstOrDefault(j => j.Date.Date == currentDate.Date);

                    if (journalData != null)
                    {
                        switch (journalData.EmotionType)
                        {
                            case Emotions.EmotionType.Angry:
                            case Emotions.EmotionType.Sad:
                                _dateElements[i].SetNagativeColor();
                                break;
                            case Emotions.EmotionType.Neutral:
                                _dateElements[i].SetNeutralColor();
                                break;
                            case Emotions.EmotionType.Happy:
                            case Emotions.EmotionType.Overjoyed:
                                _dateElements[i].SetPositiveColor();
                                break;
                        }

                        if (currentDate.Date == today.Date)
                        {
                            EnablePlane(journalData);
                        }
                    }
                    else
                    {
                        _dateElements[i].Select();
                    }
                }

                if (currentDate.Date == today.Date)
                {
                    _dateElements[i].transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.5f, 10, 1)
                        .SetDelay(0.5f + (_staggerDelay * (_dateElements.Length + 3)));
                }
            }

            _dateText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f);
            _dateText.text = today.Day + ", " + today.ToString("MMMM");
        }

        private void OnDateSelected(DateTime selectedDate)
        {
            _currentSelectedDate = selectedDate;
            JournalData journalData = _journalDataList?.FirstOrDefault(j => j.Date.Date == selectedDate.Date);

            _dateText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f);
            _dateText.text = selectedDate.Day + ", " + selectedDate.ToString("MMMM");

            if (journalData != null)
            {
                EnablePlane(journalData);
            }
            else
            {
                DisableAllPlanes();
            }
        }

        private void EnablePlane(JournalData data)
        {
            DisableAllPlanes();

            JournalPlane plane = _journalPlanes.FirstOrDefault(p => !p.IsActive);
            if (plane != null)
            {
                plane.transform.localScale = Vector3.zero;

                plane.Enable(data);

                plane.transform.DOScale(1f, _scaleAnimDuration * 1.5f).SetEase(_animEaseType);
            }
        }

        private void DisableAllPlanes()
        {
            foreach (JournalPlane journalPlane in _journalPlanes)
            {
                if (journalPlane.IsActive)
                {
                    journalPlane.transform.DOScale(0f, _scaleAnimDuration * 0.5f)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => journalPlane.Disable());
                }
                else
                {
                    journalPlane.Disable();
                }
            }
        }
    }
}