using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleTarget : MonoBehaviour
{
    [SerializeField] private string animationName = string.Empty;

    private AIAgent currentAgent = null;
    public Vector3 Position => transform.position;
    public string AnimationName => animationName;
    public AIAgent CurrentAgent => currentAgent;
    public void StartIdling(AIAgent agent)
    {
        currentAgent = agent;
    }
    public void StopIdling()
    {
        currentAgent = null;
    }
}
