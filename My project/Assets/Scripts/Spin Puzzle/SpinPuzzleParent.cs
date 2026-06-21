using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinPuzzleParent : MonoBehaviour
{
    public List<PopupPositionEffect2D> popupPositionEffect2Ds = new List<PopupPositionEffect2D>();
    [SerializeField] private float _delay;
    public void PlaySpinPuzzlePopIns()
    {
        StartCoroutine(RoutinSpinPuzzlePopUp());
    }

    public IEnumerator RoutinSpinPuzzlePopUp()
    {
        yield return new WaitForSeconds(_delay);

        for (int i = 0; i < popupPositionEffect2Ds.Count; i++)
        {
            popupPositionEffect2Ds[i].PlayIn();
        }
    }
}
