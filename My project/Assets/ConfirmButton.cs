using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class ConfirmButton : MonoBehaviour
{
    public CodeLockManager codeLockManager;
    public SpriteRenderer spriteRenderer;

    [Header("Hover Animation")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float hoverAnimDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    [Header("Click Pop Animation")]
    [SerializeField] private float punchScaleAmount = 0.25f;
    [SerializeField] private float punchDuration = 0.3f;
    [SerializeField] private int punchVibrato = 6;
    [SerializeField] private float punchElasticity = 0.6f;

    private Tween scaleTween;
    private Tween punchTween;
    private Vector3 baseScale;

    [Header("Sprites")]
    public Sprite pressed;
    public Sprite unpressed;

    [Header("UI")]
    public GameObject rewardPanel;
   

  

    private void Awake()
    {
        baseScale = transform.localScale;
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    private void Start()
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
        //change sprite
        spriteRenderer.sprite = pressed;

        PlayClickPop();
        codeLockManager.OnConfirmPressed();
    }

    private void PlayClickPop()
    {
        // Kill hover tween so it doesn't fight the punch over localScale
        scaleTween?.Kill();
        punchTween?.Kill();

        transform.localScale = baseScale;
        punchTween = transform.DOPunchScale(Vector3.one * punchScaleAmount, punchDuration, punchVibrato, punchElasticity)
            .OnComplete(() => 
            transform.localScale = baseScale).OnComplete(() => rewardPanel.SetActive(true));
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        bool isactive = newGameState == GameState.Gameplay;
        gameObject.SetActive(isactive);
       
    }
}