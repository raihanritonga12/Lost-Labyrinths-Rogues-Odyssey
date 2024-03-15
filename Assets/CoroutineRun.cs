using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static Coroutine RunCoroutine(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }

    public static void StopCor(Coroutine coroutine)
    {
        instance.StopCoroutine(coroutine);
    }
}
