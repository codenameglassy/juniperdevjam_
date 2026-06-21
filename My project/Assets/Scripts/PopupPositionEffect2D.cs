using System;
using UnityEngine;
using DG.Tweening;
    public class PopupPositionEffect2D : MonoBehaviour
    {
        public enum SlideDirection { Left, Right, Up, Down, Custom }

        [Header("Direction")]
        [SerializeField] private SlideDirection _direction = SlideDirection.Up;
        [SerializeField] private Vector2 _customOffset = new Vector2(0f, 1f);
        [SerializeField] private float _offsetDistance = 1f;

        [Header("Timing - In")]
        [SerializeField] private float _popInDuration = 0.3f;
        [SerializeField] private Ease _popInEase = Ease.OutBack;

        [Header("Timing - Out")]
        [SerializeField] private float _popOutDuration = 0.25f;
        [SerializeField] private Ease _popOutEase = Ease.InBack;

        [Header("Fade (optional, requires SpriteRenderer)")]
        [SerializeField] private bool _fadeAlongside = true;
        [SerializeField] private float _fadeDuration = 0.15f;

        private SpriteRenderer _spriteRenderer;
        private Vector3 _homeLocalPos;
        private Sequence _sequence;

        public event Action OnPopInComplete;
        public event Action OnPopOutComplete;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _homeLocalPos = transform.localPosition; // the "settled" position
        }

    private void Start()
    {
        _spriteRenderer.enabled = false;
    }

    private Vector3 GetOffset()
        {
            Vector2 dir = _direction switch
            {
                SlideDirection.Left => Vector2.left,
                SlideDirection.Right => Vector2.right,
                SlideDirection.Up => Vector2.up,
                SlideDirection.Down => Vector2.down,
                SlideDirection.Custom => _customOffset.normalized,
                _ => Vector2.up
            };
            return (Vector3)(dir * _offsetDistance);
        }

        /// <summary>Slides in from the offset to the home position.</summary>
        public void PlayIn()
        {
            _sequence?.Kill();

            _spriteRenderer.enabled = true;

            Vector3 startPos = _homeLocalPos + GetOffset();
            transform.localPosition = startPos;

            if (_fadeAlongside && _spriteRenderer != null)
            {
                var c = _spriteRenderer.color;
                _spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);
            }

            _sequence = DOTween.Sequence();

            if (_fadeAlongside && _spriteRenderer != null)
                _sequence.Join(_spriteRenderer.DOFade(1f, _fadeDuration));

            _sequence.Join(transform.DOLocalMove(_homeLocalPos, _popInDuration).SetEase(_popInEase));
            _sequence.OnComplete(() => OnPopInComplete?.Invoke());
        }

        /// <summary>Slides out from the home position to the offset.</summary>
        public void PlayOut()
        {
            _sequence?.Kill();

            Vector3 exitPos = _homeLocalPos + GetOffset();

            _sequence = DOTween.Sequence();
            _sequence.Join(transform.DOLocalMove(exitPos, _popOutDuration).SetEase(_popOutEase));

            if (_fadeAlongside && _spriteRenderer != null)
                _sequence.Join(_spriteRenderer.DOFade(0f, _fadeDuration));

            _sequence.OnComplete(() => OnPopOutComplete?.Invoke());

            _spriteRenderer.enabled = false;
        }

        /// <summary>Re-caches home position — call if you reposition this object before reuse (e.g. pooling).</summary>
        public void RefreshHomePosition()
        {
            _homeLocalPos = transform.localPosition;
        }

        private void OnDestroy() => _sequence?.Kill();
    }
