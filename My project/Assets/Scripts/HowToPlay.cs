using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    [Header("How To Play Sprites")]
    [SerializeField] private Sprite dragCord;
    [SerializeField] private Sprite tapGear;
    [SerializeField] private Sprite crackCode;
    [SerializeField] private Sprite pressButton;

    public Image howToPlayImage;

    void Start()
    {
        if (IsStartingLevel())
        {
            // e.g. show the How-To-Play panel only on level 1
            ShowHowToPlay();
        }

        LevelObserver.Instance.OnBoxTethered += HandleBoxTethered;
        LevelObserver.Instance.OnSpinPuzzleBaseTapped += HandleSpinPuzzleBaseTapped;
        LevelObserver.Instance.OnAllCorrectCode += HandleAllCorrectCode;
        LevelObserver.Instance.OnConfirmButtonPressed += HandleConfirmButtonPressed;
    }

    private void OnDestroy()
    {
        LevelObserver.Instance.OnBoxTethered -= HandleBoxTethered;
        LevelObserver.Instance.OnSpinPuzzleBaseTapped -= HandleSpinPuzzleBaseTapped;
        LevelObserver.Instance.OnAllCorrectCode -= HandleAllCorrectCode;
        LevelObserver.Instance.OnConfirmButtonPressed -= HandleConfirmButtonPressed;

    }

    /// <summary>
    /// Returns true if the player is on the very first level.
    /// </summary>
    public bool IsStartingLevel()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("[HowToPlay] LevelManager.Instance is null.");
            return false;
        }

        return LevelManager.Instance.IsFirstLevel();
    }

    private void ShowHowToPlay()
    {
        // your panel-showing logic here
        Debug.Log("First Level, Showing How to play.");
        howToPlayImage.gameObject.SetActive(true);

        howToPlayImage.sprite = dragCord;
    }

    public void HandleBoxTethered()
    {
        if (!IsStartingLevel())
        {
            return;
        }

        howToPlayImage.sprite = tapGear;

    }

    public void HandleSpinPuzzleBaseTapped()
    {
        if (!IsStartingLevel())
        {
            return;
        }

        howToPlayImage.sprite = crackCode;
    }

    void HandleAllCorrectCode()
    {
        if (!IsStartingLevel())
        {
            return;
        }

        howToPlayImage.sprite = pressButton;
    }

    void HandleConfirmButtonPressed()
    {
        if (!IsStartingLevel())
        {
            return;
        }
        howToPlayImage.gameObject.SetActive(false);
    }
}