using System;
using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using Stress;
using UnityEngine;
using UnityEngine.UI;

namespace StressLevelScreen
{
    public class StressLevelScreenController : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private StressLevelHolder[] _stressLevelHolders;
        [SerializeField] private Button _saveButton;
        [SerializeField] private SimpleScrollSnap _scrollSnap;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.5f;

        [SerializeField] private float _emotionEnterDelay = 0.1f;
        [SerializeField] private Ease _emotionEnterEase = Ease.OutBack;
        [SerializeField] private float _saveButtonPopDuration = 0.3f;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private StressLevelHolder _currentSelectedEmotion;

        public event Action<StressData> DataSaved;
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
            foreach (var holder in _stressLevelHolders)
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

            foreach (var holder in _stressLevelHolders)
            {
                holder.SetUnselected();
            }

            for (int i = 0; i < _stressLevelHolders.Length; i++)
            {
                var holder = _stressLevelHolders[i];
                holder.transform.localScale = Vector3.zero;

                holder.transform.DOScale(1f, _animationDuration)
                    .SetDelay(i * _emotionEnterDelay)
                    .SetEase(_emotionEnterEase);
            }

            OnPanelChanged(0, 0);

            _saveButton.transform.localScale = Vector3.zero;
            _saveButton.transform.DOScale(1f, _animationDuration)
                .SetDelay(_stressLevelHolders.Length * _emotionEnterDelay)
                .SetEase(Ease.OutBack);

            _saveButton.interactable = false;
        }

        public void Disable()
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < _stressLevelHolders.Length; i++)
            {
                var holder = _stressLevelHolders[i];
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

            foreach (var holder in _stressLevelHolders)
            {
                holder.SetUnselected();
            }
        }

        private void OnPanelChanged(int start, int end)
        {
            foreach (var emotionHolder in _stressLevelHolders)
            {
                emotionHolder.SetUnselected();
            }

            _currentSelectedEmotion = _stressLevelHolders[start];
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
                var emotionData = new StressData(_currentSelectedEmotion.StressType);

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