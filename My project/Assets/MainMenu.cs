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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCreditsPanel(bool bool_)
    {
        creditsPanel.SetActive(bool_);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
