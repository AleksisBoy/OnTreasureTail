using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : PlayerSubinteraction
{
    [Header("Combat")]
    [SerializeField] private Transform attackImpactTransform = null;
    [SerializeField] private float combatLockRadius = 10f;
    [SerializeField] private float combatSpeed = 3f;
    [SerializeField] private float combatSprintSpeed = 3f;
    [Header("Attacks")]
    [SerializeField] private KeyCode attackInput = KeyCode.F;
    [SerializeField] private float attackDamage = 30f;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private int attackAnimations = 2;
    [Header("Evasion")]
    [SerializeField] private float evadeSpeed = 3f;
    [SerializeField] private float evadeDistance = 20f;
    [Header("Agility")]
    [SerializeField] private float baseAgility = 100f;
    [SerializeField] private float sprintAgility = 1f;
    [SerializeField] private float attackAgility = 20f;
    [SerializeField] private float evadeAgility = 20f;
    [SerializeField] private float restoreAgilityTimer = 1f;
    [SerializeField] private float restoreAgilityModifier = 10f;

    private float agility = 100f;
    private float lastAgilityUse = 0f;
    private float lastAttackTime = 0f;

    private PlayerCamera playerCamera = null;
    private AIEnemy target = null;

    public AIEnemy Target => target;
    public float Speed => combatSpeed;
    public float EvadeSpeed => evadeSpeed;
    public float EvadeDistance => evadeDistance;
    public float SprintSpeed
    {
        get
        {
            if (Agility <= 0f) return Speed;

            agility -= Time.deltaTime * sprintAgility;
            lastAgilityUse = Time.time;
            return combatSprintSpeed;
        }
    }
    private float Agility // decrease agility on evade
    {
        get
        {
            return agility;
        }
        set
        {
            agility = Mathf.Clamp(value, 0f, baseAgility);
        }
    }

    public override void Set(Animator animator, params object[] setList)
    {
        base.Set(animator, setList);

        foreach (var item in setList)
        {
            if (item as PlayerCamera)
            {
                playerCamera = (PlayerCamera)item;
            }
            else if(item as PlayerMovement)
            {
                PlayerMovement movement = (PlayerMovement)item;
                movement.SetCombat(this);
            }
        }
        ResetAgility();
    }
    public void SetTarget(AIEnemy target)
    {
        this.target = target;
    }
    private void ResetAgility()
    {
        Agility = baseAgility;
    }
    private void Update()
    {
        ToggleCameraLockInput();
        AttackInput();
        AgilityRestoreProcess();
    }

    private void AgilityRestoreProcess()
    {
        if (Agility < baseAgility && Time.time - lastAgilityUse > restoreAgilityTimer)
        {
            Agility += Time.deltaTime * restoreAgilityModifier;
        }
    }

    private void AttackInput()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (Agility < attackAgility) return;

        if (Input.GetKeyDown(attackInput))
        {
            lastAttackTime = Time.time;
            lastAgilityUse = Time.time;
            Agility -= attackAgility;
            animator.SetInteger("AttackAnim", Random.Range(0, attackAnimations));
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(30f, 30f, 100f, 30f), string.Format("{0:0}", Agility), InternalSettings.Get.DebugStyle);
    }
    private void ToggleCameraLockInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleCombatLock();
        }
    }
    private void ToggleCombatLock()
    {
        if (target == null)
        {
            StartCombat();
        }
        else
        {
            EndCombat();
        }
    }

    private void StartCombat()
    {
        List<AIEnemy> enemiesAround = CombatManager.GetEnemiesInRadius(transform.position, combatLockRadius);
        if (enemiesAround.Count == 0) return;

        animator.SetBool("CombatLocked", true);
        AIEnemy enemy = enemiesAround[0];
        SetTarget(enemy);
        playerCamera.LockCameraOn(enemy.transform);
    }
    private void EndCombat()
    {
        animator?.SetBool("CombatLocked", false);
        SetTarget(null);
        playerCamera?.UnlockCameraFromTarget();
    }
    public void AnimationEvent_AttackImpact()
    {
        List<AIEnemy> enemiesNear = CombatManager.GetEnemiesInRadius(attackImpactTransform.position, 1f);
        if (enemiesNear.Count == 0) return;

        foreach(AIEnemy enemy in enemiesNear)
        {
            enemy.Health.DealDamage(attackDamage, transform.position);
        }
    }
    public bool IsAttacking()
    {
        return animator.GetInteger("AttackAnim") > -1 ? true : false;
    }
    public bool InCombat()
    {
        return target != null;
    }
    public float GetDistanceToTarget()
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }
    public bool TryEvade()
    {
        bool didEvade = agility >= evadeAgility;
        if (didEvade)
        {
            Agility -= evadeAgility;
            lastAgilityUse = Time.time;
        }

        return didEvade;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EndCombat();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, combatLockRadius);
    }
}
