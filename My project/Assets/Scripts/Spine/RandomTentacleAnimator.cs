using UnityEngine;
using Spine.Unity;
using Spine;

[RequireComponent(typeof(SkeletonAnimation))]
public class RandomTentacleAnimator : MonoBehaviour
{
    [Header("Spine Animation Names")]
    [SerializeField]
    private string[] animationNames = new string[]
    {
        "Idle",
        "Idle2",
        "Idle3"
    };

    [Header("Timing")]
    [SerializeField] private float minDelay = 1.5f;
    [SerializeField] private float maxDelay = 4f;

    [Header("Behavior")]
    [SerializeField] private bool avoidRepeatingSameAnimation = true;
    [SerializeField] private bool loopAnimation = false;

    private SkeletonAnimation skeletonAnimation;
    private int lastIndex = -1;
    private float timer;
    private bool isPlaying;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable()
    {
        skeletonAnimation.AnimationState.Complete += OnAnimationComplete;
        ScheduleNext();
    }

    private void OnDisable()
    {
        skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
    }

    private void Update()
    {
        if (isPlaying) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PlayRandomAnimation();
        }
    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        isPlaying = false;
        ScheduleNext();
    }

    private void ScheduleNext()
    {
        timer = Random.Range(minDelay, maxDelay);
    }

    private void PlayRandomAnimation()
    {
        if (animationNames.Length == 0) return;

        int index = GetRandomIndex();
        lastIndex = index;

        skeletonAnimation.AnimationState.SetAnimation(0, animationNames[index], loopAnimation);
        isPlaying = true;
    }

    private int GetRandomIndex()
    {
        if (!avoidRepeatingSameAnimation || animationNames.Length <= 1)
            return Random.Range(0, animationNames.Length);

        int index;
        do
        {
            index = Random.Range(0, animationNames.Length);
        } while (index == lastIndex);

        return index;
    }
}