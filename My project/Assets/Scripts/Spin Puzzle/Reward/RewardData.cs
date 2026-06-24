using UnityEngine;

[CreateAssetMenu(fileName = "NewRewardData", menuName = "PuzzleLock/Reward Data")]
public class RewardData : ScriptableObject
{
    [System.Serializable]
    public class LevelReward
    {
        [TextArea]
        public string rewardText = "Well done!";
        public Sprite rewardSprite;
    }

    [Tooltip("One reward per level. Element 0 = Level 1, Element 1 = Level 2, etc.")]
    [SerializeField]
    private LevelReward[] levelRewards = new LevelReward[]
    {
        new LevelReward { rewardText = "Level 1 cleared!" },
        new LevelReward { rewardText = "Level 2 cleared!" },
        new LevelReward { rewardText = "Level 3 cleared!" },
    };

    public int LevelCount => levelRewards.Length;

    // levelNumber is 1-based. Clamps so an out-of-range level still returns a valid reward.
    public LevelReward GetRewardForLevel(int levelNumber)
    {
        if (levelRewards == null || levelRewards.Length == 0)
            return null;

        int index = Mathf.Clamp(levelNumber - 1, 0, levelRewards.Length - 1);
        return levelRewards[index];
    }
}