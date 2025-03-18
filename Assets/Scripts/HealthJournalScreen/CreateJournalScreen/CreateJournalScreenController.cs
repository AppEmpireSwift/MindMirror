using System;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace HealthJournalScreen.CreateJournalScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class CreateJournalScreenController : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _titleInput;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private GameObject _dateSelectionScreen;
        [SerializeField] private Button _openDateButton;
        [SerializeField] private DatePickerSettings _datePickerSettings;
        [SerializeField] private Button _confirmDateButton;
        [SerializeField] private Button _closeDateButton;
        [SerializeField] private EmotionButton[] _emotionButtons;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private Button _saveButton;

        [Header("Animation Settings")] [SerializeField]
        private float _fadeAnimDuration = 0.3f;

        [SerializeField] private float _moveAnimDuration = 0.4f;
        [SerializeField] private float _scaleAnimDuration = 0.3f;
        [SerializeField] private Ease _animEaseType = Ease.OutBack;

        [Header("Screen Animation")] [SerializeField]
        private RectTransform _screenContainer;

        [SerializeField] private CanvasGroup _screenCanvasGroup;

        [Header("Date Selection Animation")] [SerializeField]
        private RectTransform _dateSelectionRect;

        [SerializeField] private CanvasGroup _dateSelectionCanvasGroup;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private EmotionButton _currentEmotionButton;
        private DateTime _selectedDate;
        private Sequence _screenSequence;
        private Sequence _dateSelectionSequence;

        public event Action BackButtonClicked;
        public event Action<JournalData> JournalCreated;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();

            if (_screenCanvasGroup == null)
                _screenCanvasGroup = GetComponent<CanvasGroup>();

            if (_dateSelectionCanvasGroup == null && _dateSelectionScreen != null)
                _dateSelectionCanvasGroup = _dateSelectionScreen.GetComponent<CanvasGroup>();

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _backButton.onClick.AddListener(OnBackButtonClick);
            _openDateButton.onClick.AddListener(OnOpenDateButtonClick);
            _saveButton.onClick.AddListener(OnSaveButtonClick);
            _saveButton.interactable = false;
            _confirmDateButton.onClick.AddListener(OnDateSelectionDone);
            _closeDateButton.onClick.AddListener(CloseDateSelection);

            foreach (EmotionButton emotionButton in _emotionButtons)
            {
                emotionButton.EmotionSelected += OnEmotionButtonClicked;

                emotionButton.transform.localScale = Vector3.one;
            }

            _titleInput.onValueChanged.AddListener(_ => ValidateSaveButton());
            _descriptionInput.onValueChanged.AddListener(_ => ValidateSaveButton());

            _selectedDate = DateTime.Now;
            _dateText.text = _selectedDate.ToString("dd.MM.yyyy");
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            AnimateScreenIn();
            ResetScreen();
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
            float staggerDelay = 0.1f;

            _titleInput.transform.DOLocalMoveX(10f, 0f);
            _titleInput.transform.DOLocalMoveX(0f, _moveAnimDuration).SetEase(Ease.OutQuint);

            RectTransform dateRectTransform = _dateText.transform.parent.GetComponent<RectTransform>();
            if (dateRectTransform != null)
            {
                dateRectTransform.DOLocalMoveX(10f, 0f);
                dateRectTransform.DOLocalMoveX(0f, _moveAnimDuration).SetDelay(staggerDelay).SetEase(Ease.OutQuint);
            }

            for (int i = 0; i < _emotionButtons.Length; i++)
            {
                _emotionButtons[i].transform.localScale = Vector3.zero;
                _emotionButtons[i].transform.DOScale(1f, _scaleAnimDuration)
                    .SetDelay(staggerDelay * 2 + (i * 0.05f))
                    .SetEase(_animEaseType);
            }

            RectTransform descriptionRectTransform = _descriptionInput.GetComponent<RectTransform>();
            if (descriptionRectTransform != null)
            {
                CanvasGroup descriptionCanvasGroup = _descriptionInput.GetComponent<CanvasGroup>();
                if (descriptionCanvasGroup == null)
                {
                    descriptionCanvasGroup = _descriptionInput.gameObject.AddComponent<CanvasGroup>();
                }

                descriptionCanvasGroup.alpha = 0f;
                descriptionRectTransform.anchoredPosition += new Vector2(0, -20f);

                DOTween.Sequence()
                    .Append(descriptionCanvasGroup.DOFade(1f, _fadeAnimDuration))
                    .Join(descriptionRectTransform.DOAnchorPosY(descriptionRectTransform.anchoredPosition.y + 20f,
                        _moveAnimDuration))
                    .SetDelay(staggerDelay * 3)
                    .SetEase(Ease.OutQuint);
            }

            _saveButton.transform.localScale = Vector3.zero;
            _saveButton.transform.DOScale(1f, _scaleAnimDuration)
                .SetDelay(staggerDelay * 4)
                .SetEase(_animEaseType)
                .OnComplete(() => _saveButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f));
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

        private void OnBackButtonClick()
        {
            _backButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, 10, 1).OnComplete(() =>
            {
                BackButtonClicked?.Invoke();
                Disable();
            });
        }

        private void OnOpenDateButtonClick()
        {
            _openDateButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, 10, 1).OnComplete(() =>
            {
                OpenDateSelection();
            });
        }

        private void OpenDateSelection()
        {
            _dateSelectionScreen.SetActive(true);

            _dateSelectionSequence?.Kill();

            if (_dateSelectionCanvasGroup != null)
                _dateSelectionCanvasGroup.alpha = 0f;

            if (_dateSelectionRect != null)
                _dateSelectionRect.localScale = new Vector3(0.9f, 0.9f, 1f);
            else
                _dateSelectionRect = _dateSelectionScreen.GetComponent<RectTransform>();

            _dateSelectionSequence = DOTween.Sequence();

            if (_dateSelectionCanvasGroup != null)
                _dateSelectionSequence.Join(_dateSelectionCanvasGroup.DOFade(1f, _fadeAnimDuration));

            if (_dateSelectionRect != null)
                _dateSelectionSequence.Join(_dateSelectionRect.DOScale(1f, _scaleAnimDuration).SetEase(_animEaseType));
        }

        private void CloseDateSelection()
        {
            _closeDateButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, 10, 1);

            _dateSelectionSequence?.Kill();
            _dateSelectionSequence = DOTween.Sequence();

            if (_dateSelectionCanvasGroup != null)
                _dateSelectionSequence.Join(_dateSelectionCanvasGroup.DOFade(0f, _fadeAnimDuration));

            if (_dateSelectionRect != null)
                _dateSelectionSequence.Join(_dateSelectionRect.DOScale(0.9f, _scaleAnimDuration).SetEase(Ease.InBack));

            _dateSelectionSequence.OnComplete(() => _dateSelectionScreen.SetActive(false));
        }

        private void OnDateSelectionDone()
        {
            _confirmDateButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, 10, 1).OnComplete(() =>
            {
                SetDate();
                CloseDateSelection();
                ValidateSaveButton();
            });
        }

        private void OnEmotionButtonClicked(EmotionButton button)
        {
            foreach (EmotionButton emotionButton in _emotionButtons)
            {
                emotionButton.SetUnselected();
            }

            _currentEmotionButton = button;
            _currentEmotionButton.SetSelected();

            _currentEmotionButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.3f, 10, 1);

            ValidateSaveButton();
        }

        private void SetDate()
        {
            var selection = _datePickerSettings.Content.Selection;

            _selectedDate = selection.GetItem(0);

            _dateText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f);
            _dateText.text = _selectedDate.ToString("dd.MM.yyyy");
        }

        private void ValidateSaveButton()
        {
            bool isTitleFilled = !string.IsNullOrEmpty(_titleInput.text);
            bool isDescriptionFilled = !string.IsNullOrEmpty(_descriptionInput.text);
            bool isEmotionSelected = _currentEmotionButton != null;
            bool shouldBeInteractable = isTitleFilled && isDescriptionFilled && isEmotionSelected;

            if (shouldBeInteractable != _saveButton.interactable)
            {
                if (shouldBeInteractable)
                {
                    _saveButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f);
                }
            }

            _saveButton.interactable = shouldBeInteractable;
        }

        private void OnSaveButtonClick()
        {
            if (!_saveButton.interactable)
                return;

            _saveButton.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0), 0.3f).OnComplete(() =>
            {
                JournalData journalData = new JournalData
                {
                    Name = _titleInput.text,
                    Description = _descriptionInput.text,
                    Date = _selectedDate,
                    EmotionType = _currentEmotionButton.EmotionType
                };

                JournalCreated?.Invoke(journalData);
                ResetScreen();
                Disable();
            });
        }

        private void ResetScreen()
        {
            _titleInput.text = string.Empty;
            _descriptionInput.text = string.Empty;
            _selectedDate = DateTime.Now;
            _dateText.text = _selectedDate.ToString("dd.MM.yyyy");

            if (_currentEmotionButton != null)
            {
                _currentEmotionButton.SetUnselected();
                _currentEmotionButton = null;
            }

            _saveButton.interactable = false;
        }

        private void OnDisable()
        {
            DOTween.Kill(_screenContainer);
            DOTween.Kill(_screenCanvasGroup);
            DOTween.Kill(_dateSelectionRect);
            DOTween.Kill(_dateSelectionCanvasGroup);
            DOTween.Kill(_titleInput.transform);
            DOTween.Kill(_dateText.transform);
            DOTween.Kill(_saveButton.transform);
            DOTween.Kill(_backButton.transform);
            _screenSequence?.Kill();
            _dateSelectionSequence?.Kill();

            foreach (EmotionButton emotionButton in _emotionButtons)
            {
                DOTween.Kill(emotionButton.transform);
            }

            _backButton.onClick.RemoveListener(OnBackButtonClick);
            _openDateButton.onClick.RemoveListener(OnOpenDateButtonClick);
            _saveButton.onClick.RemoveListener(OnSaveButtonClick);

            foreach (EmotionButton emotionButton in _emotionButtons)
            {
                emotionButton.EmotionSelected -= OnEmotionButtonClicked;
            }

            _titleInput.onValueChanged.RemoveAllListeners();
            _descriptionInput.onValueChanged.RemoveAllListeners();
            _confirmDateButton.onClick.RemoveAllListeners();
            _closeDateButton.onClick.RemoveAllListeners();
        }
    }
}