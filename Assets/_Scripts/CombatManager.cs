using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatManager
{
    public static bool Ongoing { get; private set; } = false;
    private static Action CombatStarted;
    private static Action CombatEnded;
    private static List<AIEnemy> InCombat = new List<AIEnemy>();
    public static List<AIEnemy> EnemyList
    {
        get
        {
            return new List<AIEnemy>(InCombat);
        }
    }
    public static void Clear()
    {
        Ongoing = false;
        CombatStarted = null;
        CombatEnded = null;
        InCombat.Clear();
    }
    public static void AddInCombat(AIEnemy enemy)
    {
        if(InCombat.Contains(enemy)) return;
        InCombat.Add(enemy);

        CheckCombatState();
    }
    public static void RemoveFromCombat(AIEnemy enemy)
    {
        if(!InCombat.Contains(enemy)) return;
        InCombat.Remove(enemy);

        CheckCombatState();
    }
    private static void CheckCombatState()
    {
        if(InCombat.Count > 0 && !Ongoing)
        {
            Call_CombatStarted();
            Ongoing = true;
        }
        else if(InCombat.Count == 0 && Ongoing)
        {
            Call_CombatEnded();
            Ongoing = false;
        }
    }
    public static void Call_CombatStarted()
    {
        if (CombatStarted != null) CombatStarted();
    }
    public static void Call_CombatEnded()
    {
        if (CombatEnded != null) CombatEnded();
    }
    public static void AssignOnCombatStarted(Action action)
    {
        CombatStarted += action;
    }
    public static void AssignOnCombatEnded(Action action)
    {
        CombatEnded += action;
    }
}
