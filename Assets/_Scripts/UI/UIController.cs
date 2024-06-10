using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TailPanel pauseMenu = null;
    [SerializeField] private TailPanel islandProgress = null;
    private void Update()
    {
        UIInput();

        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        UIManager.Toggle(islandProgress);
    }

    private void UIInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (UIManager.IsOpen()) UIManager.CloseLast();
        else
        {
            UIManager.Open(pauseMenu);
        }
    }
}
