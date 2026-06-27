using UnityEngine;

[CreateAssetMenu(fileName = "NewCodeLockData", menuName = "PuzzleLock/Code Lock Data")]
public class CodeLockData : ScriptableObject
{
    [System.Serializable]
    public class LevelCode
    {
        [Tooltip("One entry per dial, left to right. Length 2-5, digits 0-4.")]
        public int[] code = new int[] { 2, 4, 2, 4 };

        [Tooltip("Order dials must be solved, as 1-based dial positions (1 = leftmost). " +
                 "Example: {1,3,4,2} = first, third, fourth, second. " +
                 "Leave EMPTY for any-order. Must be a permutation of 1..code.Length.")]
        public int[] solveOrder = new int[0];
    }

    [Tooltip("One code per level. Element 0 = Level 1, Element 1 = Level 2, etc.")]
    [SerializeField]
    private LevelCode[] levelCodes = new LevelCode[]
    {
        new LevelCode { code = new int[] { 2, 4, 2, 4 }, solveOrder = new int[] { 1, 2, 3, 4 } }, // Level 1: left-to-right (gentle)
        new LevelCode { code = new int[] { 1, 3, 0, 2 }, solveOrder = new int[] { 1, 3, 4, 2 } }, // Level 2: first, third, fourth, second
        new LevelCode { code = new int[] { 4, 0, 4, 1 }, solveOrder = new int[] { 2, 4, 1, 3 } }, // Level 3
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

    // Returns the 1-based solve order for the level, or an empty array if any-order.
    public int[] GetSolveOrderForLevel(int levelNumber)
    {
        if (levelCodes == null || levelCodes.Length == 0)
            return new int[0];
        int index = Mathf.Clamp(levelNumber - 1, 0, levelCodes.Length - 1);
        return levelCodes[index].solveOrder ?? new int[0];
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

            ValidateSolveOrder(entry, l + 1);
        }
    }

    // Warns (doesn't auto-fix) if a solveOrder is set but isn't a valid permutation.
    private void ValidateSolveOrder(LevelCode entry, int levelNumber)
    {
        if (entry.solveOrder == null || entry.solveOrder.Length == 0)
            return; // empty = any-order, valid

        int n = entry.code.Length;
        if (entry.solveOrder.Length != n)
        {
            Debug.LogWarning($"{name}: Level {levelNumber} solveOrder length ({entry.solveOrder.Length}) " +
                             $"should equal code length ({n}). It will be ignored at runtime until fixed.");
            return;
        }

        bool[] seen = new bool[n];
        foreach (int v in entry.solveOrder)
        {
            int idx = v - 1;
            if (idx < 0 || idx >= n || seen[idx])
            {
                Debug.LogWarning($"{name}: Level {levelNumber} solveOrder must be a permutation of 1..{n} " +
                                 $"(each position once). It will be ignored at runtime until fixed.");
                return;
            }
            seen[idx] = true;
        }
    }
}