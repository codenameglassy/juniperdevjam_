using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpinSpeedText : MonoBehaviour
{
    public TextMeshPro speedText;
    public SpinPuzzleBase spinPuzzle;

    private void Start()
    {
        spinPuzzle.OnClicked += ChangeSpeedText;
    }

    private void OnDestroy()
    {
        spinPuzzle.OnClicked -= ChangeSpeedText;
    }

    void ChangeSpeedText(int newSpeed)
    {
        speedText.text = newSpeed.ToString();
    }
}