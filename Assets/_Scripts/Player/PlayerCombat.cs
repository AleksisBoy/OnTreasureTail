using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : PlayerSubinteraction
{
    [Header("Combat")]
    [SerializeField] private float combatLockRadius = 10f;
    [SerializeField] private float combatSpeed = 3f;
    [SerializeField] private float combatSprintSpeed = 3f;
    [Header("Evasion")]
    [SerializeField] private float evadeSpeed = 3f;
    [SerializeField] private float evadeDistance = 20f;
    [Header("Agility")]
    [SerializeField] private float baseAgility = 100f;
    [SerializeField] private float sprintAgility = 1f;
    [SerializeField] private float restoreAgilityTimer = 1f;
    [SerializeField] private float restoreAgilityModifier = 10f;

    private float agility = 100f;
    private float lastAgilityUse = 0f;

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
    private float Agility
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
    private void Start()
    {
        CombatManager.AssignOnCombatEnded(ResetAgility);
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
        if(Agility < baseAgility && Time.time - lastAgilityUse > restoreAgilityTimer)
        {
            Agility += Time.deltaTime * restoreAgilityModifier;
        }
    }
    private void OnGUI()
    {
        GUI.Box(new Rect(30f, 30f, 100f, 30f), string.Format("{0:0}", Agility), InternalSettings.Get.DebugStyle);
    }
    private void ToggleCameraLockInput()
    {
        //if (!CombatManager.Ongoing) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleCombatLock();
        }
    }
    private void ToggleCombatLock()
    {
        if (target != null)
        {
            EndCombat();
        }
        else
        {
            StartCombat();
        }
    }

    private void StartCombat()
    {
        List<AIEnemy> enemiesAround = CombatManager.GetEnemiesInRadius(transform.position, combatLockRadius);
        if (enemiesAround.Count == 0) return;

        PlayerInteraction.Instance.SetCombatAnimator(true);
        AIEnemy enemy = enemiesAround[0];
        SetTarget(enemy);
        playerCamera.LockCameraOn(enemy.transform);
    }
    private void EndCombat()
    {
        PlayerInteraction.Instance.SetCombatAnimator(false);
        SetTarget(null);
        playerCamera?.UnlockCameraFromTarget();
    }


    public bool InCombat()
    {
        return target != null;
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
