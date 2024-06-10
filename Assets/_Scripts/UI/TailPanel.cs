using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class TailPanel : MonoBehaviour, ITailable
{
    [Header("Tail Panel")]
    [SerializeField] private bool blockPlayer = false;

    public bool BlockPlayer 
    {
        get => blockPlayer;
        set => blockPlayer = value;
    }
    public List<ITailable> Children 
    {
        get => children;
        set
        {
            Debug.LogError("Set the children list, not this one");
        }
    }

    public List<ITailable> children = new List<ITailable>();

    private Action onClose = null;
    private Canvas canvas;
    protected virtual void Awake()
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
    // Static
    public static void CloseUI(TailPanel panel)
    {
        UIManager.Close(panel);
    }
    public static void OpenUI(TailPanel panel)
    {
        UIManager.Open(panel);
    }

    public string GetName()
    {
        return name;
    }
}
