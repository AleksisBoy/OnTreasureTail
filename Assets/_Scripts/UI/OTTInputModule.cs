using UnityEngine;
using UnityEngine.EventSystems;

public class OTTInputModule : StandaloneInputModule
{
    public static OTTInputModule Module { get; private set; }
    protected override void Awake()
    {
        if (Module == null) Module = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public GameObject GetFocused()
    {
        return GetCurrentFocusedGameObject();
    }
}
