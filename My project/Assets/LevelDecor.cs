using System.Collections.Generic;
using UnityEngine;

public class LevelDecor : MonoBehaviour
{
    public List<GameObject> level1_decorToDisable = new List<GameObject>();
    public List<GameObject> level2_decorToDisable = new List<GameObject>();
    public List<GameObject> level3_decorToDisable = new List<GameObject>();
    public List<GameObject> level4_decorToDisable = new List<GameObject>();

    void Start()
    {
        ShowLevelSpecificDecor();
    }

    void ShowLevelSpecificDecor()
    {
        switch (LevelManager.Instance.CurrentLevel)
        {
            case 1: DisableAll(level1_decorToDisable); break;
            case 2: DisableAll(level2_decorToDisable); break;
            case 3: DisableAll(level3_decorToDisable); break;
            case 4: DisableAll(level4_decorToDisable); break;
        }
    }

    void DisableAll(List<GameObject> decor)
    {
        foreach (GameObject go in decor)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
}