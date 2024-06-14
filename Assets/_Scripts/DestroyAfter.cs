using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] private float timer = 10f;

    private float time = 0f;
    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if(time > timer)
        {
            Destroy(gameObject);
        }
    }
}
