using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressPiece : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText = null;
    [SerializeField] private GapText[] gapTexts = null;

    public GapText[] GapTexts => gapTexts;
    private Status status = Status.NotAcquired;

    private void Awake()
    {
        foreach(GapText gapText in gapTexts)
        {
            gapText.AddOnTextSet(OnGapTextSet);
        }
        SetStatus(Status.NotAcquired);
    }
    private void OnGapTextSet()
    {
        // First check if all gaps are filled in
        foreach(GapText gapText in gapTexts)
        {
            if (!gapText.IsInfoSet())
            {
                SetStatus(Status.EmptyGaps);
                return;
            }
        }
        // Then check if info is set correctly
        foreach(GapText gapText in gapTexts)
        {
            if (!gapText.InfoSetCorrectly())
            {
                SetStatus(Status.CompletedWrong);
                return;
            }
        }
        // If everything passed piece is completed correctly, proceed further
        SetStatus(Status.CompletedCorrect);
        ProgressPieceCompleted();
    }
    private void ProgressPieceCompleted()
    {
        foreach(GapText gapText in gapTexts)
        {
            gapText.LockGapTextCorrect();
        }
    }
    private void UpdateStatusText()
    {
        switch (status)
        {
            case Status.NotAcquired:
                statusText.text = InternalSettings.Get.StatusText_NotAcquired; break;
            case Status.EmptyGaps:
                statusText.text = InternalSettings.Get.StatusText_EmptyGaps; break;
            case Status.CompletedWrong:
                statusText.text = InternalSettings.Get.StatusText_CompletedWrong; break;
            case Status.CompletedCorrect:
                statusText.text = InternalSettings.Get.StatusText_CompletedCorrect; break;
            default:
                statusText.text = "STATUS TEXT NOT SET"; break;
        }
    }
    // Getters
    public Status GetStatus()
    {
        return status;
    }
    // Setters
    private void SetStatus(Status newStatus)
    {
        if (status == newStatus) return;

        status = newStatus;
        UpdateStatusText();
    }
    public enum Status
    {
        NotAcquired,
        Investigating,
        AllInfoProvided,
        CompletedWrong,
        CompletedCorrect,
        EmptyGaps
    }
}
