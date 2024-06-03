using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandProgressPanel : TailPanel
{
    [Header("Progress Menu")]
    [SerializeField] private ProgressPiece progressPieces = null;
    [SerializeField] private InformationProgressBar informationBar = null;
}
