using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerCombat : PlayerSubinteraction
{
    [Header("Combat")]
    [SerializeField] private float combatSpeed = 3f;
    [SerializeField] private float combatSprintSpeed = 3f;
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
        CombatManager.AssignOnCombatEnded(EndCombat);
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
        GUI.Box(new Rect(0f, 0f, 100f, 30f), Agility.ToString());
    }
    private void ToggleCameraLockInput()
    {
        if (!CombatManager.Ongoing) return;

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

    private void EndCombat()
    {
        PlayerInteraction.Instance.SetCombatAnimator(false);
        playerCamera.UnlockCameraFromTarget();
        SetTarget(null);
    }

    private void StartCombat()
    {
        PlayerInteraction.Instance.SetCombatAnimator(true);
        AIEnemy enemy = CombatManager.EnemyList[0];
        SetTarget(enemy);
        playerCamera.LockCameraOn(enemy.transform);
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

    }
}
