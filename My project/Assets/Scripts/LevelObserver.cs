using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LevelObserver : MonoBehaviour
{
    public static LevelObserver Instance { get; private set; }

    public event Action OnBoxTethered;
    public event Action OnBoxUnTethered;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            NotifyOnBoxTethered();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            NotifyOnBoxUnTethered();
        }
    }

    public void NotifyOnBoxTethered()
    {
        OnBoxTethered?.Invoke();
        Debug.Log("Notify All OnBoxTethered Subscribers");
    }

    public void NotifyOnBoxUnTethered()
    {
        OnBoxUnTethered?.Invoke();
        Debug.Log("Notify All OnBoxUnTethered Subscribers");
    }
}
