using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace PractisesScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class CreatePracticeScreen : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _titleInput;
        [SerializeField] private TMP_InputField _typeInput;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private GameObject _timeSelectorScreen;
        [SerializeField] private TimeSelector _timeSelector;
        [SerializeField] private Button _openTimeSelectorButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _cancelTimeSelectorButton;
        [SerializeField] private Button _saveTimeSelectorButton;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.5f;

        [SerializeField] private Ease _animationEase = Ease.OutBack;
        [SerializeField] private Vector3 _popupScale = new Vector3(1.05f, 1.05f, 1.05f);

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private int _minutes;
        private int _seconds;
        private string _selectedTime;

        public event Action<PracticeData> DataSaved;
        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
            _openTimeSelectorButton.onClick.AddListener(OpenTimeSelector);
            _saveTimeSelectorButton.onClick.AddListener(SaveTimeSelection);
            _cancelTimeSelectorButton.onClick.AddListener(CancelTimeSelection);
            _saveButton.onClick.AddListener(SavePracticeData);
            _titleInput.onValueChanged.AddListener(OnInputValueChanged);
            _typeInput.onValueChanged.AddListener(OnInputValueChanged);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _openTimeSelectorButton.onClick.RemoveListener(OpenTimeSelector);
            _saveTimeSelectorButton.onClick.RemoveListener(SaveTimeSelection);
            _cancelTimeSelectorButton.onClick.RemoveListener(CancelTimeSelection);
            _saveButton.onClick.RemoveListener(SavePracticeData);
            _titleInput.onValueChanged.RemoveListener(OnInputValueChanged);
            _typeInput.onValueChanged.RemoveListener(OnInputValueChanged);
        }

        private void Start()
        {
            Disable();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();

            SetCurrentDateTime();
            _saveButton.interactable = false;
            _timeSelectorScreen.SetActive(false);

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, _animationDuration)
                .SetEase(_animationEase);
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void SetCurrentDateTime()
        {
            _selectedTime = "05:00";
            _timeText.text = _selectedTime;

            AnimateTimeTextUpdate();

            string[] timeParts = _selectedTime.Split(':');
            if (timeParts.Length == 2)
            {
                int.TryParse(timeParts[0], out _minutes);
                int.TryParse(timeParts[1], out _seconds);
            }

            UpdateSaveButtonState();
        }

        private void OpenTimeSelector()
        {
            _timeSelectorScreen.SetActive(true);

            _timeSelectorScreen.transform.localScale = Vector3.zero;
            _timeSelectorScreen.transform.DOScale(Vector3.one, _animationDuration)
                .SetEase(_animationEase);

            _timeSelector.Enable();
        }

        private void SaveTimeSelection()
        {
            _selectedTime = _timeSelector.GetTimeString();

            if (!string.IsNullOrEmpty(_selectedTime))
            {
                _timeText.text = _selectedTime;

                AnimateTimeTextUpdate();

                string[] timeParts = _selectedTime.Split(':');
                if (timeParts.Length == 2)
                {
                    int.TryParse(timeParts[0], out _minutes);
                    int.TryParse(timeParts[1], out _seconds);
                }
            }

            _timeSelectorScreen.transform.DOScale(Vector3.zero, _animationDuration * 0.7f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _timeSelectorScreen.SetActive(false);
                    _timeSelector.Disable();
                });

            UpdateSaveButtonState();
        }

        private void AnimateTimeTextUpdate()
        {
            Color originalColor = _timeText.color;
            Vector3 originalScale = _timeText.transform.localScale;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_timeText.transform.DOScale(_popupScale, _animationDuration * 0.5f));
            sequence.Join(_timeText.DOColor(Color.green, _animationDuration * 0.5f));
            sequence.Append(_timeText.transform.DOScale(originalScale, _animationDuration * 0.5f));
            sequence.Join(_timeText.DOColor(originalColor, _animationDuration * 0.5f));
        }

        private void CancelTimeSelection()
        {
            _timeSelectorScreen.transform.DOScale(Vector3.zero, _animationDuration * 0.7f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _timeSelectorScreen.SetActive(false);
                    _timeSelector.Disable();
                });
        }

        private void OnBackButtonClicked()
        {
            _backButton.transform.DOScale(_popupScale, _animationDuration * 0.3f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _backButton.transform.DOScale(Vector3.one, _animationDuration * 0.3f);
                    BackClicked?.Invoke();
                });
        }

        private void SavePracticeData()
        {
            if (!string.IsNullOrEmpty(_titleInput.text) &&
                !string.IsNullOrEmpty(_typeInput.text) &&
                !string.IsNullOrEmpty(_timeText.text))
            {
                _saveButton.transform.DOScale(_popupScale, _animationDuration * 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => { _saveButton.transform.DOScale(Vector3.one, _animationDuration * 0.3f); });

                PracticeData data = new PracticeData(_minutes, _seconds, _typeInput.text, _titleInput.text);
        
                DataSaved?.Invoke(data);
            }
        }

        private void OnInputValueChanged(string newValue)
        {
            UpdateSaveButtonState();
        }

        private void UpdateSaveButtonState()
        {
            bool shouldBeInteractable = !string.IsNullOrEmpty(_titleInput.text) &&
                                        !string.IsNullOrEmpty(_typeInput.text) &&
                                        !string.IsNullOrEmpty(_timeText.text);

            if (shouldBeInteractable != _saveButton.interactable)
            {
                if (shouldBeInteractable)
                {
                    _saveButton.transform.DOScale(_popupScale, _animationDuration * 0.5f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => { _saveButton.transform.DOScale(Vector3.one, _animationDuration * 0.5f); });
                }
            }

            _saveButton.interactable = shouldBeInteractable;
        }
    }
}