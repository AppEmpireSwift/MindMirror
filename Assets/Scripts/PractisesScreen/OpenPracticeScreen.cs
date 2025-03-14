using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace PractisesScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class OpenPracticeScreen : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _pausePlayButton;
        [SerializeField] private Sprite _pauseSprite;
        [SerializeField] private Sprite _playSprite;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private Image _filledImage;
        [SerializeField] private RectTransform _contentContainer;
        [SerializeField] private TMP_Text _nameText;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.5f;

        [SerializeField] private Ease _animationEase = Ease.OutBack;
        [SerializeField] private float _buttonAnimationScale = 1.1f;
        [SerializeField] private float _progressAnimationDuration = 1f;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private bool _isFromMainScreen;
        private PracticeData _currentPracticeData;
        private int _remainingTime;
        private int _totalTime;
        private bool _isPaused = true;
        private Coroutine _timerCoroutine;
        private Sequence _animationSequence;
        private Tween _progressTween;

        public event Action BackClicked;
        public event Action BackFromMainClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();

            _backButton.onClick.AddListener(OnBackButtonClicked);
            _pausePlayButton.onClick.AddListener(OnPausePlayButtonClicked);

            SetupButtonAnimation(_backButton);
            SetupButtonAnimation(_pausePlayButton);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _pausePlayButton.onClick.RemoveListener(OnPausePlayButtonClicked);

            StopTimer();
            KillAllAnimations();
        }

        private void Start()
        {
            Disable();
        }

        public void Enable(PracticeData data, bool fromMain = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _screenVisabilityHandler.EnableScreen();
            _isFromMainScreen = fromMain;

            _currentPracticeData = data;
            _remainingTime = data.TotalSeconds;
            _totalTime = data.TotalSeconds;
            _isPaused = true;
            _nameText.text = data.Title;

            UpdateTimerDisplay();
            UpdatePausePlayButton();

            _filledImage.fillAmount = 0f;

            PlayEnterAnimation();
        }

        public void Disable()
        {
            StopTimer();
            _screenVisabilityHandler.DisableScreen();
            KillAllAnimations();
        }

        private void OnBackButtonClicked()
        {
            StopTimer();

            PlayExitAnimation(() =>
            {
                if (_isFromMainScreen)
                {
                    BackFromMainClicked?.Invoke();
                }
                else
                {
                    BackClicked?.Invoke();
                }

                Disable();
            });
        }

        private void OnPausePlayButtonClicked()
        {
            _pausePlayButton.transform
                .DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f)
                .SetEase(Ease.OutQuad);

            if (_isPaused)
            {
                StartTimer();
            }
            else
            {
                PauseTimer();
            }
        }

        private void StartTimer()
        {
            if (_remainingTime <= 0)
                return;

            _isPaused = false;
            UpdatePausePlayButton();

            StopTimer();
            _timerCoroutine = StartCoroutine(TimerCoroutine());
        }

        private void PauseTimer()
        {
            _isPaused = true;
            UpdatePausePlayButton();
            StopTimer();
        }

        private void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }

            if (_progressTween != null && _progressTween.IsActive())
            {
                _progressTween.Kill();
                _progressTween = null;
            }
        }

        private IEnumerator TimerCoroutine()
        {
            while (_remainingTime > 0)
            {
                yield return new WaitForSeconds(1f);
                _remainingTime--;
                UpdateTimerDisplay();
                UpdateProgressBarAnimated();

                if (_remainingTime <= 0)
                {
                    OnTimerComplete();
                    break;
                }
            }
        }

        private void UpdateTimerDisplay()
        {
            int minutes = _remainingTime / 60;
            int seconds = _remainingTime % 60;

            _timerText.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 1, 0.5f);
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        private void UpdatePausePlayButton()
        {
            Image buttonImage = _pausePlayButton.GetComponent<Image>();

            buttonImage.DOFade(0f, 0.15f).OnComplete(() =>
            {
                buttonImage.sprite = _isPaused ? _playSprite : _pauseSprite;
                buttonImage.DOFade(1f, 0.15f);
            });
        }

        private void UpdateProgressBarAnimated()
        {
            if (_totalTime <= 0)
            {
                _filledImage.fillAmount = 1f;
                return;
            }

            float targetProgress = 1f - ((float)_remainingTime / _totalTime);

            if (_progressTween != null && _progressTween.IsActive())
            {
                _progressTween.Kill();
            }

            _progressTween = _filledImage.DOFillAmount(targetProgress, _progressAnimationDuration)
                .SetEase(Ease.OutQuad);
        }

        private void OnTimerComplete()
        {
            _isPaused = true;
            UpdatePausePlayButton();

            // Анимация завершения таймера
            Sequence completeSequence = DOTween.Sequence();
            completeSequence.Append(_timerText.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack));
            completeSequence.Append(_timerText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad));
            completeSequence.Play();
        }

        private void PlayEnterAnimation()
        {
            KillAnimationSequence();

            if (_contentContainer != null)
            {
                _contentContainer.localScale = Vector3.zero;

                _animationSequence = DOTween.Sequence();
                _animationSequence.Append(_contentContainer.DOScale(1f, _animationDuration).SetEase(_animationEase));
                _animationSequence.Play();
            }
        }

        private void PlayExitAnimation(Action onComplete = null)
        {
            KillAnimationSequence();

            if (_contentContainer != null)
            {
                _animationSequence = DOTween.Sequence();
                _animationSequence.Append(_contentContainer.DOScale(0f, _animationDuration).SetEase(Ease.InBack));

                if (onComplete != null)
                {
                    _animationSequence.OnComplete(() => onComplete.Invoke());
                }

                _animationSequence.Play();
            }
            else if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }

        private void SetupButtonAnimation(Button button)
        {
            if (button == null) return;

            ButtonAnimationHandler animHandler = button.gameObject.GetComponent<ButtonAnimationHandler>();
            if (animHandler == null)
            {
                animHandler = button.gameObject.AddComponent<ButtonAnimationHandler>();
            }

            animHandler.SetAnimationSettings(_buttonAnimationScale);
        }

        private class ButtonAnimationHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
            IPointerDownHandler, IPointerUpHandler
        {
            private Vector3 _originalScale;
            private float _scaleMultiplier = 1.1f;

            private void Awake()
            {
                _originalScale = transform.localScale;
            }

            public void SetAnimationSettings(float scaleMultiplier)
            {
                _scaleMultiplier = scaleMultiplier;
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                transform.DOScale(_originalScale * _scaleMultiplier, 0.2f).SetEase(Ease.OutQuad);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                transform.DOScale(_originalScale, 0.2f).SetEase(Ease.OutQuad);
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                transform.DOScale(_originalScale * 0.95f, 0.1f).SetEase(Ease.OutQuad);
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                transform.DOScale(_originalScale * _scaleMultiplier, 0.1f).SetEase(Ease.OutQuad);
            }
        }

        private void KillAnimationSequence()
        {
            if (_animationSequence != null && _animationSequence.IsActive())
            {
                _animationSequence.Kill(true);
                _animationSequence = null;
            }
        }

        private void KillAllAnimations()
        {
            KillAnimationSequence();

            if (_progressTween != null && _progressTween.IsActive())
            {
                _progressTween.Kill();
                _progressTween = null;
            }

            DOTween.Kill(_timerText.transform);
            DOTween.Kill(_backButton.transform);
            DOTween.Kill(_pausePlayButton.transform);
            DOTween.Kill(_filledImage);

            if (_contentContainer != null)
            {
                DOTween.Kill(_contentContainer);
            }
        }
    }
}