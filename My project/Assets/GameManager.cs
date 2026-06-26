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
        LevelManager.Instance.NextLevel();   // bump the persisted level FIRST
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // then reload
    }


}
