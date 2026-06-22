using System.Collections.Generic;
using UnityEngine;

public class CodeLockManager : MonoBehaviour
{
    [Header("Dials in left-to-right order matching the code")]
    [SerializeField] private SpinPuzzleBase[] dials = new SpinPuzzleBase[4];

    [Header("Code Settings")]
    [SerializeField] private int[] correctCode = new int[] { 2, 4, 2, 4 };

    [Header("Events")]
    public System.Action OnCodeCorrect;
    public System.Action OnCodeIncorrect;

    // Hook this up to your Confirm UI Button's OnClick
    public void OnConfirmPressed()
    {
        if (CheckCode())
        {
            Debug.Log("Code correct!");
            OnCodeCorrect?.Invoke();
        }
        else
        {
            Debug.Log("Code incorrect, resetting dials.");
            OnCodeIncorrect?.Invoke();
            ResetAllDials();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnConfirmPressed();
        }
    }

    private bool CheckCode()
    {
        if (dials.Length != correctCode.Length)
        {
            Debug.LogError("CodeLockManager: dials and correctCode length mismatch.");
            return false;
        }

        for (int i = 0; i < dials.Length; i++)
        {
            if (dials[i].CurrentSpinInt != correctCode[i])
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