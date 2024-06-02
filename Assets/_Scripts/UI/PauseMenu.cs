using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PauseMenu : TailPanel
{
    [SerializeField] private TailPanel settingsPanel = null;
    public void Button_Continue()
    {
        if(UIManager.IsOpen(this)) UIManager.Close(this);
    }
    public void Button_Settings()
    {
        UIManager.Open(settingsPanel);
    }
    public void Button_QuitToMenu()
    {
        Debug.Log("Autosave");
        Debug.Log("Quit to menu");
        
    }
    public void Button_QuitGame()
    {
        // Autosave before quit
        Debug.Log("Autosave");
        Debug.Log("Quit game");
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
