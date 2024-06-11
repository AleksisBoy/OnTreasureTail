using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private float hp = 0f;
    public float Value => hp;

    private Action onDamage;
    private Action onDie;

    private void Awake()
    {
        hp = maxHealth;
    }
    public void DealDamage(float damage)
    {
        hp -= damage;
        Call_OnDamage();

        if (hp <= 0f)
        {
            float overdamage = Math.Abs(hp);
            hp = 0f;
            Call_OnDie();
        }
    }
    public bool IsDead()
    {
        return hp <= 0f;
    }
    public void AssignOnDamage(Action onDamageAction)
    {
        onDamage += onDamageAction;
    }
    public void AssignOnDie(Action onDieAction)
    {
        onDie += onDieAction;
    }
    private void Call_OnDamage()
    {
        if (onDamage != null) onDamage();
    }
    private void Call_OnDie()
    {
        if (onDie != null) onDie();
    }
}
