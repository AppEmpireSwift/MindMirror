using System;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;
using Emotions;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MoodTrackerScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class MoodTrackerScreenController : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private EmotionHolder[] _emotionHolders;
        [SerializeField] private Button _saveButton;
        [SerializeField] private SimpleScrollSnap _scrollSnap;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.5f;

        [SerializeField] private float _emotionEnterDelay = 0.1f;
        [SerializeField] private Ease _emotionEnterEase = Ease.OutBack;
        [SerializeField] private float _saveButtonPopDuration = 0.3f;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private EmotionHolder _currentSelectedEmotion;

        public event Action<EmotionData> DataSaved;
        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackClicked);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _scrollSnap.OnPanelCentered.AddListener(OnPanelChanged);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackClicked);
            _saveButton.onClick.RemoveListener(OnSaveClicked);
            _scrollSnap.OnPanelCentered.RemoveListener(OnPanelChanged);

            _saveButton.transform.DOKill();
            foreach (var holder in _emotionHolders)
            {
                holder.transform.DOKill();
            }
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();

            foreach (var holder in _emotionHolders)
            {
                holder.SetUnselected();
            }

            for (int i = 0; i < _emotionHolders.Length; i++)
            {
                var holder = _emotionHolders[i];
                holder.transform.localScale = Vector3.zero;

                holder.transform.DOScale(1f, _animationDuration)
                    .SetDelay(i * _emotionEnterDelay)
                    .SetEase(_emotionEnterEase);
            }

            OnPanelChanged(0, 0);

            _saveButton.transform.localScale = Vector3.zero;
            _saveButton.transform.DOScale(1f, _animationDuration)
                .SetDelay(_emotionHolders.Length * _emotionEnterDelay)
                .SetEase(Ease.OutBack);

            _saveButton.interactable = false;
        }

        public void Disable()
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < _emotionHolders.Length; i++)
            {
                var holder = _emotionHolders[i];
                sequence.Join(holder.transform.DOScale(0f, _animationDuration / 2)
                    .SetDelay(i * _emotionEnterDelay * 0.5f)
                    .SetEase(Ease.InBack));
            }

            sequence.Join(_saveButton.transform.DOScale(0f, _animationDuration / 2)
                .SetEase(Ease.InBack));

            sequence.OnComplete(() => _screenVisabilityHandler.DisableScreen());
        }

        private void OnReset()
        {
            _scrollSnap.GoToPanel(0);
            _currentSelectedEmotion = null;
            _saveButton.interactable = false;

            foreach (var holder in _emotionHolders)
            {
                holder.SetUnselected();
            }
        }

        private void OnPanelChanged(int start, int end)
        {
            foreach (var emotionHolder in _emotionHolders)
            {
                emotionHolder.SetUnselected();
            }

            _currentSelectedEmotion = _emotionHolders[start];
            _currentSelectedEmotion.SetSelected();

            _saveButton.interactable = true;

            _saveButton.transform.DOScale(1.1f, _saveButtonPopDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                    _saveButton.transform.DOScale(1f, _saveButtonPopDuration)
                        .SetEase(Ease.InOutQuad));
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            Disable();
        }

        private void OnSaveClicked()
        {
            if (_currentSelectedEmotion != null)
            {
                var emotionData = new EmotionData(_currentSelectedEmotion.EmotionType);

                DataSaved?.Invoke(emotionData);

                _saveButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 5, 0.5f)
                    .OnComplete(() =>
                    {
                        OnReset();
                        Disable();
                    });
            }
        }
    }
}