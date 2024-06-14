using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPoolAfter : MonoBehaviour
{
    [SerializeField] private float updateTime = 10f;
    [SerializeField] private float playerDistance = -1f;

    private IEnumerator ReturnProcess()
    {
        while(true)
        {
            yield return new WaitForSeconds(updateTime);
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerInteraction.Instance.transform.position);
            if (distanceToPlayer > playerDistance)
            {
                ObjectPoolingManager.ReturnObject(gameObject);
                yield break;
            }
        }
    }
    private void OnEnable()
    {
        StartCoroutine(ReturnProcess());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
