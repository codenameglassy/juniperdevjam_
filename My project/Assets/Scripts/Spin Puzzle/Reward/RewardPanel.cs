using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPanel : MonoBehaviour
{
    [Header("Reward Source")]
    [SerializeField] private RewardData rewardData;

    [Header("UI References")]
    [Tooltip("The visual panel to show/hide. Should NOT be this script's own GameObject.")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Image rewardPng;
    [SerializeField] private Button nextLevelButton;

    [Header("Code Lock")]
    [SerializeField] private CodeLockManager codeLockManager;

    private void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    private void OnEnable()
    {
        if (codeLockManager != null)
            codeLockManager.OnCodeCorrect += ShowReward;

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelPressed);
    }

    private void OnDisable()
    {
        if (codeLockManager != null)
            codeLockManager.OnCodeCorrect -= ShowReward;

        if (nextLevelButton != null)
            nextLevelButton.onClick.RemoveListener(OnNextLevelPressed);
    }

    // Fires when the puzzle is solved. Shows the reward for the level just cleared.
    private void ShowReward()
    {
        GameManager.Instance.EndLevel();

        int level = LevelManager.Instance != null ? LevelManager.Instance.CurrentLevel : 1;
        RewardData.LevelReward reward = rewardData != null ? rewardData.GetRewardForLevel(level) : null;

        if (reward != null)
        {
            if (rewardText != null)
                rewardText.text = reward.rewardText;

            if (rewardPng != null)
            {
                rewardPng.sprite = reward.rewardSprite;
                rewardPng.enabled = reward.rewardSprite != null; // hide the image slot if no sprite set
            }
        }

        if (panelRoot != null) panelRoot.SetActive(true);
    }

    private void OnNextLevelPressed()
    {
        if (panelRoot != null) panelRoot.SetActive(false);

        // Advances the level → CodeLockManager.HandleLevelChanged reloads the next code & resets dials.
        //if (LevelManager.Instance != null)
        //LevelManager.Instance.NextLevel();

        GameManager.Instance.LoadNextLevel();
    }
}