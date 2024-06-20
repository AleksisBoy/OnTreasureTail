using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompass : PlayerSubinteraction
{
    [Header("Compass")]
    [SerializeField] private float onCompassWalkSpeed = 2f;
    [SerializeField] private float onCompassSprintWalkSpeed = 2f;

    private bool onCompass = false;
    public float WalkSpeed => onCompassWalkSpeed;
    public float SprintSpeed => onCompassSprintWalkSpeed;
    public override void Set(Animator animator, params object[] setList)
    {
        base.Set(animator, setList);

        foreach(var obj in setList)
        {
            if(obj as PlayerMovement)
            {
                PlayerMovement movement = (PlayerMovement)obj;
                movement.SetCompass(this);
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetOnCompass(true);
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            SetOnCompass(false);
        }
    }
    private void SetOnCompass(bool state)
    {
        onCompass = state;
        animator?.SetBool("OnCompass", state);
        UserInterface.Instance?.ShowCompassUI(state);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        SetOnCompass(false);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        SetOnCompass(false);
    }
    public bool OnCompass()
    {
        return onCompass;
    }
}
