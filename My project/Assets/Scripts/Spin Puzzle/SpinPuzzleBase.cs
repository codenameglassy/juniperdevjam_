using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class SpinPuzzleBase : MonoBehaviour
{
    [Header("Hover Animation")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float hoverAnimDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    private Tween scaleTween;
    private Vector3 baseScale;
    private Camera mainCamera;

    public event Action<int> OnClicked;

    private int currentSpinInt = 0;
    public int CurrentSpinInt => currentSpinInt;

    private const int maxSpinValue = 4;

    private void Awake()
    {
        mainCamera = Camera.main;
        baseScale = transform.localScale;
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
        //sfx
        SoundManager.Instance.PlayOneShot("click");

        SwitchCurrentSpinInt();
        OnClicked?.Invoke(currentSpinInt);
    }

    void SwitchCurrentSpinInt()
    {
        currentSpinInt = (currentSpinInt + 1) % (maxSpinValue + 1); // wraps 0->1->2->3->4->0
    }

    // Called by CodeLockManager to reset this dial on failure
    public void ResetSpin()
    {
        currentSpinInt = 0;
        OnClicked?.Invoke(currentSpinInt); // so UI/rotation visually reset too
    }
}