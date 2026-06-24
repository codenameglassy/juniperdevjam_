using UnityEngine;

[CreateAssetMenu(fileName = "NewCodeLockData", menuName = "PuzzleLock/Code Lock Data")]
public class CodeLockData : ScriptableObject
{
    [System.Serializable]
    public class LevelCode
    {
        [Tooltip("One entry per dial, left to right. Length 2-5, digits 0-4.")]
        public int[] code = new int[] { 2, 4, 2, 4 };
    }

    [Tooltip("One code per level. Element 0 = Level 1, Element 1 = Level 2, etc.")]
    [SerializeField]
    private LevelCode[] levelCodes = new LevelCode[]
    {
        new LevelCode { code = new int[] { 2, 4, 2, 4 } }, // Level 1
        new LevelCode { code = new int[] { 1, 3, 0, 2 } }, // Level 2
        new LevelCode { code = new int[] { 4, 0, 4, 1 } }, // Level 3
    };

    public int LevelCount => levelCodes.Length;

    private const int minDials = 2;
    private const int maxDials = 5;
    private const int maxSpinValue = 4;

    // levelNumber is 1-based. Clamps so an out-of-range level still returns a valid code.
    public int[] GetCodeForLevel(int levelNumber)
    {
        if (levelCodes == null || levelCodes.Length == 0)
            return new int[] { 0, 0 };

        int index = Mathf.Clamp(levelNumber - 1, 0, levelCodes.Length - 1);
        return levelCodes[index].code;
    }

    public int GetLengthForLevel(int levelNumber) => GetCodeForLevel(levelNumber).Length;

    private void OnValidate()
    {
        if (levelCodes == null || levelCodes.Length == 0)
            levelCodes = new LevelCode[] { new LevelCode { code = new int[] { 0, 0 } } };

        for (int l = 0; l < levelCodes.Length; l++)
        {
            var entry = levelCodes[l];
            if (entry == null) { entry = new LevelCode(); levelCodes[l] = entry; }
            if (entry.code == null || entry.code.Length == 0) entry.code = new int[] { 0, 0 };

            if (entry.code.Length < minDials)
            {
                Debug.LogWarning($"{name}: Level {l + 1} code needs at least {minDials} digits. Clamping.");
                System.Array.Resize(ref entry.code, minDials);
            }
            else if (entry.code.Length > maxDials)
            {
                Debug.LogWarning($"{name}: Level {l + 1} code needs at most {maxDials} digits. Clamping.");
                System.Array.Resize(ref entry.code, maxDials);
            }

            for (int i = 0; i < entry.code.Length; i++)
                entry.code[i] = Mathf.Clamp(entry.code[i], 0, maxSpinValue);
        }
    }
}