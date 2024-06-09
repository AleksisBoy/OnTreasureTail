using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPopup : MonoBehaviour
{
    [SerializeField] private AnimationCurve translationCurve = null;
    [SerializeField] private float speed = 2f;
    [SerializeField] private TMP_Text popupText = null;

    private RectTransform target;
    public RectTransform RT { get { return (RectTransform)transform; } }

    private Vector2 startPos;
    private float time = 0f;
    private float step = 0f;
    public void SetPopup(string text, RectTransform target)
    {
        this.target = target;
        popupText.text = text;
        startPos = RT.position;
        time = 0f;
        step = 0f;
    }
    private void Update()
    {
        if (!target) return;

        RT.position = Vector2.Lerp(startPos, target.position, step);
        time += Time.deltaTime * speed;
        step = translationCurve.Evaluate(time);
        if(time > 1f)
        {
            Destroy(gameObject);
        }
    }
}
