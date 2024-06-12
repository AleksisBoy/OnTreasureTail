using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class AIEnemy : AIAgent
{
    [Header("AIEnemy")]
    [SerializeField] private AIIdleTarget[] idleTargets = null;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float cooldown = 2f;

    private float timeOfLastAttack = 0f;
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
    protected override void Awake()
    {
        if (!List.Contains(this)) List.Add(this);
        health = GetComponent<Health>();

        base.Awake();

        // Checkers
        Leaf isTriggered = new Leaf("Is Triggered", IsTriggered);
        Inverter isNotTriggered = new Inverter("Is Not Triggered");
        isNotTriggered.AddChild(isTriggered);

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
        beIdleConditions.AddChild(isNotTriggered);
        beIdleCondition.AddChild(beIdleConditions);

        DepSequence beIdle = new DepSequence("Idle Behaviour", beIdleCondition, agent);

        Leaf doIdleAnimation = new Leaf("Idle Animation", IdleAnimation);

        Selector findIdleTarget = new Selector("Find Idle Target");
        Leaf hasIdleTarget = new Leaf("Has Idle Target", HasIdleTarget);
        Leaf setRandomFreeIdleTarget = new Leaf("Set Random Free Idle Target", SetRandomFreeIdleTarget);

        findIdleTarget.AddChild(hasIdleTarget);
        findIdleTarget.AddChild(setRandomFreeIdleTarget);

        beIdle.AddChild(findIdleTarget);
        beIdle.AddChild(isCloseToTarget);
        beIdle.AddChild(doIdleAnimation);

        // Chase Player DepSequence
        BehaviourTree dependancyChasePlayer = new BehaviourTree("Dependancy chase player");
        Sequence dependancyChasePlayerSequence = new Sequence("dependancyChasePlayerSequence");
        dependancyChasePlayerSequence.AddChild(isNotAttacking);
        dependancyChasePlayerSequence.AddChild(canSeePlayer);
        dependancyChasePlayer.AddChild(dependancyChasePlayerSequence);

        DepSequence chasePlayer = new DepSequence("Chase player", dependancyChasePlayer, agent);
        Selector findPlayerTarget = new Selector("Find Player target");
        Leaf setPlayerTarget = new Leaf("Set Player target", SetPlayerTarget);
        findPlayerTarget.AddChild(isPlayerTarget);
        findPlayerTarget.AddChild(setPlayerTarget);

        Leaf attackPlayer = new Leaf("Attack Player", AttackPlayer);

        chasePlayer.AddChild(findPlayerTarget);
        chasePlayer.AddChild(isCloseToTarget);
        chasePlayer.AddChild(attackPlayer);

        //
        Leaf resetTarget = new Leaf("Reset target", ResetTarget);

        // Enemy Behaviour
        Selector behaveEnemy = new Selector("Behave Like Enemy");
        behaveEnemy.AddChild(beIdle);
        behaveEnemy.AddChild(chasePlayer);
        behaveEnemy.AddChild(resetTarget);

        tree.AddChild(behaveEnemy);

        tree.PrintTree();
    }
    protected override void Prebehave()
    {
        seePlayer = SeePlayerState.DidNotCheck;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        animator.SetFloat("Speed", agent.speed);
    }
    private Node.Status CanSeePlayer()
    {
        if(seePlayer == SeePlayerState.DidNotCheck)
        {
            Node.Status s = CanSee(PlayerInteraction.Instance.transform.position);
            if(s == Node.Status.SUCCESS)
            {
                seePlayer = SeePlayerState.SeePlayer;
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
    private Node.Status HasIdleTarget()
    {
        return currentIdleTarget != null ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status SetRandomFreeIdleTarget()
    {
        List<int> indexes = new List<int>();
        for(int i = 0; i < idleTargets.Length; i++)
        {
            indexes.Add(i);
        }
        while(indexes.Count > 0)
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
    private Node.Status IdleAnimation()
    {
        if(currentIdleTarget == null) return Node.Status.FAILURE;

        if(currentIdleTarget.CurrentAgent == this && !currentIdleTarget.Processing)
        {
            agent.speed = 0f;
            currentIdleTarget.StartIdling(this);
            animator.SetBool(currentIdleTarget.AnimationName, true);
            return Node.Status.RUNNING;
        }
        else if(currentIdleTarget.CurrentAgent == this)
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
    public void AnimationEvent_IdleActionAnimationFinished()
    {
        if (currentIdleTarget)
        {
            currentIdleTarget.StopIdling(this);
            animator.SetBool(currentIdleTarget.AnimationName, false);
            currentIdleTarget = null;
            agent.speed = walkSpeed;
        }
        else
        {
            throw new System.Exception("No idle target for " + name);
        }
    }
    protected override Node.Status ResetTarget()
    {
        base.ResetTarget();
        currentIdleTarget = null;
        CombatManager.RemoveFromCombat(this);

        return Node.Status.SUCCESS;
    }
    private Node.Status IsTriggered()
    {
        return state == AIState.TRIGGERED ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status IsPlayerTarget()
    {
        return target == PlayerInteraction.Instance.transform ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status SetPlayerTarget()
    {
        target = PlayerInteraction.Instance.transform;
        CombatManager.AddInCombat(this);
        return Node.Status.SUCCESS;
    }
    private Node.Status IsAttacking()
    {
        return animator.GetBool("Attacking") ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }
    private Node.Status AttackPlayer()
    {
        if(Time.time - timeOfLastAttack < cooldown) return Node.Status.FAILURE;
        timeOfLastAttack = Time.time;

        animator.SetBool("Attacking", true);
        agent.speed = 0f;
        return Node.Status.SUCCESS;
    }
    public void AnimationEvent_AttackImpact()
    {
        agent.speed = walkSpeed;
        Health playerHealth = PlayerInteraction.Instance.Health;
        float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
        if (distance > closeDistance) return; 

        playerHealth.DealDamage(attackDamage);
    }
    private void OnDestroy()
    {
        if (List.Contains(this)) List.Remove(this);
    }
}
