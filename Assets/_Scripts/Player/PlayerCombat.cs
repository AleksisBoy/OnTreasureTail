using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float combatSpeed = 3f;

    private PlayerCamera playerCamera = null;
    private AIEnemy target = null;

    public AIEnemy Target => target;
    public float Speed => combatSpeed;

    public void Set(PlayerCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }
    private void Start()
    {
        CombatManager.AssignOnCombatEnded(EndCombat);
    }
    public void SetTarget(AIEnemy target)
    {
        this.target = target;
    }
    private void Update()
    {
        ToggleCameraLockInput();
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
}
