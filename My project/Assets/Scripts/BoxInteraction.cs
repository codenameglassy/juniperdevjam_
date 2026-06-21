using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxInteraction : MonoBehaviour
{
    [SerializeField] private GameObject _tetheredBox;
    [SerializeField] private GameObject _untetheredBox;

    [Header("Optional Events")]
    [Tooltip("For PopUp Effects etc.")]
    public UnityEvent OptionalTetheredEvent;

    // Start is called before the first frame update
    void Start()
    {
        LevelObserver.Instance.OnBoxTethered += TetherBox;
        LevelObserver.Instance.OnBoxUnTethered += UnthetherBox;
    }

    private void OnDestroy()
    {
        LevelObserver.Instance.OnBoxTethered -= TetherBox;
        LevelObserver.Instance.OnBoxUnTethered -= UnthetherBox;

    }

    // Update is called once per frame
    void Update()
    {
      
    }


    void TetherBox()
    {
        _tetheredBox.SetActive(true);
        _untetheredBox.SetActive(false);
        OptionalTetheredEvent?.Invoke();    
    }

    void UnthetherBox()
    {
        _tetheredBox.SetActive(false);
        _untetheredBox.SetActive(true);
    }
}
