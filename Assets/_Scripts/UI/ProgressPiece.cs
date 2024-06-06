using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressPiece : MonoBehaviour
{
    private Status status = Status.NotAcquired;
    


    private void Awake()
    {

    }
    public void UpdateInfoID(string infoID)
    {

    }
    private void UpdateStatus()
    {

    }
    
    // Getters
    public Status GetStatus()
    {
        return status;
    }
    public enum Status
    {
        NotAcquired,
        Investigating,
        AllInfoProvided,
        CompletedWrong,
        CompletedCorrect
    }
}
