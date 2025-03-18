using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace NotesScreen.CreateNote
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class CreateNoteScreen : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _titleInput;
        [SerializeField] private Button _tagButton;
        [SerializeField] private GameObject _tagSelectionScreen;
        [SerializeField] private TagElement[] _tagElements;
        [SerializeField] private TMP_Text _tagText;
        [SerializeField] private Button _confirmTagButton;
        [SerializeField] private Button _cancelTagButton;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private PhotosController _photosController;
        [SerializeField] private Button _saveButton;
        [SerializeField] private TMP_Text _screenTitleText;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.3f;

        [SerializeField] private Ease _easeType = Ease.OutBack;
        [SerializeField] private RectTransform _mainPanel;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private TagType _selectedType;
        private byte[] _selectedPhoto;
        private Sequence _showSequence;
        private Sequence _hideSequence;
        private bool _isEditMode = false;
        private NoteData _editingNoteData;

        public event Action BackClicked;
        public event Action<NoteData> NoteCreated;
        public event Action<NoteData> NoteUpdated;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _saveButton.interactable = false;
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackClicked);
            _tagButton.onClick.AddListener(OpenTagSelection);
            _confirmTagButton.onClick.AddListener(CloseTagSelection);
            _cancelTagButton.onClick.AddListener(CloseTagSelection);
            _saveButton.onClick.AddListener(OnSaveClicked);

            _photosController.SetPhoto += SetPhoto;

            _titleInput.onValueChanged.AddListener(_ => ValidateData());
            _descriptionInput.onValueChanged.AddListener(_ => ValidateData());

            foreach (TagElement tagElement in _tagElements)
            {
                tagElement.TagSelected += OnTagSelected;
            }
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackClicked);
            _tagButton.onClick.RemoveListener(OpenTagSelection);
            _confirmTagButton.onClick.RemoveListener(CloseTagSelection);
            _cancelTagButton.onClick.RemoveListener(CloseTagSelection);
            _saveButton.onClick.RemoveListener(OnSaveClicked);

            _photosController.SetPhoto -= SetPhoto;

            _titleInput.onValueChanged.RemoveAllListeners();
            _descriptionInput.onValueChanged.RemoveAllListeners();

            foreach (TagElement tagElement in _tagElements)
            {
                tagElement.TagSelected -= OnTagSelected;
            }

            DOTween.Kill(this);
        }

        private void Start()
        {
            ResetScreen();
            _screenVisabilityHandler.DisableScreen();

            if (_mainPanel != null)
            {
                _mainPanel.localScale = Vector3.zero;
            }
        }

        public void Enable()
        {
            _isEditMode = false;
            UpdateScreenTitle();
            ResetScreen();
            _screenVisabilityHandler.EnableScreen();
            PlayShowAnimation();
        }

        public void EnableEditMode(NoteData noteData)
        {
            _isEditMode = true;
            _editingNoteData = noteData;
            UpdateScreenTitle();

            _titleInput.text = noteData.Title;
            _descriptionInput.text = noteData.Description;
            _selectedType = noteData.TagType;
            _tagText.text = _selectedType.ToString();
            _selectedPhoto = noteData.Photo;

            _photosController.SetPhotos(noteData.Photo);

            ValidateData();

            _screenVisabilityHandler.EnableScreen();
            PlayShowAnimation();
        }

        public void Disable()
        {
            PlayHideAnimation(() =>
            {
                _screenVisabilityHandler.DisableScreen();
                ResetScreen();
            });
        }

        private void UpdateScreenTitle()
        {
            if (_screenTitleText != null)
            {
                _screenTitleText.text = _isEditMode ? "Edit Note" : "Create Note";
            }
        }

        private void PlayShowAnimation()
        {
            DOTween.Kill(this);

            _showSequence = DOTween.Sequence().SetId(this);

            if (_mainPanel != null)
            {
                _mainPanel.localScale = Vector3.zero;
                _showSequence.Append(_mainPanel.DOScale(1f, _animationDuration).SetEase(_easeType));
            }

            float delay = _animationDuration * 0.5f;

            AnimateElementIn(_backButton.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementIn(_titleInput.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementIn(_tagButton.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementIn(_descriptionInput.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementIn(_photosController.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementIn(_saveButton.transform as RectTransform, delay);

            _showSequence.Play();
        }

        private void AnimateElementIn(RectTransform element, float delay)
        {
            if (element == null) return;

            element.localScale = Vector3.zero;

            _showSequence.Insert(delay, element.DOScale(1f, _animationDuration).SetEase(_easeType));

            var canvasGroup = element.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                _showSequence.Insert(delay, canvasGroup.DOFade(1f, _animationDuration).SetEase(Ease.InQuad));
            }
        }

        private void PlayHideAnimation(Action onComplete = null)
        {
            DOTween.Kill(this);

            _hideSequence = DOTween.Sequence().SetId(this);

            float delay = 0f;

            AnimateElementOut(_saveButton.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementOut(_photosController.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementOut(_descriptionInput.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementOut(_tagButton.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementOut(_titleInput.transform as RectTransform, delay);
            delay += 0.05f;

            AnimateElementOut(_backButton.transform as RectTransform, delay);

            if (_mainPanel != null)
            {
                _hideSequence.Append(_mainPanel.DOScale(0f, _animationDuration).SetEase(Ease.InBack));
            }

            if (onComplete != null)
            {
                _hideSequence.OnComplete(() => onComplete.Invoke());
            }

            _hideSequence.Play();
        }

        private void AnimateElementOut(RectTransform element, float delay)
        {
            if (element == null) return;

            _hideSequence.Insert(delay, element.DOScale(0f, _animationDuration).SetEase(Ease.InBack));

            var canvasGroup = element.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                _hideSequence.Insert(delay, canvasGroup.DOFade(0f, _animationDuration).SetEase(Ease.OutQuad));
            }
        }

        private void AnimateTagSelection(bool show)
        {
            if (_tagSelectionScreen == null)
                return;

            var canvasGroup = _tagSelectionScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = _tagSelectionScreen.AddComponent<CanvasGroup>();
            }

            var rectTransform = _tagSelectionScreen.GetComponent<RectTransform>();

            if (show)
            {
                canvasGroup.alpha = 0;
                rectTransform.localScale = Vector3.zero;

                Sequence sequence = DOTween.Sequence();
                sequence.Append(rectTransform.DOScale(1, _animationDuration).SetEase(_easeType));
                sequence.Join(canvasGroup.DOFade(1, _animationDuration).SetEase(Ease.OutQuad));
                sequence.Play();
            }
            else
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(rectTransform.DOScale(0, _animationDuration).SetEase(Ease.InBack));
                sequence.Join(canvasGroup.DOFade(0, _animationDuration).SetEase(Ease.InQuad));
                sequence.OnComplete(() => _tagSelectionScreen.SetActive(false));
                sequence.Play();
            }
        }

        private void AnimateButtonClick(Button button)
        {
            if (button == null)
                return;

            Sequence clickSequence = DOTween.Sequence();
            clickSequence.Append(button.transform.DOScale(0.9f, 0.1f).SetEase(Ease.OutQuad));
            clickSequence.Append(button.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack));
            clickSequence.Play();
        }

        private void SetType(TagType tagType)
        {
            _selectedType = tagType;
            _tagText.text = _selectedType.ToString();

            Sequence tagSequence = DOTween.Sequence();
            tagSequence.Append(_tagText.transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad));
            tagSequence.Append(_tagText.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad));
            tagSequence.Play();

            ValidateData();
        }

        private void OnTagSelected(TagElement tagElement)
        {
            SetType(tagElement.TagType);

            AnimateButtonClick(tagElement.GetComponent<Button>());
        }

        private void OpenTagSelection()
        {
            _tagSelectionScreen.SetActive(true);
            AnimateTagSelection(true);
            AnimateButtonClick(_tagButton);
        }

        private void CloseTagSelection()
        {
            AnimateTagSelection(false);
            ValidateData();

            if (_confirmTagButton.gameObject.activeInHierarchy)
                AnimateButtonClick(_confirmTagButton);
            else
                AnimateButtonClick(_cancelTagButton);
        }

        private void SetPhoto()
        {
            _selectedPhoto = _photosController.GetPhoto();

            ValidateData();
        }

        private void OnBackClicked()
        {
            AnimateButtonClick(_backButton);
            BackClicked?.Invoke();
            Disable();
        }

        private void OnSaveClicked()
        {
            if (!IsDataValid())
                return;

            AnimateButtonClick(_saveButton);

            var noteData = new NoteData
            {
                TagType = _selectedType,
                Photo = _selectedPhoto,
                Title = _titleInput.text,
                Description = _descriptionInput.text
            };

            if (_isEditMode && _editingNoteData != null)
            {
                NoteUpdated?.Invoke(noteData);
            }
            else
            {
                NoteCreated?.Invoke(noteData);
            }

            ResetScreen();
            Disable();
            BackClicked?.Invoke();
        }

        private bool IsDataValid()
        {
            bool isTitleValid = !string.IsNullOrWhiteSpace(_titleInput.text);
            bool isDescriptionValid = !string.IsNullOrWhiteSpace(_descriptionInput.text);
            bool isPhotoSelected = _selectedPhoto != null && _selectedPhoto.Length > 0;
            bool isTagSelected = _selectedType != TagType.None;

            return isTitleValid && isDescriptionValid && isPhotoSelected && isTagSelected;
        }

        private void ValidateData()
        {
            bool isValid = IsDataValid();
            if (isValid != _saveButton.interactable)
            {
                _saveButton.interactable = isValid;

                if (isValid)
                {
                    _saveButton.transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack)
                        .OnComplete(() => _saveButton.transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad));
                }
            }
        }

        private void ResetScreen()
        {
            if (!_isEditMode)
            {
                _titleInput.text = "";
                _descriptionInput.text = "";
                _selectedPhoto = null;
                _selectedType = default;
                _tagText.text = "Select Tag";
                _selectedType = TagType.None;
                _saveButton.interactable = false;
                _photosController.ResetPhotos();
            }
        }
    }
}