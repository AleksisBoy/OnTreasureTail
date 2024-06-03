using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressPiece : MonoBehaviour
{
    private Status status = Status.NotAcquired;
    /*
    [SerializeField] private InfoPieceUI[] infoPieces = null;

    private void Awake()
    {
        foreach(var piece in infoPieces)
        {
            piece.gameObject.SetActive(false);
        }
    }
    public void UpdateInfoID(string infoID)
    {
        foreach (InfoPieceUI piece in infoPieces)
        {

        }
    }
    private void UpdateStatus()
    {
        foreach (InfoPieceUI piece in infoPieces)
        {

        }
    }
    */
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
