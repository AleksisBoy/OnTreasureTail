using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UIManager
{
    public static Stack<ITailable> Current {  get; private set; } = new Stack<ITailable>();
    public static bool Open(ITailable panel)
    {
        if (Current.Contains(panel))
        {
            Debug.LogWarning(panel.GetName() + " already opened");
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
        foreach (ITailable openPanel in Current)
        {
            if (openPanel.BlockPlayer)
            {
                blockPlayer = true;
                break;
            }
        }

        PlayerInteraction.Instance.EnablePlayerComponents(!blockPlayer);
    }

    public static bool Open(ITailable panel, Action closeEvent)
    {
        if(Open(panel)) panel.AddOnClose(closeEvent);
        return true;
    }
    public static void Close(ITailable panel)
    {
        List<ITailable> currentList = Current.ToList();
        Queue<ITailable> dependantPanels = new Queue<ITailable>();
        foreach(ITailable child in panel.Children)
        {
            dependantPanels.Enqueue(child);
        }
        while (dependantPanels.Count > 0)
        {
            ITailable dependantPanel = dependantPanels.Dequeue();

            if(currentList.Contains(dependantPanel)) currentList.Remove(dependantPanel);

            foreach (ITailable child in dependantPanel.Children)
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
    public static void Toggle(ITailable panel)
    {
        if (Current.Contains(panel)) Close(panel);
        else Open(panel);
    }
    public static void CloseLast()
    {
        Close(Current.Pop());
    }
    public static ITailable LastOpen
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
    public static bool IsOpen(ITailable panel)
    {
        return Current.Contains(panel);
    }
    public static bool IsOpenOnly(ITailable panel)
    {
        if (Current.Count != 1) return false;

        return Current.Peek() == panel;
    }
    /*
    public static List<int> Blockers { get; private set; } = new List<int>();

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
    */
    public static void Clear()
    {
        Current.Clear();
        //Blockers.Clear();
    }
}
