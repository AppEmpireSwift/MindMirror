using System;
using DG.Tweening;
using PhotoPicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace NotesScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class OpenNoteScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _back;
        [SerializeField] private Button _edit;
        [SerializeField] private TMP_Text _tagText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private ImagePlacer _imagePlacer;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.5f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _contentPanel;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private NotePlane _currentPlane;
        private Sequence _showSequence;
        private Sequence _hideSequence;

        public event Action<NotePlane> EditClicked;
        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();

            _back.onClick.AddListener(OnBackClicked);
            _edit.onClick.AddListener(OnEditClicked);

            _canvasGroup.alpha = 0f;
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OnDisable()
        {
            _back.onClick.RemoveListener(OnBackClicked);
            _edit.onClick.RemoveListener(OnEditClicked);

            KillAllTweens();
        }

        public void Enable(NotePlane plane)
        {
            _currentPlane = plane ?? throw new ArgumentNullException(nameof(plane));

            _nameText.text = _currentPlane.NoteData.Title;
            _tagText.text = _currentPlane.NoteData.TagType.ToString();
            _descriptionText.text = _currentPlane.NoteData.Description;
            _imagePlacer.SetImage(_currentPlane.NoteData.Photo);

            _screenVisabilityHandler.EnableScreen();

            PlayShowAnimation();
        }

        public void Disable()
        {
            PlayHideAnimation(() => _screenVisabilityHandler.DisableScreen());
        }

        private void PlayShowAnimation()
        {
            KillAllTweens();

            _contentPanel.localScale = Vector3.one * 0.8f;
            _canvasGroup.alpha = 0f;

            _showSequence = DOTween.Sequence();

            _showSequence.Append(_canvasGroup.DOFade(1f, _animationDuration).SetEase(Ease.OutQuad));

            _showSequence.Play();
        }

        private void PlayHideAnimation(Action onComplete = null)
        {
            KillAllTweens();

            _hideSequence = DOTween.Sequence();

            _hideSequence.Append(_contentPanel.DOScale(Vector3.one * 0.8f, _animationDuration * 0.7f)
                .SetEase(Ease.InQuad));

            _hideSequence.Join(_canvasGroup.DOFade(0f, _animationDuration * 0.7f).SetEase(Ease.InQuad));

            if (onComplete != null)
                _hideSequence.OnComplete(() => onComplete.Invoke());

            _hideSequence.Play();
        }

        private void KillAllTweens()
        {
            if (_showSequence != null && _showSequence.IsActive())
                _showSequence.Kill();

            if (_hideSequence != null && _hideSequence.IsActive())
                _hideSequence.Kill();

            DOTween.Kill(_canvasGroup);
            DOTween.Kill(_contentPanel);
            DOTween.Kill(_nameText);
            DOTween.Kill(_tagText);
            DOTween.Kill(_descriptionText);

            if (_back != null)
                DOTween.Kill(_back.GetComponent<RectTransform>());

            if (_edit != null)
                DOTween.Kill(_edit.GetComponent<RectTransform>());
        }

        private void OnEditClicked()
        {
            EditClicked?.Invoke(_currentPlane);
            Disable();
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            Disable();
        }
    }
}