using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDecor : MonoBehaviour
{
    public List<GameObject> level1_decorToDisable = new List<GameObject>();
    public List<GameObject> level2_decorToDisable = new List<GameObject>();
    public List<GameObject> level3_decorToDisable = new List<GameObject>();
    public List<GameObject> level4_decorToDisable = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ShowLevelSpecificDecor();
    }

    void ShowLevelSpecificDecor()
    {
        switch (LevelManager.Instance.CurrentLevel)
        {
            case 1:
                for (int i = 0; i < level1_decorToDisable.Count;)
                {
                    level1_decorToDisable[i].SetActive(false);
                }
                break;

            case 2:
                for (int i = 0; i < level2_decorToDisable.Count;)
                {
                    level2_decorToDisable[i].SetActive(false);
                }
                break;

            case 3:
                for (int i = 0; i < level3_decorToDisable.Count;)
                {
                    level3_decorToDisable[i].SetActive(false);
                }
                break;

            case 4:
                for (int i = 0; i < level4_decorToDisable.Count;)
                {
                    level4_decorToDisable[i].SetActive(false);
                }
                break;
        }
    }
}
