using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner_call : MonoBehaviour
{
    //public static Runner_call runner;
    public static List<IEnumerator> Coroutines = new List<IEnumerator>();

    private void Awake()
    {
        //runner = this;
        DontDestroyOnLoad(this);
    }

    private void FixedUpdate()
    {
        if (Coroutines.Count > 0)
        {
            StartCoroutine(Coroutines[0]);
            Coroutines.RemoveAt(0);
        }
    }
}
