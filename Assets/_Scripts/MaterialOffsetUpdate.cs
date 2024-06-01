using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOffsetUpdate : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float speed;

    private Material mat;
    private void OnEnable()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
    }
    private void Start()
    {
        mat.mainTextureOffset = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }
    private void Update()
    {
        mat.mainTextureOffset += direction * speed * Time.deltaTime;
    }
}
