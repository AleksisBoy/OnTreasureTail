using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UIManager
{
    public static Stack<TailPanel> Current {  get; private set; } = new Stack<TailPanel>();
    public static List<int> Blockers { get; private set; } = new List<int>();
    public static bool Open(TailPanel panel)
    {
        if (Current.Contains(panel))
        {
            Debug.LogWarning(panel.name + " already opened");
            return false;
        }
        panel.Open();
        Current.Push(panel);
        UpdatePlayerBlocking();
        return true;
    }

    private static void UpdatePlayerBlocking()
    {
        bool blockPlayer = false;
        foreach (TailPanel openPanel in Current)
        {
            if (openPanel.BlockPlayer)
            {
                blockPlayer = true;
                break;
            }
        }
        PlayerInteraction.Instance.EnablePlayerComponents(!blockPlayer);
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
        for(int i = currentList.Count - 1; i >= 0; i--)
        {
            Current.Push(currentList[i]);
        }
        UpdatePlayerBlocking();
    }
    public static void Toggle(TailPanel panel)
    {
        if (Current.Contains(panel)) Close(panel);
        else Open(panel);
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
    public static void AddBlocker(GameObject blocker)
    {
        int instanceID = blocker.GetInstanceID();
        if (Blockers.Contains(instanceID)) return;

        Blockers.Add(instanceID);
    }
    public static void RemoveBlocker(GameObject blocker)
    {
        int instanceID = blocker.GetInstanceID();
        if (!Blockers.Contains(instanceID)) return;

        Blockers.Remove(instanceID);
    }
    public static bool Blocking
    {
        get
        {
            return Blockers.Count > 0;
        }
    }
    public static void Clear()
    {
        Current.Clear();
        Blockers.Clear();
    }
}
