using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetheredBoxParent : MonoBehaviour
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
        bool isactive = newGameState == GameState.Gameplay;
        gameObject.SetActive(isactive);
    }


}
