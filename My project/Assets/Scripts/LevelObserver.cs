using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LevelObserver : MonoBehaviour
{
    public static LevelObserver Instance { get; private set; }

    public event Action OnBoxTethered;
    public event Action OnBoxUnTethered;

    public event Action OnSpinPuzzleBaseTapped;
    public event Action OnAllCorrectCode;
    public event Action OnConfirmButtonPressed;

    public event Action OnGameFinished;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NotifyOnBoxTethered()
    {
        OnBoxTethered?.Invoke();
        Debug.Log("Notify All OnBoxTethered Subscribers");
    }

    public void NotifyOnBoxUnTethered()
    {
        OnBoxUnTethered?.Invoke();
        Debug.Log("Notify All OnBoxUnTethered Subscribers");
    }


    public void NotifySpinPuzzleBaseTapped()
    {
        OnSpinPuzzleBaseTapped?.Invoke();
    }

    public void NotifyAllCorrectCode()
    {
        OnAllCorrectCode?.Invoke();
    }

    public void NotifyConfirmButtonPressed()
    {
        OnConfirmButtonPressed?.Invoke();
    }

    public void NotifyGameOver()
    {
        Debug.Log("Notification gameover");
        OnGameFinished?.Invoke();
    }
}
