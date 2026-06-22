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


    public event Action <float> OnClicked;
    public float currentSpinInt;
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
        //Notify subscribers Onmousedown
        Debug.Log(gameObject.name + " clicked");
        //change spin int
        SwitchCurrentSpinInt();
        OnClicked?.Invoke(currentSpinInt);

       

    }

    void SwitchCurrentSpinInt()
    {
        switch(currentSpinInt)
        {
            case 4:
                currentSpinInt = 3;
                break;

            case 3:
                currentSpinInt = 2;
                break;

            case 2:
                currentSpinInt = 1;
                break;

            case 1:
                currentSpinInt = 4;
                break;

        }
    }
}
