using UnityEngine;

public class CodeLockManager : MonoBehaviour
{
    [Header("Dials in left-to-right order matching the code (2 to 5 dials)")]
    [SerializeField] private SpinPuzzleBase[] dials;

    [Header("Code Source")]
    [SerializeField] private CodeLockData codeLockData;

    [Header("Live Feedback")]
    [Tooltip("Enabled when all dials are currently correct, disabled otherwise. Updates live as dials are clicked.")]
    [SerializeField] private GameObject allCorrectIndicator;

    [Header("Events")]
    public System.Action OnCodeCorrect;
    public System.Action OnCodeIncorrect;

    private void Awake()
    {
        ValidateSetup();
        SyncTargetDigitsToDials();
    }

    private void OnEnable()
    {
        SubscribeToDials();
    }

    private void OnDisable()
    {
        UnsubscribeFromDials();
    }

    private void Start()
    {
        UpdateAllCorrectIndicator();
    }

    private void SubscribeToDials()
    {
        if (dials == null) return;
        foreach (var dial in dials)
        {
            if (dial != null)
                dial.OnClicked += HandleDialClicked;
        }
    }

    private void UnsubscribeFromDials()
    {
        if (dials == null) return;
        foreach (var dial in dials)
        {
            if (dial != null)
                dial.OnClicked -= HandleDialClicked;
        }
    }

    // Fires on every dial click - keeps the live indicator in sync
    private void HandleDialClicked(int _)
    {
        UpdateAllCorrectIndicator();
    }

    private void UpdateAllCorrectIndicator()
    {
        if (allCorrectIndicator == null)
            return;

        allCorrectIndicator.SetActive(CheckCode());
    }

    private void ValidateSetup()
    {
        if (codeLockData == null)
        {
            Debug.LogError($"{name}: no CodeLockData assigned.", this);
            return;
        }

        if (dials.Length != codeLockData.Length)
        {
            Debug.LogError(
                $"{name}: dial count ({dials.Length}) doesn't match CodeLockData length ({codeLockData.Length}). " +
                "Fix the dials array or the assigned CodeLockData asset.", this);
        }
    }

    private void SyncTargetDigitsToDials()
    {
        if (codeLockData == null || dials.Length != codeLockData.Length)
            return;

        int[] correctCode = codeLockData.CorrectCode;
        for (int i = 0; i < dials.Length; i++)
        {
            dials[i].SetTargetDigit(correctCode[i]);
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
        if (codeLockData == null || dials.Length != codeLockData.Length)
            return false;

        foreach (var dial in dials)
        {
            if (!dial.IsCorrect)
                return false;
        }

        return true;
    }

    private void ResetAllDials()
    {
        foreach (var dial in dials)
        {
            dial.ResetSpin();
        }
    }
}