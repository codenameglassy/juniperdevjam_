using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Range")]
    [SerializeField] private int firstLevel = 1;
    [SerializeField] private int lastLevel = 3;

    public int CurrentLevel { get; private set; }
    public int FirstLevel => firstLevel;
    public int LastLevel => lastLevel;

    // Listeners (like CodeLockManager) react to this to reload their code.
    public event Action<int> OnLevelChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;                      // returns BEFORE touching CurrentLevel
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);   // survives the reload
        CurrentLevel = firstLevel;       // runs only on first boot
    }

    public void SetLevel(int level)
    {
        int clamped = Mathf.Clamp(level, firstLevel, lastLevel);
        if (clamped == CurrentLevel) return;

        CurrentLevel = clamped;
        OnLevelChanged?.Invoke(CurrentLevel);
    }

    public void NextLevel() => SetLevel(CurrentLevel + 1);

    public void ResetToFirstLevel() => SetLevel(firstLevel);

    public bool IsFirstLevel()
    {
        return CurrentLevel == firstLevel;
    }

    public bool IsLastLevel => CurrentLevel >= lastLevel;
}