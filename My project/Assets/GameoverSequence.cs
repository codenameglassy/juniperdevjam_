using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameoverSequence : MonoBehaviour
{
    public GameObject tentacleHand;
    public GameObject octoNerd;
    private void Start()
    {
        SoundManager.Instance.Play("theme");
         StartCoroutine(GameoverRoutine());
    }
    IEnumerator GameoverRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        tentacleHand.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        LevelObserver.Instance.NotifyGameOver();
        SoundManager.Instance.Play("throwobject");
        tentacleHand.SetActive(false);
        yield return new WaitForSeconds(.7f);
        octoNerd.transform.DOMove(new Vector3(0, 1.5f, 0), 3f);

    }
}
