using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private UnityEvent onDeathEvent = null;

    private float hp = 0f;
    public float Value => hp;

    private Action<Vector3> onDamage;
    private Action onDie;

    private void Awake()
    {
        hp = maxHealth;
    }
    public void DealDamage(float damage, Vector3 actorPosition)
    {
        Debug.Log( damage +" Damage dealt on " + name);
        hp -= damage;
        Vector3 direction = actorPosition - transform.position;
        Call_OnDamage(direction.normalized);

        if (hp <= 0f)
        {
            Die();
        }
    }
    public void Heal(float perc)
    {
        hp = Mathf.Clamp(hp + maxHealth * perc, 0, maxHealth);
    }
    private void Die()
    {
        float overdamage = Math.Abs(hp);
        hp = 0f;
        Call_OnDie();
        onDeathEvent.Invoke();
    }
    // Getters
    public bool IsDead()
    {
        return hp <= 0f;
    }
    // Action Calls
    public void AssignOnDamage(Action<Vector3> onDamageAction)
    {
        onDamage += onDamageAction;
    }
    public void AssignOnDie(Action onDieAction)
    {
        onDie += onDieAction;
    }
    public void DesignFromOnDamage(Action<Vector3> onDamageAction)
    {
        onDamage -= onDamageAction;
    }
    public void DesignFromOnDie(Action onDieAction)
    {
        onDie -= onDieAction;
    }
    private void Call_OnDamage(Vector3 direction)
    {
        if (onDamage != null) onDamage(direction);
    }
    private void Call_OnDie()
    {
        if (onDie != null) onDie();
    }
}
