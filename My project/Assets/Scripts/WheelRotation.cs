using UnityEngine;
using DG.Tweening;

public class WheelRotation : MonoBehaviour
{
    [SerializeField] private float duration = 2f; // time for one full rotation
    [SerializeField] private bool clockwise = true;
    [SerializeField] private Ease ease = Ease.Linear;

    private Tween rotationTween;

    [SerializeField] private SpinPuzzleBase _spinPuzzleBase;

    private void OnEnable()
    {
        //StartSpin();
    }

    private void Start()
    {
        _spinPuzzleBase.OnClicked += SetSpeed;
    }

    private void OnDestroy()
    {
        _spinPuzzleBase.OnClicked -= SetSpeed;

    }

    private void OnDisable()
    {
        rotationTween?.Kill();
    }

    private void StartSpin()
    {
        float angle = clockwise ? -360f : 360f;

        rotationTween = transform
            .DORotate(new Vector3(0f, 0f, angle), duration, RotateMode.FastBeyond360)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Incremental)
            .SetRelative(false);
    }

    // Call this if you want to change speed at runtime
    public void SetSpeed(float newDuration)
    {
        duration = newDuration;
        rotationTween?.Kill();
        StartSpin();
    }
}
