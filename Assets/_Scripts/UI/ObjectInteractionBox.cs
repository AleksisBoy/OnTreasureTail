using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectInteractionBox : MonoBehaviour, ITailable
{
    [SerializeField] private TMP_Text interactionText = null;
    [SerializeField] private Vector2 widthRange = new Vector2(100f, 600f);
    [SerializeField] private Vector2 heightRange = new Vector2(60f, 200f);
    [SerializeField] private Vector2 setPosOffset = new Vector2(10f, 10f);

    private static bool IsOpen = false;
    public static string Text = string.Empty;
    private float setTime = 0f;
    private RectTransform RT => (RectTransform)transform;

    public bool BlockPlayer 
    {
        get => true;
        set => throw new NotImplementedException(); 
    }
    public List<ITailable> Children 
    {
        get => new List<ITailable>();
        set => throw new NotImplementedException(); 
    }

    private void Start()
    {
        Close();
    }
    // make text appearing fast animation
    private void LateUpdate()
    {
        if (IsOpen && Time.time - setTime > 0.1f && Input.GetMouseButtonDown(0))
        {
            UIManager.Close(this);
        }
    }
    private void SetBox(string text, Vector2 mousePos)
    {
        if (IsOpen || text == string.Empty) return;

        gameObject.SetActive(true);
        IsOpen = true;
        setTime = Time.time;

        RT.position = mousePos;

        interactionText.text = text;
        RT.sizeDelta = new Vector2(Mathf.Clamp(interactionText.preferredWidth, widthRange.x, widthRange.y), RT.sizeDelta.y);
        RT.sizeDelta = new Vector2(RT.sizeDelta.x, Mathf.Clamp(interactionText.preferredHeight, heightRange.x, heightRange.y));

        Vector2 offset = Vector2.zero;
        if (mousePos.x > Screen.width / 2f) // mouse to the right side
        {
            offset.x = -RT.sizeDelta.x / 2f - setPosOffset.x;
        }
        else // mouse to the left side
        {
            offset.x = RT.sizeDelta.x / 2f + setPosOffset.x;
        }
        RT.position += (Vector3)offset;
    }

    public void Open()
    {
        SetBox(Text, Input.mousePosition);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        IsOpen = false;
    }

    public void AddOnClose(Action action)
    {
        throw new NotImplementedException();
    }

    public string GetName()
    {
        return name;
    }
}
