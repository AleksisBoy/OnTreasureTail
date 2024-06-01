using UnityEditor;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;

// ChatGPT
[InitializeOnLoad]
public class OpenVisualStudioOnProjectOpen
{
    private static bool isOpen = false;
    static OpenVisualStudioOnProjectOpen()
    {
        if (isOpen) return;

        EditorApplication.projectWindowItemOnGUI += OnProjectOpen;
    }

    private static void OnProjectOpen(string guid, Rect selectionRect)
    {
        EditorApplication.projectWindowItemOnGUI -= OnProjectOpen;

        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string solutionFilePath = Directory.GetFiles(projectPath, "*.sln")[0];

        if (solutionFilePath != null && !IsVisualStudioRunningWithProject(projectPath))
        {
            Process.Start(solutionFilePath);
            isOpen = true;
        }
    }
    private static bool IsVisualStudioRunningWithProject(string projectPath)
    {
        Process[] processes = Process.GetProcessesByName("devenv");
        
        string projectName = projectPath.Substring(projectPath.LastIndexOf('\\') + 1);
        foreach (Process process in processes)
        {
            string windowTitle = GetWindowTitle(process.MainWindowHandle);
            if (windowTitle.Contains(projectName, StringComparison.OrdinalIgnoreCase))
            {
                isOpen = true;
                return true;
            }
        }
        return false;
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    private static string GetWindowTitle(IntPtr hWnd)
    {
        const int nChars = 256;
        StringBuilder Buff = new StringBuilder(nChars);
        if (GetWindowText(hWnd, Buff, nChars) > 0)
        {
            return Buff.ToString();
        }
        return null;
    }
}
