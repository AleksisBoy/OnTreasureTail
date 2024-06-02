using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TailPanel pauseMenu = null;
    private void Update()
    {
        UIInput();
    }

    private void UIInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || UIManager.Blocking) return;

        if (UIManager.IsOpen()) UIManager.CloseLast();
        else
        {
            bool opened = UIManager.Open(pauseMenu, OnPauseMenuClose);
            if (opened) PlayerInteraction.Instance.EnablePlayerComponents(false);
        }
    }
    private void OnPauseMenuClose()
    {
        PlayerInteraction.Instance.EnablePlayerComponents(true);
    }
}
