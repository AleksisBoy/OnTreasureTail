using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class AIEnemy : AIAgent
{
    [Header("AI Enemy")]
    [SerializeField] private AIIdleTarget[] idleTargets = null;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float alertTimer = 15f;
    [SerializeField] private float investigatingSpeed = 2f;
    [SerializeField] private float evadeSpeed = 4f;
    [SerializeField] private float evadeDuration = 0.3f;
    [SerializeField] private float evadeCooldown = 3f;

    private float timeOfLastAttack = 0f;
    private bool alerted = false;
    private float alertTime = 0f;
    private Transform lastPlayerSeenTransform = null;

    private bool evading = false;
    private float evade = 0f;
    private float timeOfLastEvade = 0f;

    private Health health;
    public Health Health => health;

    private AIIdleTarget currentIdleTarget = null;
    private SeePlayerState seePlayer = SeePlayerState.DidNotCheck;

    public static HashSet<AIEnemy> List { get; private set; } = new HashSet<AIEnemy>();

    private enum SeePlayerState
    {
        DoNotSeePlayer,
        SeePlayer,
        DidNotCheck
    }
    private void EnemySetup()
    {
        if (!List.Contains(this)) List.Add(this);

        health = GetComponent<Health>();
        health.AssignOnDie(OnDeath);
        health.AssignOnDamage(OnDamage);

        lastPlayerSeenTransform = new GameObject().transform;
        lastPlayerSeenTransform.name = "Last Player Seen for " + name;
    }
    protected override void Awake()
    {
        EnemySetup();
        base.Awake();

        // Checkers
        Leaf isAlert = new Leaf("Is Alert", IsAlert);
        Inverter isNotAlerted = new Inverter("Is Not Alerted");
        isNotAlerted.AddChild(isAlert);

        Leaf canSeePlayer = new Leaf("Can See player", CanSeePlayer);
        Inverter cannotSeePlayer = new Inverter("Cannot see player");
        cannotSeePlayer.AddChild(canSeePlayer);

        Leaf isAttacking = new Leaf("Is Attacking", IsAttacking);
        Inverter isNotAttacking = new Inverter("Is Not attacking");
        isNotAttacking.AddChild(isAttacking);

        Leaf isCloseToTarget = new Leaf("Is Close To Target", IsCloseToTarget);

        Leaf isPlayerTarget = new Leaf("Is player target", IsPlayerTarget);

        // Be Idle DepSequence
        BehaviourTree beIdleCondition = new BehaviourTree("Dependancy for be idle");
        Sequence beIdleConditions = new Sequence("dependancyBeIdleSequence");
        beIdleConditions.AddChild(cannotSeePlayer);
        beIdleConditions.AddChild(isNotAlerted);
        beIdleCondition.AddChild(beIdleConditions);

        DepSequence beIdle = new DepSequence("Idle Behaviour", beIdleCondition, agent);

        Leaf doIdleAnimation = new Leaf("Idle Animation", IdleAnimation);

        Selector findIdleTarget = new Selector("Find Idle Target");
        Leaf hasIdleTarget = new Leaf("Has Idle Target", IsIdleTarget);
        Leaf setRandomFreeIdleTarget = new Leaf("Set Random Free Idle Target", SetRandomFreeIdleTarget);

        findIdleTarget.AddChild(hasIdleTarget);
        findIdleTarget.AddChild(setRandomFreeIdleTarget);

        beIdle.AddChild(findIdleTarget);
        beIdle.AddChild(isCloseToTarget);
        beIdle.AddChild(doIdleAnimation);

        // Chase Player DepSequence
        Leaf setPlayerTarget = new Leaf("Set Player target", SetPlayerTarget);
        Leaf attackPlayer = new Leaf("Attack Player", AttackPlayer);

        BehaviourTree dependancyChasePlayer = new BehaviourTree("Dependancy chase player");
        Sequence dependancyChasePlayerSequence = new Sequence("dependancyChasePlayerSequence");
        dependancyChasePlayerSequence.AddChild(isNotAttacking);
        dependancyChasePlayerSequence.AddChild(canSeePlayer);
        dependancyChasePlayer.AddChild(dependancyChasePlayerSequence);

        DepSequence chasePlayer = new DepSequence("Chase player", dependancyChasePlayer, agent);

        Selector findPlayerTarget = new Selector("Find Player target");
        findPlayerTarget.AddChild(isPlayerTarget);
        findPlayerTarget.AddChild(setPlayerTarget);

        chasePlayer.AddChild(findPlayerTarget);
        chasePlayer.AddChild(isCloseToTarget);
        chasePlayer.AddChild(attackPlayer);

        // Go to last seen player position
        Leaf isLastSeenTarget = new Leaf("Is Last seen target", IsLastSeenPlayerTarget);
        Leaf setLastSeenTarget = new Leaf("Set last seen target", SetLastSeenPlayerTarget);
        Leaf offsetLastSeenTarget = new Leaf("Offset last seen target", OffsetLastSeenPlayerTarget);

        BehaviourTree checkLastSeenPositionCondition = new BehaviourTree("Conditions");
        Sequence checkLastSeenPositionConditions = new Sequence("checkLastSeenPositionConditions");
        checkLastSeenPositionConditions.AddChild(cannotSeePlayer);
        checkLastSeenPositionConditions.AddChild(isAlert);
        checkLastSeenPositionCondition.AddChild(checkLastSeenPositionConditions);

        DepSequence checkLastSeenPosition = new DepSequence("Check last seen player pos", checkLastSeenPositionCondition, agent);
        
        Selector findLastSeenTransform = new Selector("Find last seen transform");
        findLastSeenTransform.AddChild(isLastSeenTarget);
        findLastSeenTransform.AddChild(setLastSeenTarget);

        checkLastSeenPosition.AddChild(findLastSeenTransform);
        checkLastSeenPosition.AddChild(isCloseToTarget);
        checkLastSeenPosition.AddChild(offsetLastSeenTarget);
        //
        // if is close to target -> look around? go around?

        // Reset
        Leaf resetTarget = new Leaf("Reset target", ResetTarget);

        // Evade attack
        Leaf isPlayerAttacking = new Leaf("Is Player attacking", IsPlayerAttacking);
        Leaf isPlayerClose = new Leaf("Is player Close", IsPlayerClose);
        Leaf evadeBack = new Leaf("Evade back", EvadeBack);

        BehaviourTree evadeAttackCondition = new BehaviourTree();
        Sequence evadeAttackConditions = new Sequence("Evade conditions");
        evadeAttackConditions.AddChild(isPlayerAttacking);
        evadeAttackConditions.AddChild(isPlayerClose);
        evadeAttackCondition.AddChild(evadeAttackConditions);

        DepSequence evadeAttack = new DepSequence("Evade Attack", evadeAttackCondition, agent);

        evadeAttack.AddChild(evadeBack);

        // Enemy Behaviour
        Selector behaveEnemy = new Selector("Behave Like Enemy");
        behaveEnemy.AddChild(evadeAttack);
        behaveEnemy.AddChild(chasePlayer);
        behaveEnemy.AddChild(checkLastSeenPosition);
        behaveEnemy.AddChild(beIdle);
        //behaveEnemy.AddChild(resetTarget);

        tree.AddChild(behaveEnemy);

        tree.PrintTree();
    }
    protected override void Update()
    {
        if (EvadeProcess()) return;
        base.Update();
    }

    private bool EvadeProcess()
    {
        if (!evading) return false;

        transform.position += -transform.forward * evadeSpeed * Time.deltaTime;
        evade += Time.deltaTime / evadeDuration;
        if (evade > 1f)
        {
            evading = false;
            targetState = TargetState.None;
            Speed = walkSpeed; 
        }
        return true;
    }

    private void Alert()
    {
        alerted = true;
        alertTime = Time.time;
        if (currentIdleTarget) StopIdleAnimation();
    }

    private void StopIdleAnimation()
    {
        currentIdleTarget.StopIdling(this);
        animator.SetBool(currentIdleTarget.AnimationName, false);
        ResetTarget();
    }
    // Leaf Actions
    private Node.Status IdleAnimation()
    {
        if (currentIdleTarget == null) return Node.Status.FAILURE;

        if (currentIdleTarget.CurrentAgent == this && !currentIdleTarget.Processing)
        {
            currentIdleTarget.StartIdling(this);
            animator.SetBool(currentIdleTarget.AnimationName, true);
            return Node.Status.RUNNING;
        }
        else if (currentIdleTarget.CurrentAgent == this)
        {
            return Node.Status.RUNNING;
        }
        else
        {
            // idle target occupied
            animator.SetBool(currentIdleTarget.AnimationName, false);
            return Node.Status.FAILURE;
        }
    }
    protected override Node.Status ResetTarget()
    {
        base.ResetTarget();
        currentIdleTarget = null;
        CombatManager.RemoveFromCombat(this);

        return Node.Status.SUCCESS;
    }
    private Node.Status OffsetLastSeenPlayerTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * 5f;
        lastPlayerSeenTransform.position += new Vector3(randomPoint.x, 0f, randomPoint.y);
        if(target == lastPlayerSeenTransform)
        {
            Speed = investigatingSpeed;
            targetState = TargetState.InProcess;
        }
        // adjust to terrain height
        return Node.Status.SUCCESS;
    }
    private Node.Status AttackPlayer()
    {
        if (Time.time - timeOfLastAttack < cooldown) return Node.Status.FAILURE;
        timeOfLastAttack = Time.time;
        targetState = TargetState.None;

        animator.SetBool("Attacking", true);
        return Node.Status.SUCCESS;
    }
    private Node.Status EvadeBack()
    {
        if (Time.time - timeOfLastEvade < evadeCooldown) return Node.Status.FAILURE;

        animator.SetTrigger("Evade");
        animator.SetBool("Attacking", false);
        evade = 0f;
        evading = true;
        timeOfLastEvade = Time.time;
        targetState = TargetState.None;
        return Node.Status.SUCCESS;
    }
    // Leaf Setters
    private Node.Status SetRandomFreeIdleTarget()
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < idleTargets.Length; i++)
        {
            indexes.Add(i);
        }
        while (indexes.Count > 0)
        {
            int randomIndex = indexes[UnityEngine.Random.Range(0, indexes.Count)];
            indexes.Remove(randomIndex);
            if (idleTargets[randomIndex].CurrentAgent) continue;

            currentIdleTarget = idleTargets[randomIndex];
            currentIdleTarget.CurrentAgent = this;
            SetTarget(currentIdleTarget.transform);

            return Node.Status.SUCCESS;
        }

        return Node.Status.FAILURE;
    }
    private Node.Status SetPlayerTarget()
    {
        SetTarget(PlayerInteraction.Instance.transform);
        CombatManager.AddInCombat(this);
        return Node.Status.SUCCESS;
    }
    private Node.Status SetLastSeenPlayerTarget()
    {
        SetTarget(lastPlayerSeenTransform);
        return Node.Status.SUCCESS;
    }
    // Leaf Checks
    private Node.Status CanSeePlayer()
    {
        if (seePlayer == SeePlayerState.DidNotCheck)
        {
            Vector3 playerPosition = PlayerInteraction.Instance.transform.position;
            Node.Status s = CanSee(playerPosition);
            if (s == Node.Status.SUCCESS)
            {
                seePlayer = SeePlayerState.SeePlayer;
                lastPlayerSeenTransform.position = playerPosition;
                Alert();
            }
            else
            {
                seePlayer = SeePlayerState.DoNotSeePlayer;
            }
            return s;
        }
        else
        {
            return seePlayer == SeePlayerState.SeePlayer ? Node.Status.SUCCESS : Node.Status.FAILURE;
        }
    }
    private Node.Status IsIdleTarget()
    {
        return currentIdleTarget != null ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsAlert()
    {
        if (alerted && Time.time - alertTime > alertTimer)
        {
            alerted = false;
        }
        return alerted ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsPlayerTarget()
    {
        return target == PlayerInteraction.Instance.transform ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsLastSeenPlayerTarget()
    {
        return target == lastPlayerSeenTransform ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsAttacking()
    {
        return animator.GetBool("Attacking") ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsKnockbacked()
    {
        return animator.GetBool("Knockback") ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsPlayerClose()
    {
        return Vector3.Distance(transform.position, PlayerInteraction.Instance.transform.position) < closeDistance ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsPlayerAttacking()
    {
        return PlayerInteraction.Instance.IsAttacking() ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private bool IsEvading()
    {
        return evading;
    }
    protected override bool Prebehave()
    {
        seePlayer = SeePlayerState.DidNotCheck;
        if (IsKnockbacked() == Node.Status.SUCCESS) return true;
        if (IsEvading()) return true;

        return false;
    }
    // Animation
    public void AnimationEvent_AttackImpact()
    {
        Speed = walkSpeed;
        Health playerHealth = PlayerInteraction.Instance.Health;
        float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
        if (distance > closeDistance) return;

        playerHealth.DealDamage(attackDamage, transform.position);
    }
    public void AnimationEvent_IdleActionAnimationFinished()
    {
        if (currentIdleTarget)
        {
            StopIdleAnimation();
        }
        else
        {
            throw new System.Exception("No idle target for " + name);
        }
    }
    // Health
    private void OnDamage(Vector3 direction)
    {
        Alert();
        lastPlayerSeenTransform.position = transform.position + direction;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        targetState = TargetState.None;
        animator.SetBool("Knockback", true);
        animator.SetBool("Attacking", false);
    }
    private void OnDeath()
    {
        Destroy(gameObject);
        enabled = false;
    }
    private void OnDestroy()
    {
        if (List.Contains(this)) List.Remove(this);
        if (lastPlayerSeenTransform) Destroy(lastPlayerSeenTransform.gameObject);
    }
}
