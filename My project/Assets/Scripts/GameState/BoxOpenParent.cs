using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOpenParent : MonoBehaviour
{
    private void Start()
    {
        
    }
    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;

    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        Debug.Log(newGameState.ToString());
        bool isactive = newGameState == GameState.Gameover;
        gameObject.SetActive(isactive);
    }
}
