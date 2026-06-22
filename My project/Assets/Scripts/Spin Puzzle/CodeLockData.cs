using UnityEngine;

[CreateAssetMenu(fileName = "NewCodeLockData", menuName = "PuzzleLock/Code Lock Data")]
public class CodeLockData : ScriptableObject
{
    [Tooltip("Length must be between 2 and 5 - one entry per dial, left to right.")]
    [SerializeField] private int[] correctCode = new int[] { 2, 4, 2, 4 };

    public int[] CorrectCode => correctCode;
    public int Length => correctCode.Length;

    private const int minDials = 2;
    private const int maxDials = 5;
    private const int maxSpinValue = 4; // matches SpinPuzzleBase wraparound max

    // Validates in the editor so bad data gets caught immediately, not at runtime
    private void OnValidate()
    {
        if (correctCode == null || correctCode.Length == 0)
        {
            correctCode = new int[] { 0, 0 };
        }

        if (correctCode.Length < minDials)
        {
            Debug.LogWarning($"{name}: code must have at least {minDials} digits. Clamping.");
            System.Array.Resize(ref correctCode, minDials);
        }
        else if (correctCode.Length > maxDials)
        {
            Debug.LogWarning($"{name}: code must have at most {maxDials} digits. Clamping.");
            System.Array.Resize(ref correctCode, maxDials);
        }

        for (int i = 0; i < correctCode.Length; i++)
        {
            correctCode[i] = Mathf.Clamp(correctCode[i], 0, maxSpinValue);
        }
    }
}