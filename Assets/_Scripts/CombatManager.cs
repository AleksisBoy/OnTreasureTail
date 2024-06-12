using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CombatManager
{
    public static bool Ongoing { get; private set; } = false;
    private static Action CombatStarted;
    private static Action CombatEnded;
    public static HashSet<AIEnemy> InCombat { get; private set; } = new HashSet<AIEnemy>();
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
    public static List<AIEnemy> GetEnemiesInRadius(Vector3 position, float radius)
    {
        Dictionary<AIEnemy, float> enemies = new Dictionary<AIEnemy, float>();

        foreach (AIEnemy enemy in AIEnemy.List)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < radius) enemies.Add(enemy, distance);
        }
        if (enemies.Count > 0)
        {
            enemies = enemies.OrderBy(enemy => enemy.Value).ToDictionary(enemy => enemy.Key, enemy =>  enemy.Value);
        }
        else return new List<AIEnemy>();

        return enemies.Keys.ToList();
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
