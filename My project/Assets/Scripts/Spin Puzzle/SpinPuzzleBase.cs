using DG.Tweening;
using System;
using UnityEngine;

public class SpinPuzzleBase : MonoBehaviour
{
    [Header("Hover Animation")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float hoverAnimDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    [Header("Click Pop Animation")]
    [SerializeField] private float punchScaleAmount = 0.25f;
    [SerializeField] private float punchDuration = 0.3f;
    [SerializeField] private int punchVibrato = 6;
    [SerializeField] private float punchElasticity = 0.6f;

    [Header("Code")]
    [Tooltip("The digit (0-4) this dial must land on to be correct.")]
    [SerializeField] private int targetDigit = 0;

    private Tween scaleTween;
    private Tween punchTween;
    private Vector3 baseScale;
    private Camera mainCamera;

    public event Action<int> OnClicked;

    private int currentSpinInt = 0;
    public int CurrentSpinInt => currentSpinInt;
    public bool IsCorrect => currentSpinInt == targetDigit;

    private const int maxSpinValue = 4;

    [Header("Live-Feedback")]
    public GameObject eyeBallOpen;
    public GameObject eyeBallClose;

    private BoxCollider2D boxCollider2d;

    // Remove the isGameplay field entirely. Keep isParticipating:
    private bool isParticipating = true;
    private void Awake()
    {
        mainCamera = Camera.main;
        baseScale = transform.localScale;
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        ApplyActiveState(); // seed from the real state, not a guess
    }

    private void Start()
    {
        boxCollider2d = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnMouseEnter()
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale * hoverScaleMultiplier, hoverAnimDuration).SetEase(hoverEase);
    }

    private void OnMouseExit()
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale, hoverAnimDuration).SetEase(hoverEase);
    }

    private void OnMouseDown()
    {
        Debug.Log(gameObject.name + " clicked");
        //notify Level Observer
        LevelObserver.Instance.NotifySpinPuzzleBaseTapped();

        //sfx
        SoundManager.Instance.PlayOneShot("click");

        //scale tween
        PlayClickPop();

        SwitchCurrentSpinInt();
        OnClicked?.Invoke(currentSpinInt);
        CheckSelf();
    }

    private void PlayClickPop()
    {
        // Kill hover tween so it doesn't fight the punch over localScale
        scaleTween?.Kill();
        punchTween?.Kill();
        transform.localScale = baseScale;

        punchTween = transform.DOPunchScale(Vector3.one * punchScaleAmount, punchDuration, punchVibrato, punchElasticity)
            .OnComplete(() => transform.localScale = baseScale);
    }

    void SwitchCurrentSpinInt()
    {
        currentSpinInt = (currentSpinInt + 1) % (maxSpinValue + 1); // wraps 0->1->2->3->4->0
    }

    private void CheckSelf()
    {
        switch (IsCorrect)
        {
            case true:
                eyeBallClose.SetActive(false);
                eyeBallOpen.SetActive(true);
                SoundManager.Instance.PlayOneShot("correctcode");
                break;
            case false:
                eyeBallClose.SetActive(true);
                eyeBallOpen.SetActive(false);
                break;
        }
    }

    // Called by CodeLockManager to set/sync this dial's target digit from the CodeLockData
    public void SetTargetDigit(int digit)
    {
        targetDigit = digit;
    }

    // Called by CodeLockManager: whether this dial is part of the current level's code.
    public void SetParticipating(bool participating)
    {
        isParticipating = participating;
        ApplyActiveState();
    }

    // Called by CodeLockManager to reset this dial on failure
    public void ResetSpin()
    {
        currentSpinInt = 0;
        OnClicked?.Invoke(currentSpinInt);

        // Refresh eye visuals so a previously-correct dial doesn't keep its open eye.
        if (eyeBallOpen != null) eyeBallOpen.SetActive(false);
        if (eyeBallClose != null) eyeBallClose.SetActive(true);
    }


    private void OnGameStateChanged(GameState newGameState)
    {
        ApplyActiveState();
    }

    // Active only if this dial is in the current code AND we're actually in gameplay.
    private void ApplyActiveState()
    {
        bool gameplay = GameStateManager.Instance != null
                     && GameStateManager.Instance.CurrentGameState == GameState.Gameplay;
        gameObject.SetActive(isParticipating && gameplay);
    }
}