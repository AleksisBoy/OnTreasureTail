using UnityEngine;

public class AIIdleTarget : MonoBehaviour
{
    [SerializeField] private string animationName = string.Empty;

    private bool processing = false;
    private AIAgent currentAgent = null;
    public Vector3 Position => transform.position;
    public string AnimationName => animationName;
    public AIAgent CurrentAgent
    {
        get => currentAgent;
        set => currentAgent = value;
    }
    public bool Processing => processing;
    public void StartIdling(AIAgent agent)
    {
        if(agent == currentAgent)
            processing = true;
    }
    public void StopIdling(AIAgent agent)
    {
        if(agent == currentAgent)
        {
            currentAgent = null;
            processing = false;
        }
    }
}
