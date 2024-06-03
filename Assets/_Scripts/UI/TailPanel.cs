using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class TailPanel : MonoBehaviour
{
    [Header("Tail Panel")]
    [SerializeField] private bool blockPlayer = false;

    public bool BlockPlayer => blockPlayer;
    public List<TailPanel> children = new List<TailPanel>();

    private Action onClose = null;
    private Canvas canvas;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        gameObject.SetActive(false);
    }
    public void Close()
    {
        foreach(TailPanel child in children)
        {
            child.Close();
        }
        if (onClose != null)
        {
            onClose();
            onClose = null;
        }
        gameObject.SetActive(false);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void AddOnClose(Action action)
    {
        onClose += action;
    }
}
