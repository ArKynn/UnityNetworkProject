using System;
using System.Collections;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(StartTimer(5, DestroyThis));
    }

    private void DestroyThis() => Destroy(gameObject);
    
    private IEnumerator StartTimer(float duration, Action methodToCall)
    {
        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
        
            yield return null;
        }
        methodToCall();
    }
}
