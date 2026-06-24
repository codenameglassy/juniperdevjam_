using UnityEngine;

public class CodeLockManager : MonoBehaviour
{
    [Header("Dials in left-to-right order matching the code (2 to 5 dials)")]
    [SerializeField] private SpinPuzzleBase[] dials;

    [Header("Code Source")]
    [SerializeField] private CodeLockData codeLockData;

    [Header("Live Feedback")]
    [Tooltip("Enabled when all active dials are currently correct. Updates live as dials are clicked.")]
    [SerializeField] private GameObject allCorrectIndicator;

    [Header("Events")]
    public System.Action OnCodeCorrect;
    public System.Action OnCodeIncorrect;

    private int activeLevel = 1;
    private int activeDialCount;

    private void Awake()
    {
        if (codeLockData == null)
            Debug.LogError($"{name}: no CodeLockData assigned.", this);
    }

    private void OnEnable() => SubscribeToDials();
    private void OnDisable() => UnsubscribeFromDials();

    private void Start()
    {
        // All Awakes have run by now, so LevelManager.Instance is set.
        activeLevel = LevelManager.Instance != null ? LevelManager.Instance.CurrentLevel : 1;

        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelChanged += HandleLevelChanged;

        ValidateSetup();
        LoadLevelCode();
        UpdateAllCorrectIndicator();
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelChanged -= HandleLevelChanged;
    }

    private void HandleLevelChanged(int newLevel)
    {
        activeLevel = newLevel;
        LoadLevelCode();
        ResetAllDials();
        UpdateAllCorrectIndicator();
    }

    // Pushes the current level's code into the dials and enables only as many as the code needs.
    private void LoadLevelCode()
    {
        if (codeLockData == null) return;

        int[] code = codeLockData.GetCodeForLevel(activeLevel);
        activeDialCount = code.Length;

        if (activeDialCount > dials.Length)
        {
            Debug.LogError(
                $"{name}: Level {activeLevel} code length ({activeDialCount}) exceeds available dials ({dials.Length}).", this);
            activeDialCount = dials.Length;
        }

        for (int i = 0; i < dials.Length; i++)
        {
            if (dials[i] == null) continue;

            bool participates = i < activeDialCount;
            dials[i].SetParticipating(participates);

            if (participates)
                dials[i].SetTargetDigit(code[i]);
        }
    }

    private void SubscribeToDials()
    {
        if (dials == null) return;
        foreach (var dial in dials)
            if (dial != null) dial.OnClicked += HandleDialClicked;
    }

    private void UnsubscribeFromDials()
    {
        if (dials == null) return;
        foreach (var dial in dials)
            if (dial != null) dial.OnClicked -= HandleDialClicked;
    }

    // Fires on every dial click - keeps the live indicator in sync
    private void HandleDialClicked(int _) => UpdateAllCorrectIndicator();

    private void UpdateAllCorrectIndicator()
    {
        if (allCorrectIndicator == null) return;
        allCorrectIndicator.SetActive(CheckCode());
    }

    private void ValidateSetup()
    {
        if (codeLockData == null)
        {
            Debug.LogError($"{name}: no CodeLockData assigned.", this);
            return;
        }

        int needed = codeLockData.GetCodeForLevel(activeLevel).Length;
        if (needed > dials.Length)
        {
            Debug.LogError(
                $"{name}: Level {activeLevel} needs {needed} dials but only {dials.Length} are assigned. " +
                "Add more dials or shorten the level code.", this);
        }
    }

    // Hook this up to your Confirm UI Button's OnClick
    public void OnConfirmPressed()
    {
        if (CheckCode())
        {
            Debug.Log("Code correct!");
            //puzzle solve sfx
            SoundManager.Instance.Play("solved");
            OnCodeCorrect?.Invoke();

            // To advance on solve, uncomment:
            // LevelManager.Instance.NextLevel();
        }
        else
        {
            Debug.Log("Code incorrect, resetting dials.");
            OnCodeIncorrect?.Invoke();
            ResetAllDials();
        }
    }

    private bool CheckCode()
    {
        if (codeLockData == null) return false;

        // Only the active dials (first activeDialCount) count toward the solution.
        for (int i = 0; i < activeDialCount; i++)
        {
            if (dials[i] == null || !dials[i].IsCorrect)
                return false;
        }
        return true;
    }

    private void ResetAllDials()
    {
        for (int i = 0; i < dials.Length; i++)
            if (dials[i] != null) dials[i].ResetSpin();
    }
}