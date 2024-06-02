using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TailPanel pauseMenu = null;
    [SerializeField] private PlayerInteraction playerInteraction = null;
    private void Update()
    {
        UIInput();
    }

    private void UIInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (UIManager.IsOpen()) UIManager.CloseLast();
        else
        {
            bool opened = UIManager.Open(pauseMenu, OnPauseMenuClose);
            if (opened) playerInteraction.EnablePlayerComponents(false);
        }
    }
    private void OnPauseMenuClose()
    {
        playerInteraction.EnablePlayerComponents(true);
    }
}
