using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> thingsToDisable = new List<GameObject>();

    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;

    [Header("Scene Names")]
    [SerializeField] private string gameOverSceneName = "Gameover";

    [Header("Canvas")]
    [SerializeField] private CanvasGroup fadeCanvas;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        fadeCanvas.alpha = 1.0f;
        fadeCanvas.DOFade(0, 3f);

        GameStateManager.Instance.SetState(GameState.Gameplay);
        SoundManager.Instance.Play("theme");
        OnGameStart?.Invoke();
    }

    public void EndLevel()
    {
        //hide numbers
        for (int i = 0; i < thingsToDisable.Count; i++)
        {
            thingsToDisable[i].SetActive(false);
        }

        //switch game state to gameover
        GameStateManager.Instance.SetState(GameState.Gameover);

        //fadeout theme music
        SoundManager.Instance.FadeOutStop("theme", 0.5f);
        
        OnGameEnd?.Invoke();
    }

    public void LoadNextLevel()
    {
        // Check BEFORE bumping — NextLevel() clamps, so after bumping you can't tell
        // "was on last level" from "just moved to last level".
        if (LevelManager.Instance.IsLastLevel)
        {
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }

        LevelManager.Instance.NextLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
