using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandProgressPanel : TailPanel
{
    [Header("Progress Menu")]
    [SerializeField] private ProgressPiece[] progressPieces = null;
    [SerializeField] private InformationBar informationBar = null;

    protected override void Awake()
    {
        List<ICellDragEnd> list = new List<ICellDragEnd>();
        foreach(ProgressPiece piece in progressPieces)
        {
            foreach(GapText gapText in piece.GapTexts)
            {
                list.Add(gapText);
            }
        }

        informationBar.SetViewportDragObjects(list.ToArray());

        base.Awake();
    }
}
