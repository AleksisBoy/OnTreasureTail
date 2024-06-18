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
    [SerializeField] private float attackRadius = 1.5f;
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

    private int enemyLockedIndex = 0;
    private List<AIEnemy> enemiesAround = new List<AIEnemy>();

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
        if (this.target)
        {
            this.target.Health.DesignFromOnDie(SwitchEnemyLocked);
        }
        this.target = target;
        if (this.target)
        {
            this.target.Health.AssignOnDie(SwitchEnemyLocked);
        }
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
        if (Input.GetKeyDown(KeyCode.U))
        {
            CombatManager.PrintOut();
        }
    }
    private void ToggleCombatLock()
    {
        if (enemiesAround.Count == 0)
        {
            UpdateEnemiesAround();
            if (enemiesAround.Count > 0) StartCombat();
            else EndCombat();
        }
        else
        {
            SwitchEnemyLocked();
        }
    }

    private void StartCombat()
    {
        if (enemiesAround.Count == 0) return;

        animator.SetBool("CombatLocked", true);
        enemyLockedIndex = 0;
        AIEnemy enemyToLockOn = enemiesAround[enemyLockedIndex];
        SetTarget(enemyToLockOn);
        playerCamera.LockCameraOn(enemyToLockOn.transform);
    }
    private void EndCombat()
    {
        enemiesAround.Clear();
        animator?.SetBool("CombatLocked", false);
        SetTarget(null);
        playerCamera?.UnlockCameraFromTarget();
    }
    private void UpdateEnemiesAround()
    {
        enemiesAround = CombatManager.GetEnemiesInRadius(transform.position, combatLockRadius);
    }
    private void UpdateEnemiesAroundWithNew()
    {
        List<AIEnemy> enemiesAroundNew = CombatManager.GetEnemiesInRadius(transform.position, combatLockRadius);
        foreach(AIEnemy enemy in enemiesAroundNew) 
        {
            if (!enemiesAround.Contains(enemy))
            {
                enemiesAround.Add(enemy);
            }
        }
    }
    private void UpdateEnemiesAroundOfDestroyed()
    {
        List<AIEnemy> enemiesToRemove = new List<AIEnemy>();
        foreach(AIEnemy enemy in enemiesAround)
        {
            if(enemy.Health.IsDead())
            {
                enemiesToRemove.Add(enemy);
            }
        }
        foreach(AIEnemy enemy in enemiesToRemove)
        {
            enemiesAround.Remove(enemy);
        }
    }
    private void SwitchEnemyLocked()
    {
        Debug.Log("Switch enemy locked");
        UpdateEnemiesAroundOfDestroyed();
        UpdateEnemiesAroundWithNew();
        if(enemiesAround.Count == 0)
        {
            EndCombat();
            return;
        }
        enemyLockedIndex++;
        if (enemyLockedIndex >= enemiesAround.Count)
        {
            enemyLockedIndex = 0;
        }
        AIEnemy enemyToLockOn = enemiesAround[enemyLockedIndex];
        SetTarget(enemyToLockOn);
        playerCamera.LockCameraOn(enemyToLockOn.transform);
    }
    public void AnimationEvent_AttackImpact()
    {
        List<AIEnemy> enemiesNear = CombatManager.GetEnemiesInRadius(transform.position, attackRadius);
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
        // Combat Lock radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, combatLockRadius);

        // Attack Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
