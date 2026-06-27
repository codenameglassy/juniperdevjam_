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
    // Fires with the 0-based index of the dial the player must click next (-1 when no sequence / solved).
    // Wire a highlight to this so the player knows which dial is active.
    public System.Action<int> OnSequenceActiveDialChanged;

    private int activeLevel = 1;
    private int activeDialCount;

    // Sequence state (0-based dial indices). Empty/null solveOrder => any-order.
    private int[] solveOrder;
    private int sequenceProgress;
    private bool useSequence;

    private void Awake()
    {
        if (codeLockData == null)
            Debug.LogError($"{name}: no CodeLockData assigned.", this);
    }

    private void OnEnable() => SubscribeToDials();
    private void OnDisable() => UnsubscribeFromDials();

    private void Start()
    {
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

        LoadSequence();
    }

    // Reads + validates this level's solve order. Falls back to any-order on bad data.
    private void LoadSequence()
    {
        sequenceProgress = 0;
        useSequence = false;
        solveOrder = null;

        if (codeLockData == null) { RaiseActiveDialChanged(); return; }

        int[] raw = codeLockData.GetSolveOrderForLevel(activeLevel); // 1-based
        if (raw == null || raw.Length == 0) { RaiseActiveDialChanged(); return; }

        if (raw.Length != activeDialCount)
        {
            Debug.LogError($"{name}: Level {activeLevel} solveOrder length ({raw.Length}) must equal " +
                           $"active dial count ({activeDialCount}). Falling back to any-order.", this);
            RaiseActiveDialChanged();
            return;
        }

        int[] zeroBased = new int[raw.Length];
        bool[] seen = new bool[activeDialCount];
        for (int i = 0; i < raw.Length; i++)
        {
            int idx = raw[i] - 1; // 1-based -> 0-based
            if (idx < 0 || idx >= activeDialCount || seen[idx])
            {
                Debug.LogError($"{name}: Level {activeLevel} solveOrder must be a permutation of 1..{activeDialCount}. " +
                               $"Invalid/duplicate entry '{raw[i]}'. Falling back to any-order.", this);
                RaiseActiveDialChanged();
                return;
            }
            seen[idx] = true;
            zeroBased[i] = idx;
        }

        solveOrder = zeroBased;
        useSequence = true;
        RaiseActiveDialChanged();
    }

    private void SubscribeToDials()
    {
        if (dials == null) return;
        foreach (var dial in dials)
            if (dial != null) dial.OnClickedDial += HandleDialClicked;
    }

    private void UnsubscribeFromDials()
    {
        if (dials == null) return;
        foreach (var dial in dials)
            if (dial != null) dial.OnClickedDial -= HandleDialClicked;
    }

    // Fires on every dial click (identity-aware). Drives sequence enforcement + the live indicator.
    private void HandleDialClicked(SpinPuzzleBase dial)
    {
        if (useSequence)
            ProcessSequenceClick(dial);

        UpdateAllCorrectIndicator();
    }

    private void ProcessSequenceClick(SpinPuzzleBase dial)
    {
        int clickedIndex = System.Array.IndexOf(dials, dial);
        if (clickedIndex < 0 || clickedIndex >= activeDialCount)
            return; // not a participating dial

        if (sequenceProgress >= solveOrder.Length)
            return; // sequence already complete

        int expectedIndex = solveOrder[sequenceProgress];

        if (clickedIndex != expectedIndex)
        {
            // Out-of-turn click -> reset the entire puzzle.
            Debug.Log($"Out-of-sequence click on dial {clickedIndex + 1}; expected {expectedIndex + 1}. Resetting.");
            SoundManager.Instance.PlayOneShot("error");
            OnCodeIncorrect?.Invoke();
            ResetAllDials();
            return;
        }

        // Right dial, in turn. Advance only once it has reached its target digit.
        if (dials[clickedIndex].IsCorrect)
        {
            sequenceProgress++;
            RaiseActiveDialChanged();
        }
    }

    // Broadcasts which dial is currently active in the sequence (-1 if none).
    private void RaiseActiveDialChanged()
    {
        int idx = (useSequence && solveOrder != null && sequenceProgress < solveOrder.Length)
                  ? solveOrder[sequenceProgress]
                  : -1;
        OnSequenceActiveDialChanged?.Invoke(idx);
    }

    private void UpdateAllCorrectIndicator()
    {
        if (allCorrectIndicator == null) return;

        bool solved = CheckCode();
        allCorrectIndicator.SetActive(solved);

        if (solved)
            LevelObserver.Instance.NotifyAllCorrectCode();
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

        // With a sequence, the puzzle isn't solved until the sequence is fully completed.
        if (useSequence && (solveOrder == null || sequenceProgress < solveOrder.Length))
            return false;

        for (int i = 0; i < activeDialCount; i++)
        {
            if (dials[i] == null || !dials[i].IsCorrect)
                return false;
        }
        return true;
    }

    private void ResetAllDials()
    {
        sequenceProgress = 0;
        for (int i = 0; i < dials.Length; i++)
            if (dials[i] != null) dials[i].ResetSpin();

        RaiseActiveDialChanged();
    }
}