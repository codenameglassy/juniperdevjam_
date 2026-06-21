using System;
using UnityEngine;
using DG.Tweening;
 public class PopupEffect2D : MonoBehaviour
    {
        [Header("Scale")]
        [SerializeField] private float _startScaleMultiplier = 0.4f;
        [SerializeField] private float _overshootScaleMultiplier = 1.15f;

        [Header("Timing")]
        [SerializeField] private float _popDuration = 0.28f;
        [SerializeField] private float _settleDuration = 0.18f;
        [SerializeField] private Ease _popEase = Ease.OutBack;
        [SerializeField] private Ease _settleEase = Ease.OutSine;

        [Header("Fade (requires SpriteRenderer)")]
        [SerializeField] private bool _fadeIn = true;
        [SerializeField] private float _fadeDuration = 0.15f;

        [Header("Rise (optional, e.g. damage numbers / pickups)")]
        [SerializeField] private bool _riseOnPlay = false;
        [SerializeField] private float _riseDistance = 0.5f;
        [SerializeField] private float _riseDuration = 0.4f;
        [SerializeField] private Ease _riseEase = Ease.OutCubic;

        [Header("Wobble (optional)")]
        [SerializeField] private bool _wobbleOnEnd = false;
        [SerializeField] private float _wobbleStrength = 8f;
        [SerializeField] private int _wobbleVibrato = 4;

        private SpriteRenderer _spriteRenderer;
        private Vector3 _baseLocalPos;
        private Vector3 _originalScale;   // <-- the real "home" scale
        private Sequence _sequence;

        public event Action OnPopupComplete;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _baseLocalPos = transform.localPosition;
            _originalScale = transform.localScale; // cache once, before any popup runs
        }

        public void Play()
        {
            _sequence?.Kill();

            transform.localScale = _originalScale * _startScaleMultiplier;
            transform.localPosition = _baseLocalPos;

            if (_fadeIn && _spriteRenderer != null)
            {
                var c = _spriteRenderer.color;
                _spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);
            }

            _sequence = DOTween.Sequence();

            if (_fadeIn && _spriteRenderer != null)
                _sequence.Join(_spriteRenderer.DOFade(1f, _fadeDuration));

            if (_riseOnPlay)
            {
                _sequence.Join(transform.DOLocalMoveY(
                    _baseLocalPos.y + _riseDistance, _riseDuration).SetEase(_riseEase));
            }

            _sequence.Append(transform.DOScale(_originalScale * _overshootScaleMultiplier, _popDuration).SetEase(_popEase));
            _sequence.Append(transform.DOScale(_originalScale, _settleDuration).SetEase(_settleEase)); // <-- back to true original

            if (_wobbleOnEnd)
                _sequence.Append(transform.DOShakeRotation(0.3f, _wobbleStrength, _wobbleVibrato));

            _sequence.OnComplete(() => OnPopupComplete?.Invoke());
        }

        public void PlayReverse(Action onComplete = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(_originalScale * _startScaleMultiplier, _popDuration * 0.7f).SetEase(Ease.InBack));

            if (_fadeIn && _spriteRenderer != null)
                _sequence.Join(_spriteRenderer.DOFade(0f, _fadeDuration));

            _sequence.OnComplete(() => onComplete?.Invoke());
        }

        private void OnDestroy() => _sequence?.Kill();
    }
