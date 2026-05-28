using System.Collections;
using UnityEngine;

public class TempCoroutineRunner : MonoBehaviour 
{
    public void StartStaticCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(RunAndDestroy(coroutine));
    }

    private IEnumerator RunAndDestroy(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
        Destroy(this); 
    }
}