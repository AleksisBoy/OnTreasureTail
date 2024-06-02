using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UIManager
{
    public static Stack<TailPanel> Current {  get; private set; } = new Stack<TailPanel>();

    public static bool Open(TailPanel panel)
    {
        if(Current.Contains(panel))
        {
            Debug.LogWarning(panel.name + " already opened");
            return false;
        }
        panel.Open();
        Current.Push(panel);
        return true;
    }
    public static bool Open(TailPanel panel, Action closeEvent)
    {
        if(Open(panel)) panel.AddOnClose(closeEvent);
        return true;
    }
    public static void Close(TailPanel panel)
    {
        List<TailPanel> currentList = Current.ToList();
        Queue<TailPanel> dependantPanels = new Queue<TailPanel>();
        foreach(TailPanel child in panel.children)
        {
            dependantPanels.Enqueue(child);
        }
        while (dependantPanels.Count > 0)
        {
            TailPanel dependantPanel = dependantPanels.Dequeue();

            if(currentList.Contains(dependantPanel)) currentList.Remove(dependantPanel);

            foreach (TailPanel child in dependantPanel.children)
            {
                if (currentList.Contains(child)) dependantPanels.Enqueue(child);
            }
        }
        panel.Close();
        currentList.Remove(panel);
        Current.Clear();
        for(int i = currentList.Count - 1; i > 0; i--)
        {
            Current.Push(currentList[i]);
        }
    }
    public static void CloseLast()
    {
        Close(Current.Pop());
    }
    public static TailPanel LastOpen
    {
        get 
        { 
            if(Current.Count == 0)
            {
                return null;
            }
            return Current.Peek();
        }
    }
    public static bool IsOpen()
    {
        return Current.Count > 0;
    }
    public static bool IsOpen(TailPanel panel)
    {
        return Current.Contains(panel);
    }
    public static bool IsOpenOnly(TailPanel panel)
    {
        if (Current.Count != 1) return false;

        return Current.Peek() == panel;
    }
}
