using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    [Header("AI Agent")]
    [SerializeField] private float updateTime = 0.1f;
    [SerializeField] protected NavMeshAgent agent = null;
    [SerializeField] protected Animator animator = null;
    [SerializeField] protected float walkSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] protected float closeDistance = 2f;
    [Range(-1f, 1f)]
    [SerializeField] private float canSeeRange = 1f;
    [Range(0f, 100f)]
    [SerializeField] private float lookDistance = 10f;
    [SerializeField] private Vector3 eyeOffset = Vector3.zero;

    private Action onTargetChanged;

    protected BehaviourTree tree;
    protected Transform target = null;
    protected Node.Status status = Node.Status.RUNNING;
    protected AIState state = AIState.IDLE;
    protected enum AIState
    {
        IDLE,
        GOING,
        TRIGGERED
    }
    protected TargetState targetState = TargetState.None;
    protected enum TargetState
    {
        None,
        InProcess,
        InTargetRange
    }
    protected float Speed
    {
        get
        {
            return agent.speed;
        }
        set
        {
            if (value < 0f) value = 0f;
            agent.speed = value;
            animator.SetFloat("Speed", agent.speed);
        }
    }
    protected virtual void Awake()
    {
        Speed = walkSpeed;
        updateTime *= UnityEngine.Random.Range(0.8f, 1.2f);
        tree = new BehaviourTree();
    }
    protected virtual void Start()
    {
        StartCoroutine(Behave());
    }
    protected virtual void FixedUpdate()
    {
        if (!target) return;

        agent.SetDestination(target.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up), rotationSpeed * Time.fixedDeltaTime);
    }
    protected virtual void Update()
    {
        if (targetState != TargetState.InProcess) return;

        if (target && agent.path.corners.Length > 1)
        {
            transform.position += (agent.path.corners[1] - transform.position).normalized * Speed * Time.deltaTime;
        }
    }
    // Leaf Actions
    protected virtual Node.Status ResetTarget()
    {
        SetTarget(null);
        return Node.Status.SUCCESS;
    }
    // Leaf Setters
    protected Node.Status SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            target = null;
            state = AIState.IDLE;
            targetState = TargetState.None;
            return Node.Status.FAILURE;
        }
        else
        {
            if (target != newTarget)
            {
                Call_OnTargetChanged();
            }
            target = newTarget;
            targetState = TargetState.InProcess;
            Speed = walkSpeed;
            state = AIState.GOING;
            return Node.Status.SUCCESS;
        }
    }
    // Leaf Checks
    protected Node.Status IsCloseToTarget()
    {
        if (targetState == TargetState.InTargetRange) return Node.Status.SUCCESS;
        if (target == null) return Node.Status.FAILURE;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < closeDistance)
        {
            //target = null;
            Speed = 0f;
            targetState = TargetState.InTargetRange;
            return Node.Status.SUCCESS;
        }
        else if (Vector3.Distance(agent.pathEndPosition, target.position) >= closeDistance)
        {
            ResetTarget();
            return Node.Status.FAILURE;
        }
        else
        {
            targetState = TargetState.InProcess;
            Speed = walkSpeed;
            return Node.Status.RUNNING;
        }
    }
    protected Node.Status CanSee(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        if (distance > lookDistance) return Node.Status.FAILURE;

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToTarget);

        if (dot >= canSeeRange)
        {
            if (!Physics.Raycast(transform.position + eyeOffset, directionToTarget, lookDistance, InternalSettings.Get.EnvironmentMask))
            {
                return Node.Status.SUCCESS;
            }
        }

        return Node.Status.FAILURE;
    }
    protected virtual bool Prebehave()
    {
        return false;
    }
    private IEnumerator Behave()
    {
        if (tree == null)
        {
            throw new System.Exception("Behaviour Tree not set for " + name);
        }
        while (true)
        {
            bool skipBehave = Prebehave();
            if (!skipBehave) status = tree.Process();

            yield return new WaitForSeconds(updateTime);
        }
    }
    private void Call_OnTargetChanged()
    {
        if (onTargetChanged != null) onTargetChanged();
    }
    // Deprecated
    protected Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == AIState.IDLE)
        {
            agent.SetDestination(destination);
            state = AIState.GOING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= closeDistance)
        {
            state = AIState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < closeDistance)
        {
            state = AIState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }
    private void OnDrawGizmosSelected()
    {
        // draw look distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookDistance);

        // draw view
        Gizmos.color = Color.green;
        if (canSeeRange >= 0f)
        {
            Gizmos.DrawLine(transform.position + eyeOffset, transform.position + (Vector3.Lerp(transform.right, transform.forward, canSeeRange) * lookDistance) + eyeOffset);
            Gizmos.DrawLine(transform.position + eyeOffset, transform.position + (Vector3.Lerp(-transform.right, transform.forward, canSeeRange) * lookDistance) + eyeOffset);
        }
        else
        {
            Gizmos.DrawLine(transform.position + eyeOffset, transform.position + (Vector3.Lerp(transform.right, -transform.forward, -canSeeRange) * lookDistance) + eyeOffset);
            Gizmos.DrawLine(transform.position + eyeOffset, transform.position + (Vector3.Lerp(-transform.right, -transform.forward, -canSeeRange) * lookDistance) + eyeOffset);
        }

        // draw close distance
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, closeDistance);
    }
}
