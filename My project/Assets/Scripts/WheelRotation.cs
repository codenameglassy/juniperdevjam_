using UnityEngine;
using DG.Tweening;

public class WheelRotation : MonoBehaviour
{
    [SerializeField] private float baseDuration = 2f; // duration at spin level 1
    [SerializeField] private bool clockwise = true;
    [SerializeField] private Ease ease = Ease.Linear;
    private Tween rotationTween;
    [SerializeField] private SpinPuzzleBase _spinPuzzleBase;

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

    private void StartSpin(float duration)
    {
        rotationTween?.Kill();
        float angle = clockwise ? -360f : 360f;
        rotationTween = transform
            .DORotate(new Vector3(0f, 0f, angle), duration, RotateMode.FastBeyond360)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Incremental)
            .SetRelative(false);
    }

    // spinLevel: 0 = stopped, 1 = slow, 2 = faster, 3 = faster still, 4 = fastest
    public void SetSpeed(int spinLevel)
    {
        rotationTween?.Kill();

        if (spinLevel <= 0)
        {
            // stays stopped, no tween running
            return;
        }

        // Higher spinLevel -> shorter duration -> faster spin
        float duration = baseDuration / spinLevel;
        StartSpin(duration);
    }
}