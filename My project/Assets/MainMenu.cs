using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject creditsPanel;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.Play("theme");
    }

    public void SetCreditsPanel(bool bool_)
    {
        creditsPanel.SetActive(bool_);
        SoundManager.Instance.Play("button");
    }

    public void LoadGame()
    {
        SoundManager.Instance.Play("button");

        SceneManager.LoadScene("SampleScene");
    }
}
