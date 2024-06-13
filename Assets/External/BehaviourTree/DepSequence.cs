using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node 
{
    private BehaviourTree dependancy;
    private NavMeshAgent agent;
    public DepSequence(string n, BehaviourTree d, NavMeshAgent a) 
    {
        name = n;
        dependancy = d;
        agent = a;
    }

    public override Status Process() 
    {
        if (dependancy.Process() == Status.FAILURE) 
        {
            agent.ResetPath();
            Reset();
            return Status.FAILURE;
        }

        Status childstatus = children[currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.FAILURE)
        {
            Reset();
            return childstatus;
        }
        
        currentChild++;
        if (currentChild >= children.Count) 
        {
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }
}
