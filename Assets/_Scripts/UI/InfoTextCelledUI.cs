using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoTextCelledUI : InfoCelledUI
{
    [SerializeField] private TMP_Text textUI = null;

    public override void Set(InfoCelled infoCelled)
    {
        base.Set(infoCelled);
        textUI.text = ((InfoTextSO)infoCelled.info).Text;
    }
}
